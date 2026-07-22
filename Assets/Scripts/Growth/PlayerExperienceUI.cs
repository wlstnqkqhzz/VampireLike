using UnityEngine;

namespace VampireLike.Growth
{
    public class PlayerExperienceUI : MonoBehaviour
    {
        [SerializeField]
        private PlayerExperience playerExperience;

        [SerializeField]
        private float topMargin = 8f;

        [SerializeField]
        private float sideMargin = 72f;

        [SerializeField]
        private float barHeight = 18f;

        [SerializeField]
        private bool drawHud = true;

        private Texture2D whiteTexture;
        private GUIStyle levelStyle;

        private void Awake()
        {
            if (playerExperience == null)
                playerExperience = GetComponent<PlayerExperience>();

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
            barHeight = Mathf.Max(8f, barHeight);
        }

        private void OnGUI()
        {
            if (!drawHud)
                return;

            if (playerExperience == null)
                playerExperience = GetComponent<PlayerExperience>();

            if (playerExperience == null)
                return;

            EnsureTexture();
            EnsureStyles();

            GUI.depth = -1000;
            DrawHud();
        }

        private void DrawHud()
        {
            const float levelWidth = 92f;
            const float levelGap = 8f;
            float barWidth = Mathf.Max(240f, Screen.width - sideMargin * 2f - levelWidth - levelGap);

            Rect borderRect = new Rect(sideMargin, topMargin, barWidth, barHeight + 6f);
            Rect backgroundRect = new Rect(borderRect.x + 3f, borderRect.y + 3f, borderRect.width - 6f, borderRect.height - 6f);
            Rect fillRect = new Rect(backgroundRect.x, backgroundRect.y, backgroundRect.width * playerExperience.ExperienceProgress, backgroundRect.height);
            Rect levelRect = new Rect(borderRect.xMax + levelGap, borderRect.y - 1f, levelWidth, borderRect.height + 2f);

            Color previousColor = GUI.color;

            GUI.color = new Color(0.03f, 0.025f, 0.02f, 0.78f);
            GUI.DrawTexture(borderRect, whiteTexture);
            GUI.DrawTexture(levelRect, whiteTexture);

            GUI.color = new Color(0.01f, 0.012f, 0.012f, 0.78f);
            GUI.DrawTexture(backgroundRect, whiteTexture);

            GUI.color = new Color(0.16f, 0.45f, 0.95f, 0.96f);
            GUI.DrawTexture(fillRect, whiteTexture);

            GUI.color = Color.white;
            GUI.Label(levelRect, $"레벨 {playerExperience.CurrentLevel}", levelStyle);

            GUI.color = previousColor;
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
            if (levelStyle != null)
                return;

            levelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16,
                fontStyle = FontStyle.Bold
            };
            levelStyle.normal.textColor = Color.white;
        }
    }
}
