using UnityEngine;

namespace VampireLike.Combat
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class HealthPackPickup : MonoBehaviour
    {
        private const int TextureSize = 32;
        private const string SmallHealthPackPath = "Pickups/health_pack_small";
        private const string LargeHealthPackPath = "Pickups/health_pack_large";

        [SerializeField]
        private int flatHealAmount = 5;

        [SerializeField]
        private float maxHealthHealRatio;

        [SerializeField]
        private float lifetime = 18f;

        private static Sprite generatedSmallSprite;
        private static Sprite generatedLargeSprite;
        private float bobOffset;
        private Vector3 startPosition;

        public static HealthPackPickup DropSmall(Vector3 position, int healAmount)
        {
            return Drop(position, GetSmallSprite(), healAmount, 0f, 0.055f, "Small Health Pack Pickup");
        }

        public static HealthPackPickup DropLarge(Vector3 position, float maxHealthRatio)
        {
            return Drop(position, GetLargeSprite(), 0, maxHealthRatio, 0.07f, "Large Health Pack Pickup");
        }

        private static HealthPackPickup Drop(
            Vector3 position,
            Sprite sprite,
            int flatHealAmount,
            float maxHealthHealRatio,
            float visualScale,
            string objectName)
        {
            GameObject pickupObject = new GameObject(objectName);
            pickupObject.transform.position = position;
            pickupObject.transform.localScale = Vector3.one * visualScale;

            SpriteRenderer spriteRenderer = pickupObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = 780;

            CircleCollider2D collider = pickupObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 4f;

            HealthPackPickup pickup = pickupObject.AddComponent<HealthPackPickup>();
            pickup.flatHealAmount = Mathf.Max(0, flatHealAmount);
            pickup.maxHealthHealRatio = Mathf.Clamp01(maxHealthHealRatio);
            return pickup;
        }

        private void Awake()
        {
            Collider2D pickupCollider = GetComponent<Collider2D>();
            pickupCollider.isTrigger = true;

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer.sprite == null)
                spriteRenderer.sprite = GetSmallSprite();

            startPosition = transform.position;
            bobOffset = Random.Range(0f, Mathf.PI * 2f);
        }

        private void OnValidate()
        {
            flatHealAmount = Mathf.Max(0, flatHealAmount);
            maxHealthHealRatio = Mathf.Clamp01(maxHealthHealRatio);
            lifetime = Mathf.Max(1f, lifetime);
        }

        private void Update()
        {
            lifetime -= Time.deltaTime;
            if (lifetime <= 0f)
            {
                Destroy(gameObject);
                return;
            }

            float bob = Mathf.Sin(Time.time * 4f + bobOffset) * 0.035f;
            transform.position = startPosition + Vector3.up * bob;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

            if (playerHealth == null || playerHealth.IsDead)
                return;

            int ratioHealAmount = maxHealthHealRatio <= 0f
                ? 0
                : Mathf.CeilToInt(playerHealth.MaxHealth * maxHealthHealRatio);
            int healAmount = Mathf.Max(flatHealAmount, ratioHealAmount);

            playerHealth.Heal(healAmount);
            Destroy(gameObject);
        }

        private static Sprite GetSmallSprite()
        {
            Sprite sprite = Resources.Load<Sprite>(SmallHealthPackPath);
            return sprite != null ? sprite : GetGeneratedSmallSprite();
        }

        private static Sprite GetLargeSprite()
        {
            Sprite sprite = Resources.Load<Sprite>(LargeHealthPackPath);
            return sprite != null ? sprite : GetGeneratedLargeSprite();
        }

        private static Sprite GetGeneratedSmallSprite()
        {
            if (generatedSmallSprite != null)
                return generatedSmallSprite;

            generatedSmallSprite = CreateGeneratedSprite(
                new Color(0.72f, 0.03f, 0.05f, 1f),
                new Color(0.92f, 0.05f, 0.08f, 1f),
                "GeneratedSmallHealthPack");
            return generatedSmallSprite;
        }

        private static Sprite GetGeneratedLargeSprite()
        {
            if (generatedLargeSprite != null)
                return generatedLargeSprite;

            generatedLargeSprite = CreateGeneratedSprite(
                new Color(0.9f, 0.1f, 0.04f, 1f),
                new Color(1f, 0.67f, 0.08f, 1f),
                "GeneratedLargeHealthPack");
            return generatedLargeSprite;
        }

        private static Sprite CreateGeneratedSprite(Color fill, Color border, string spriteName)
        {
            Texture2D texture = new Texture2D(TextureSize, TextureSize, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            Color clear = new Color(0f, 0f, 0f, 0f);
            Color shadow = new Color(0.18f, 0.02f, 0.02f, 0.8f);
            Color highlight = new Color(1f, 0.86f, 0.86f, 1f);

            for (int y = 0; y < TextureSize; y++)
            {
                for (int x = 0; x < TextureSize; x++)
                {
                    Color color = clear;
                    bool inBox = x >= 5 && x <= 26 && y >= 5 && y <= 26;
                    bool inShadow = x >= 6 && x <= 28 && y >= 3 && y <= 25;
                    bool inCross = (x >= 13 && x <= 18 && y >= 8 && y <= 23)
                        || (x >= 8 && x <= 23 && y >= 13 && y <= 18);

                    if (inShadow)
                        color = shadow;

                    if (inBox)
                        color = fill;

                    if (inCross)
                        color = highlight;

                    if ((x == 6 || x == 25) && y >= 6 && y <= 25 || (y == 6 || y == 25) && x >= 6 && x <= 25)
                        color = border;

                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, TextureSize, TextureSize), new Vector2(0.5f, 0.5f), 32f);
            sprite.name = spriteName;
            return sprite;
        }
    }
}
