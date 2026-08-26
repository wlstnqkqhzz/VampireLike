using UnityEngine;

/// <summary>
/// 플레이어용 3프레임 스프라이트 시트를 런타임에 잘라 Idle, Walk, Attack, Hit, Death, Cast 모션으로 재생합니다.
/// </summary>
public class PlayerSpriteAnimator : MonoBehaviour
{
    private const string PlayerVisualName = "PlayerVisual";
    private const int DefaultFrameCount = 3;
    private const float ExpectedFrameHeight = 320f;

    [SerializeField]
    private string resourceFolder = "PlayerAnimations/KaelProcessed";

    [SerializeField]
    private float pixelsPerUnit = 180f;

    [SerializeField]
    private float visualScale = 1f;

    [SerializeField]
    private bool invertHorizontalFacing = true;

    [SerializeField]
    private bool invertIdleHorizontalFacing;

    [SerializeField]
    private bool invertWalkHorizontalFacing;

    [SerializeField]
    private float idleFrameRate = 4f;

    [SerializeField]
    private float walkFrameRate = 5.5f;

    [SerializeField]
    private bool pingPongWalk = true;

    [SerializeField]
    private float attackFrameRate = 10f;

    [SerializeField]
    private float hitFrameRate = 10f;

    [SerializeField]
    private float deathFrameRate = 6f;

