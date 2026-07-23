using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 방향키 입력을 받아 Rigidbody2D 기반으로 플레이어를 이동시키고,
/// 이동 방향에 맞는 플레이어 애니메이션 프레임을 표시한다.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    // 기본 플레이어 스프라이트/충돌 설정값이다.
    private const int PlayerSortingOrder = 10;
    private const float PlayerVisualScale = 3f;
    private const int SpriteSize = 16;
    private const int DirectionFrameCount = 4;
    private const float MinimumMoveSpeedMultiplier = 0.25f;
    private const string PlayerVisualName = "PlayerVisual";
    private const string WalkSpritePath = "Assets/Art/Characters/Vampire/SeparateAnim/Walk.png";
    private static readonly Vector2 PlayerColliderOffset = new Vector2(0f, -0.08f);
    private static readonly Vector2 PlayerColliderSize = new Vector2(0.28f, 0.32f);

    [SerializeField]
    private float moveSpeed = 5f;

    // 이동 애니메이션 프레임 전환 속도다.
    [SerializeField]
    private float animationFrameRate = 8f;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private Vector2 moveInput;
    private SpriteRenderer visualRenderer;
    private Sprite[][] walkFramesByDirection;
    private readonly Dictionary<object, float> moveSpeedMultipliers = new Dictionary<object, float>();
    private float currentMoveSpeedMultiplier = 1f;
    private float animationTimer;
    private int animationFrameIndex;
    private bool wasMoving;
    private Direction facingDirection = Direction.Down;
    private Direction previousFacingDirection = Direction.Down;

    private enum Direction
    {
        Down = 0,
        Up = 1,
        Left = 2,
        Right = 3
    }

    private void Awake()
    {
        // Rigidbody2D/Collider2D가 없으면 자동으로 보강해 플레이어가 항상 물리 이동 가능하게 한다.
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        playerCollider = GetComponent<Collider2D>();

        if (playerCollider == null)
            playerCollider = gameObject.AddComponent<CapsuleCollider2D>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        ConfigurePlayerCollider();
        ConfigureSpriteRenderer();
    }

    private void Start()
    {
        ConfigureSpriteRenderer();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        ConfigureSpriteRenderer();
    }
