using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    private const int PlayerSortingOrder = 10;
    private const string DefaultSpritePath = "Assets/Art/Characters/Vampire/SeparateAnim/Idle.png";
    private const string DefaultSpriteName = "Idle_0";

    [SerializeField]
    private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        if (GetComponent<Collider2D>() == null)
            gameObject.AddComponent<CapsuleCollider2D>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        ConfigureSpriteRenderer();
    }

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
    }

    private void FixedUpdate()
    {
        Vector2 nextPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);
    }

    private void ConfigureSpriteRenderer()
    {
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        if (spriteRenderer.sortingOrder < PlayerSortingOrder)
            spriteRenderer.sortingOrder = PlayerSortingOrder;

#if UNITY_EDITOR
        if (spriteRenderer.sprite == null)
            spriteRenderer.sprite = LoadEditorSprite(DefaultSpritePath, DefaultSpriteName);
#endif
    }

#if UNITY_EDITOR
    private static Sprite LoadEditorSprite(string assetPath, string spriteName)
    {
        Object[] assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetPath);

        foreach (Object asset in assets)
        {
            if (asset is Sprite sprite && sprite.name == spriteName)
                return sprite;
        }

        return null;
    }
#endif
}
