using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스가 살아 있는 동안 화면 상단 중앙에 보스 체력바를 표시한다.
    /// </summary>
    public class BossHealthBarUI : MonoBehaviour
    {
        [SerializeField]
        private BossSpawner bossSpawner;

        [SerializeField]
        private bool drawHud = true;

        [SerializeField]
        private float topOffset = 38f;

        [SerializeField]
        private float sideMargin = 220f;

        [SerializeField]
        private float barHeight = 18f;

        private Texture2D whiteTexture;
        private GUIStyle labelStyle;

        private void Awake()
        {
            if (bossSpawner == null)
                bossSpawner = GetComponent<BossSpawner>();

            EnsureTexture();
        }

        private void OnDestroy()
        {
            if (whiteTexture != null)
                Destroy(whiteTexture);
        }

        private void OnValidate()
        {
            topOffset = Mathf.Max(0f, topOffset);
            sideMargin = Mathf.Max(0f, sideMargin);
            barHeight = Mathf.Max(8f, barHeight);
        }

        private void OnGUI()
        {
            if (!drawHud)
                return;

            if (bossSpawner == null)
                bossSpawner = GetComponent<BossSpawner>();

            if (bossSpawner == null || !bossSpawner.HasActiveBoss)
                return;

            EnemyHealth bossHealth = bossSpawner.ActiveBossHealth;

            if (bossHealth == null)
                return;

            EnsureTexture();
            EnsureStyles();

            GUI.depth = -1200;
            DrawBossHealthBar(bossHealth);
        }

        private void DrawBossHealthBar(EnemyHealth bossHealth)
        {
            float width = Mathf.Max(260f, Screen.width - sideMargin * 2f);
            float x = (Screen.width - width) * 0.5f;

            Rect borderRect = new Rect(x, topOffset, width, barHeight + 8f);
            Rect backgroundRect = new Rect(borderRect.x + 4f, borderRect.y + 4f, borderRect.width - 8f, borderRect.height - 8f);
            Rect fillRect = new Rect(backgroundRect.x, backgroundRect.y, backgroundRect.width * bossHealth.HealthProgress, backgroundRect.height);

            Color previousColor = GUI.color;

            GUI.color = new Color(0.03f, 0.02f, 0.02f, 0.82f);
            GUI.DrawTexture(borderRect, whiteTexture);

            GUI.color = new Color(0.01f, 0.01f, 0.01f, 0.86f);
            GUI.DrawTexture(backgroundRect, whiteTexture);

            GUI.color = new Color(0.85f, 0.08f, 0.08f, 0.95f);
            GUI.DrawTexture(fillRect, whiteTexture);

            GUI.color = Color.white;
            GUI.Label(borderRect, "BOSS", labelStyle);

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
            if (labelStyle != null)
                return;

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
            labelStyle.normal.textColor = Color.white;
        }
    }
}
