using UnityEngine;
using VampireLike.Enemies;

namespace VampireLike.Growth
{
    /// <summary>
    /// 화면 최상단에 경험치 바, 현재 웨이브, 현재 레벨을 표시한다.
    /// </summary>
    public class PlayerExperienceUI : MonoBehaviour
    {
        [SerializeField]
        private PlayerExperience playerExperience;

        [SerializeField]
        private EnemySpawner enemySpawner;

        [SerializeField]
        private float topMargin = 8f;

        [SerializeField]
        private float sideMargin = 72f;

        [SerializeField]
        private float barHeight = 18f;

        [SerializeField]
        private bool drawHud = true;

        private Texture2D whiteTexture;
        private GUIStyle hudTextStyle;

        private void Awake()
        {
            if (playerExperience == null)
                playerExperience = GetComponent<PlayerExperience>();

            if (enemySpawner == null)
                enemySpawner = FindFirstObjectByType<EnemySpawner>();

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

            if (enemySpawner == null)
                enemySpawner = FindFirstObjectByType<EnemySpawner>();

            if (playerExperience == null)
                return;

            EnsureTexture();
            EnsureStyles();

            GUI.depth = -1000;
            DrawHud();
        }

        private void DrawHud()
        {
            const float waveWidth = 112f;
            const float levelWidth = 92f;
            const float gap = 8f;

            float barWidth = Mathf.Max(240f, Screen.width - sideMargin * 2f - waveWidth - levelWidth - gap * 2f);
            Rect waveRect = new Rect(sideMargin, topMargin - 1f, waveWidth, barHeight + 8f);
            Rect borderRect = new Rect(waveRect.xMax + gap, topMargin, barWidth, barHeight + 6f);
            Rect backgroundRect = new Rect(borderRect.x + 3f, borderRect.y + 3f, borderRect.width - 6f, borderRect.height - 6f);
            Rect fillRect = new Rect(backgroundRect.x, backgroundRect.y, backgroundRect.width * playerExperience.ExperienceProgress, backgroundRect.height);
            Rect levelRect = new Rect(borderRect.xMax + gap, borderRect.y - 1f, levelWidth, borderRect.height + 2f);

            Color previousColor = GUI.color;

            GUI.color = new Color(0.03f, 0.025f, 0.02f, 0.78f);
            GUI.DrawTexture(waveRect, whiteTexture);
            GUI.DrawTexture(borderRect, whiteTexture);
            GUI.DrawTexture(levelRect, whiteTexture);

            GUI.color = new Color(0.01f, 0.012f, 0.012f, 0.78f);
            GUI.DrawTexture(backgroundRect, whiteTexture);

            GUI.color = new Color(0.16f, 0.45f, 0.95f, 0.96f);
            GUI.DrawTexture(fillRect, whiteTexture);

            GUI.color = Color.white;
            GUI.Label(waveRect, enemySpawner == null ? "WAVE -" : $"WAVE {enemySpawner.CurrentWave}", hudTextStyle);
            GUI.Label(levelRect, $"레벨 {playerExperience.CurrentLevel}", hudTextStyle);

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
            if (hudTextStyle != null)
                return;

            hudTextStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16,
                fontStyle = FontStyle.Bold
            };
            hudTextStyle.normal.textColor = Color.white;
        }
    }
}
