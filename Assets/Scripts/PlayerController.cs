using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    private const int PlayerSortingOrder = 10;
    private const float PlayerVisualScale = 3f;
    private const int SpriteSize = 16;
    private const int DirectionFrameCount = 4;
    private const string PlayerVisualName = "PlayerVisual";
    private const string WalkSpritePath = "Assets/Art/Characters/Vampire/SeparateAnim/Walk.png";
    private static readonly Vector2 PlayerColliderOffset = new Vector2(0f, -0.08f);
    private static readonly Vector2 PlayerColliderSize = new Vector2(0.28f, 0.32f);

    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float animationFrameRate = 8f;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private Vector2 moveInput;
    private SpriteRenderer visualRenderer;
    private Sprite[][] walkFramesByDirection;
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
        Vector2 nextPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);
    }

    public void MultiplyMoveSpeed(float multiplier)
    {
        if (multiplier <= 0f)
            return;

        moveSpeed *= multiplier;
    }

    private void ConfigureSpriteRenderer()
    {
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
        if (moveInput.sqrMagnitude <= 0.01f)
            return;

        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            facingDirection = moveInput.x < 0f ? Direction.Left : Direction.Right;
        else
            facingDirection = moveInput.y < 0f ? Direction.Down : Direction.Up;
    }

    private void UpdateAnimation()
    {
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