    [SerializeField]
    private float castFrameRate = 8f;

    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;
    private Sprite[] idleFrames;
    private Sprite[] walkFrames;
    private Sprite[] attackFrames;
    private Sprite[] hitFrames;
    private Sprite[] deathFrames;
    private Sprite[] castFrames;
    private Sprite[] currentLoopFrames;
    private Sprite[] oneShotFrames;
    private float currentLoopFrameRate;
    private float oneShotFrameRate;
    private float loopTimer;
    private float oneShotTimer;
    private int loopFrameIndex;
    private int loopFrameStep = 1;
    private int oneShotFrameIndex;
    private bool isPlayingOneShot;
    private bool isDead;
    private bool isFacingLeft;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        spriteRenderer = GetOrCreateVisualRenderer();
        LoadAllFrames();
        PlayIdle();
    }

    private void OnValidate()
    {
        pixelsPerUnit = Mathf.Max(1f, pixelsPerUnit);
        visualScale = Mathf.Max(0.01f, visualScale);
        idleFrameRate = Mathf.Max(0.01f, idleFrameRate);
        walkFrameRate = Mathf.Max(0.01f, walkFrameRate);
        attackFrameRate = Mathf.Max(0.01f, attackFrameRate);
        hitFrameRate = Mathf.Max(0.01f, hitFrameRate);
        deathFrameRate = Mathf.Max(0.01f, deathFrameRate);
        castFrameRate = Mathf.Max(0.01f, castFrameRate);
    }

    private void LateUpdate()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetOrCreateVisualRenderer();

        if (!isDead && !isPlayingOneShot)
            UpdateFacing();

        if (isPlayingOneShot)
        {
            UpdateOneShot();
            return;
        }

        UpdateLoop();
    }

    /// <summary>
    /// PlayerController가 만든 PlayerVisual SpriteRenderer를 공유하도록 연결합니다.
    /// </summary>
    public void SetVisualRenderer(SpriteRenderer renderer)
    {
        spriteRenderer = renderer;
        ApplyVisualSettings();

        if (idleFrames == null || idleFrames.Length == 0)
            LoadAllFrames();

        if (spriteRenderer != null && spriteRenderer.sprite == null)
            PlayIdle();
    }

    /// <summary>
    /// 캐릭터 선택에서 다른 캐릭터 리소스 폴더를 지정할 때 사용합니다.
    /// </summary>
    public void SetResourceFolder(string folder)
    {
        if (string.IsNullOrWhiteSpace(folder) || resourceFolder == folder)
            return;

        resourceFolder = folder;
        LoadAllFrames();
        isDead = false;
        isPlayingOneShot = false;
        PlayIdle();
    }

    public void SetInvertHorizontalFacing(bool shouldInvert)
    {
        invertHorizontalFacing = shouldInvert;
        ApplySpriteFacing(spriteRenderer == null ? null : spriteRenderer.sprite);
    }

    public void SetInvertWalkHorizontalFacing(bool shouldInvert)
    {
        invertWalkHorizontalFacing = shouldInvert;
        ApplySpriteFacing(spriteRenderer == null ? null : spriteRenderer.sprite);
    }

    public void SetInvertIdleHorizontalFacing(bool shouldInvert)
    {
        invertIdleHorizontalFacing = shouldInvert;
        ApplySpriteFacing(spriteRenderer == null ? null : spriteRenderer.sprite);
    }

    public void PlayAttack()
    {
        PlayOneShot(attackFrames, attackFrameRate);
    }

    public void PlayAttack(Vector2 attackDirection)
    {
        FaceDirection(attackDirection);
        PlayAttack();
    }

    public void FaceDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) <= 0.01f)
            return;

        isFacingLeft = direction.x < 0f;
        ApplySpriteFacing(spriteRenderer == null ? null : spriteRenderer.sprite);
    }

    public void PlayHit()
    {
        if (isDead)
            return;

        PlayOneShot(hitFrames, hitFrameRate);
    }

    public void PlayDeath()
    {
        isDead = true;
        PlayOneShot(deathFrames, deathFrameRate, false);
    }

    public void PlayCast()
    {
        PlayOneShot(castFrames, castFrameRate);
    }

    private void UpdateFacing()
    {
        if (playerController == null)
            return;

        Vector2 moveInput = playerController.MoveInput;

        if (Mathf.Abs(moveInput.x) > 0.01f)
            isFacingLeft = moveInput.x < 0f;

        ApplySpriteFacing(spriteRenderer == null ? null : spriteRenderer.sprite);
    }

    private void UpdateLoop()
    {
        if (playerController == null)
            return;

        Sprite[] nextLoopFrames = playerController.IsMoving ? walkFrames : idleFrames;
        float nextFrameRate = playerController.IsMoving ? walkFrameRate : idleFrameRate;

        if (nextLoopFrames == null || nextLoopFrames.Length == 0)
            return;

        if (currentLoopFrames != nextLoopFrames)
        {
            currentLoopFrames = nextLoopFrames;
            currentLoopFrameRate = nextFrameRate;
            loopTimer = 0f;
            loopFrameIndex = 0;
            loopFrameStep = 1;
            SetSprite(currentLoopFrames[loopFrameIndex]);
            return;
        }

        currentLoopFrameRate = nextFrameRate;
        loopTimer += Time.unscaledDeltaTime;

        if (loopTimer < 1f / currentLoopFrameRate)
            return;

        loopTimer = 0f;
        AdvanceLoopFrame();
        SetSprite(currentLoopFrames[loopFrameIndex]);
    }

    private void AdvanceLoopFrame()
    {
        if (currentLoopFrames == null || currentLoopFrames.Length <= 1)
            return;

        if (pingPongWalk && currentLoopFrames == walkFrames && currentLoopFrames.Length == 3)
        {
            loopFrameIndex += loopFrameStep;

            if (loopFrameIndex >= currentLoopFrames.Length - 1)
            {
                loopFrameIndex = currentLoopFrames.Length - 1;
                loopFrameStep = -1;
            }
            else if (loopFrameIndex <= 0)
            {
                loopFrameIndex = 0;
                loopFrameStep = 1;
            }

            return;
        }

        loopFrameIndex = (loopFrameIndex + 1) % currentLoopFrames.Length;
    }

    private void UpdateOneShot()
    {
        if (oneShotFrames == null || oneShotFrames.Length == 0)
        {
            isPlayingOneShot = false;
            return;
        }

        oneShotTimer += Time.unscaledDeltaTime;

        if (oneShotTimer < 1f / oneShotFrameRate)
            return;

        oneShotTimer = 0f;
        oneShotFrameIndex++;

        if (oneShotFrameIndex >= oneShotFrames.Length)
        {
            if (isDead)
            {
                oneShotFrameIndex = oneShotFrames.Length - 1;
                SetSprite(oneShotFrames[oneShotFrameIndex]);
                return;
            }

            isPlayingOneShot = false;
            currentLoopFrames = null;
            return;
        }

        SetSprite(oneShotFrames[oneShotFrameIndex]);
    }

    private void PlayIdle()
    {
        currentLoopFrames = null;
        oneShotFrames = null;
        isPlayingOneShot = false;

        if (idleFrames != null && idleFrames.Length > 0)
            SetSprite(idleFrames[0]);
    }

    private void PlayOneShot(Sprite[] frames, float frameRate, bool returnToLoop = true)
    {
        if (frames == null || frames.Length == 0)
            return;

        oneShotFrames = frames;
        oneShotFrameRate = Mathf.Max(0.01f, frameRate);
        oneShotFrameIndex = 0;
        oneShotTimer = 0f;
        isPlayingOneShot = true;
        SetSprite(oneShotFrames[0]);
    }

    private void SetSprite(Sprite sprite)
    {
        if (spriteRenderer == null || sprite == null)
            return;

        spriteRenderer.sprite = sprite;
        ApplySpriteFacing(sprite);
    }

    private void ApplySpriteFacing(Sprite sprite)
    {
        if (spriteRenderer == null)
            return;

        bool shouldInvert = invertHorizontalFacing;

        if (invertIdleHorizontalFacing && IsIdleSprite(sprite))
            shouldInvert = !shouldInvert;

        if (invertWalkHorizontalFacing && IsWalkSprite(sprite))
            shouldInvert = !shouldInvert;

        spriteRenderer.flipX = shouldInvert ? !isFacingLeft : isFacingLeft;
    }

    private bool IsIdleSprite(Sprite sprite)
    {
        if (sprite == null || idleFrames == null)
            return false;

        for (int i = 0; i < idleFrames.Length; i++)
        {
            if (idleFrames[i] == sprite)
                return true;
        }

        return false;
    }

    private bool IsWalkSprite(Sprite sprite)
    {
        if (sprite == null || walkFrames == null)
            return false;

        for (int i = 0; i < walkFrames.Length; i++)
        {
            if (walkFrames[i] == sprite)
                return true;
        }

        return false;
    }

    private void LoadAllFrames()
    {
        idleFrames = LoadStrip("01_Idle_3Frames", DefaultFrameCount);
        walkFrames = LoadStrip("02_Walk_Production_8Frames", 8);

        if (walkFrames.Length == 0)
            walkFrames = LoadStrip("02_Walk_3Frames", DefaultFrameCount);

        attackFrames = LoadStrip("03_Attack_3Frames", DefaultFrameCount);
        hitFrames = LoadStrip("04_Hit_3Frames", DefaultFrameCount);
        deathFrames = LoadStrip("05_Death_3Frames", DefaultFrameCount);
        castFrames = LoadStrip("06_Cast_3Frames", DefaultFrameCount);
        currentLoopFrames = null;
    }

    private Sprite[] LoadStrip(string fileName, int frameCount)
    {
        Texture2D texture = Resources.Load<Texture2D>($"{resourceFolder}/{fileName}");

        if (texture == null)
            return System.Array.Empty<Sprite>();

        frameCount = Mathf.Max(1, frameCount);
        int frameWidth = texture.width / frameCount;
        int frameHeight = texture.height;
        float effectivePixelsPerUnit = pixelsPerUnit * Mathf.Max(0.01f, frameHeight / ExpectedFrameHeight);
        Sprite[] frames = new Sprite[frameCount];

        for (int i = 0; i < frameCount; i++)
        {
            Rect rect = new Rect(frameWidth * i, 0f, frameWidth, frameHeight);
            Vector2 pivot = new Vector2(0.5f, 0.14f);
            frames[i] = Sprite.Create(texture, rect, pivot, effectivePixelsPerUnit);
        }

        return frames;
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
        }

        SpriteRenderer renderer = visual.GetComponent<SpriteRenderer>();

        if (renderer == null)
            renderer = visual.gameObject.AddComponent<SpriteRenderer>();

        ApplyVisualSettings(renderer);
        return renderer;
    }

    private void ApplyVisualSettings()
    {
        ApplyVisualSettings(spriteRenderer);
    }

    private void ApplyVisualSettings(SpriteRenderer renderer)
    {
        if (renderer == null)
            return;

        renderer.sortingOrder = 10;
        renderer.transform.localPosition = Vector3.zero;
        renderer.transform.localScale = Vector3.one * visualScale;
    }
}