#endif

    private void Update()
    {
        moveInput = Vector2.zero;

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
            return;

        // 방향키 입력을 2D 벡터로 합산한다.
        if (keyboard.leftArrowKey.isPressed)
            moveInput.x -= 1;

        if (keyboard.rightArrowKey.isPressed)
            moveInput.x += 1;

        if (keyboard.upArrowKey.isPressed)
            moveInput.y += 1;

        if (keyboard.downArrowKey.isPressed)
            moveInput.y -= 1;

        moveInput = moveInput.normalized;
        UpdateFacingDirection();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        // Transform 직접 변경 대신 Rigidbody2D.MovePosition으로 이동해 충돌과 함께 동작하게 한다.
        Vector2 nextPosition = rb.position + moveInput * moveSpeed * currentMoveSpeedMultiplier * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);
    }

    /// <summary>
    /// 이동 속도 강화에서 호출하는 곱연산 기반 속도 증가 메서드다.
    /// </summary>
    public void MultiplyMoveSpeed(float multiplier)
    {
        if (multiplier <= 0f)
            return;

        moveSpeed *= multiplier;
    }

    /// <summary>
    /// 장판 같은 상태 이상이 플레이어 이동속도에 임시 배율을 적용할 때 사용한다.
    /// </summary>
    public void AddMoveSpeedMultiplier(object source, float multiplier)
    {
        if (source == null)
            return;

        moveSpeedMultipliers[source] = Mathf.Clamp(multiplier, MinimumMoveSpeedMultiplier, 1f);
        RecalculateMoveSpeedMultiplier();
    }

    /// <summary>
    /// 상태 이상이 끝났을 때 해당 source가 적용한 이동속도 배율을 제거한다.
    /// </summary>
    public void RemoveMoveSpeedMultiplier(object source)
    {
        if (source == null)
            return;

        if (moveSpeedMultipliers.Remove(source))
            RecalculateMoveSpeedMultiplier();
    }

    private void RecalculateMoveSpeedMultiplier()
    {
        currentMoveSpeedMultiplier = 1f;

        foreach (float multiplier in moveSpeedMultipliers.Values)
            currentMoveSpeedMultiplier *= multiplier;

        currentMoveSpeedMultiplier = Mathf.Max(MinimumMoveSpeedMultiplier, currentMoveSpeedMultiplier);
    }

    private void ConfigureSpriteRenderer()
    {
        // 루트 SpriteRenderer는 숨기고 PlayerVisual 자식에 실제 플레이어 이미지를 표시한다.
        visualRenderer = GetOrCreateVisualRenderer();

        if (visualRenderer.sortingOrder < PlayerSortingOrder)
            visualRenderer.sortingOrder = PlayerSortingOrder;

        visualRenderer.transform.localPosition = Vector3.zero;
        visualRenderer.transform.localScale = Vector3.one * PlayerVisualScale;

#if UNITY_EDITOR
        walkFramesByDirection = LoadEditorDirectionalSprites(WalkSpritePath);

        if (walkFramesByDirection != null)
            visualRenderer.sprite = walkFramesByDirection[(int)facingDirection][0];
#endif
    }

    private void ConfigurePlayerCollider()
    {
        // 픽셀 캐릭터 크기에 맞게 실제 충돌 범위를 작게 잡는다.
        if (playerCollider == null)
            return;

        playerCollider.isTrigger = false;

        if (playerCollider is CapsuleCollider2D capsuleCollider)
        {
            capsuleCollider.offset = PlayerColliderOffset;
            capsuleCollider.size = PlayerColliderSize;
            capsuleCollider.direction = CapsuleDirection2D.Vertical;
            return;
        }

        if (playerCollider is BoxCollider2D boxCollider)
        {
            boxCollider.offset = PlayerColliderOffset;
            boxCollider.size = PlayerColliderSize;
        }
    }

    private void UpdateFacingDirection()
    {
        // 마지막 이동 방향을 기억해 정지 중에도 바라보는 방향을 유지한다.
        if (moveInput.sqrMagnitude <= 0.01f)
            return;

        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            facingDirection = moveInput.x < 0f ? Direction.Left : Direction.Right;
        else
            facingDirection = moveInput.y < 0f ? Direction.Down : Direction.Up;
    }

    private void UpdateAnimation()
    {
        // 방향이 바뀌거나 정지/이동 상태가 바뀌면 첫 프레임부터 다시 재생한다.
        if (visualRenderer == null)
            return;

        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        Sprite[] frames = GetCurrentDirectionFrames();

        if (frames == null || frames.Length == 0)
            return;

        if (isMoving != wasMoving || facingDirection != previousFacingDirection)
        {
            animationFrameIndex = 0;
            animationTimer = 0f;
            wasMoving = isMoving;
            previousFacingDirection = facingDirection;
        }

        if (!isMoving)
        {
            visualRenderer.sprite = frames[0];
            return;
        }

        animationTimer += Time.unscaledDeltaTime;

        if (animationTimer >= 1f / animationFrameRate)
        {
            animationTimer = 0f;
            animationFrameIndex = (animationFrameIndex + 1) % frames.Length;
        }

        visualRenderer.sprite = frames[animationFrameIndex];
    }

    private Sprite[] GetCurrentDirectionFrames()
    {
        if (walkFramesByDirection == null)
            return null;

        return walkFramesByDirection[(int)facingDirection];
    }

    private SpriteRenderer GetOrCreateVisualRenderer()
    {
        // PlayerVisual 자식이 없으면 만들어서 실제 캐릭터 스프라이트 전용으로 사용한다.
        Transform visual = transform.Find(PlayerVisualName);

        if (visual == null)
        {
            GameObject visualObject = new GameObject(PlayerVisualName);
            visual = visualObject.transform;
            visual.SetParent(transform);
            visual.localPosition = Vector3.zero;
            visual.localRotation = Quaternion.identity;
            visual.localScale = Vector3.one;
        }

        SpriteRenderer spriteRenderer = visual.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            spriteRenderer = visual.gameObject.AddComponent<SpriteRenderer>();

        SpriteRenderer rootSpriteRenderer = GetComponent<SpriteRenderer>();

        if (rootSpriteRenderer != null)
            rootSpriteRenderer.enabled = false;

        return spriteRenderer;
    }

#if UNITY_EDITOR
    private static Sprite[][] LoadEditorDirectionalSprites(string assetPath)
    {
        // 에디터에서 Walk.png를 방향별/프레임별 Sprite로 잘라 애니메이션에 사용한다.
        Texture2D texture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

        if (texture == null)
            return null;

        Sprite[][] directionalSprites = new Sprite[4][];

        for (int direction = 0; direction < directionalSprites.Length; direction++)
        {
            directionalSprites[direction] = new Sprite[DirectionFrameCount];
            int x = SpriteSize * direction;

            for (int frame = 0; frame < DirectionFrameCount; frame++)
            {
                int y = texture.height - SpriteSize * (frame + 1);
                Rect rect = new Rect(x, y, SpriteSize, SpriteSize);
                Vector2 pivot = new Vector2(0.5f, 0.5f);
                directionalSprites[direction][frame] = Sprite.Create(texture, rect, pivot, 100f);
            }
        }

        return directionalSprites;
    }
#endif
}
