using UnityEngine;
using VampireLike.UI;

namespace VampireLike.Combat
{
    /// <summary>
    /// Draws a compact player HP bar on the combat HUD.
    /// </summary>
    public class PlayerHealthUI : MonoBehaviour
    {
        [SerializeField]
        private PlayerHealth playerHealth;

        [SerializeField]
        private float topMargin = 40f;

        [SerializeField]
        private float sideMargin = 72f;

        [SerializeField]
        private float width = 220f;

        [SerializeField]
        private float height = 18f;

        [SerializeField]
        private bool drawHud = true;

        private Texture2D whiteTexture;
        private GUIStyle labelStyle;

        private void Awake()
        {
            if (playerHealth == null)
                playerHealth = GetComponent<PlayerHealth>();

            EnsureTexture();
        }

        private void OnDestroy()
        {
            if (whiteTexture != null)
                Destroy(whiteTexture);
        }

        private void OnValidate()
        {
            topMargin = Mathf.Max(0f, topMargin);
            sideMargin = Mathf.Max(0f, sideMargin);
            width = Mathf.Max(120f, width);
            height = Mathf.Max(10f, height);
        }

        private void OnGUI()
        {
            if (!drawHud || GameState.IsGameOver)
                return;

            if (playerHealth == null)
                playerHealth = GetComponent<PlayerHealth>();

            if (playerHealth == null)
                return;

            EnsureTexture();
            EnsureStyles();

            GUI.depth = -999;
            DrawHealthBar();
        }

        private void DrawHealthBar()
        {
            bool isPortrait = Screen.height > Screen.width;
            float currentWidth = isPortrait ? 150f : width;
            float currentHeight = isPortrait ? 14f : height;
            float left = MobileSafeArea.HudLeft(isPortrait ? 36f : sideMargin);
            float top = MobileSafeArea.HudTop(isPortrait ? 34f : topMargin);
            labelStyle.fontSize = isPortrait ? 12 : 14;

            Rect borderRect = new Rect(left, top, currentWidth, currentHeight + 6f);
            Rect backgroundRect = new Rect(borderRect.x + 3f, borderRect.y + 3f, borderRect.width - 6f, borderRect.height - 6f);
            Rect fillRect = new Rect(backgroundRect.x, backgroundRect.y, backgroundRect.width * playerHealth.HealthProgress, backgroundRect.height);

            Color previousColor = GUI.color;

            GUI.color = new Color(0.03f, 0.025f, 0.02f, 0.78f);
            GUI.DrawTexture(borderRect, whiteTexture);

            GUI.color = new Color(0.01f, 0.01f, 0.01f, 0.8f);
            GUI.DrawTexture(backgroundRect, whiteTexture);

            GUI.color = GetHealthColor(playerHealth.HealthProgress);
            GUI.DrawTexture(fillRect, whiteTexture);

            GUI.color = Color.white;
            GUI.Label(borderRect, $"HP {playerHealth.CurrentHealth} / {playerHealth.MaxHealth}", labelStyle);

            GUI.color = previousColor;
        }

        private static Color GetHealthColor(float progress)
        {
            if (progress <= 0.25f)
                return new Color(0.95f, 0.12f, 0.08f, 0.96f);

            if (progress <= 0.5f)
                return new Color(0.95f, 0.62f, 0.12f, 0.96f);

            return new Color(0.78f, 0.1f, 0.12f, 0.96f);
        }

        private void EnsureTexture()
        {
            if (whiteTexture != null)
                return;

            whiteTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            whiteTexture.SetPixel(0, 0, Color.white);
            whiteTexture.Apply();
        }

        private void EnsureStyles()
        {
            if (labelStyle != null)
                return;

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Screen.height > Screen.width ? 12 : 14,
                fontStyle = FontStyle.Bold
            };
            labelStyle.normal.textColor = Color.white;
        }
    }
}
