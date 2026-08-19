using UnityEngine;
using VampireLike.Enemies;
using VampireLike.UI;

namespace VampireLike.Combat
{
    /// <summary>
    /// 전투 화면 상단에 플레이어 체력을 작고 선명한 HUD 카드로 표시한다.
    /// </summary>
    public class PlayerHealthUI : MonoBehaviour
    {
        [SerializeField]
        private PlayerHealth playerHealth;

        [SerializeField]
        private float topMargin = 42f;

        [SerializeField]
        private float bossFightTopMargin = 42f;

        [SerializeField]
        private float portraitBossFightTopMargin = 50f;

        [SerializeField]
        private float sideMargin = 72f;

        [SerializeField]
        private float width = 250f;

        [SerializeField]
        private float height = 22f;

        [SerializeField]
        private bool drawHud = true;

        [SerializeField]
        private float damagePulseDuration = 0.35f;

        [SerializeField]
        private float delayedBarDrainSpeed = 1.8f;

        [SerializeField]
        private float lowHealthWarningRatio = 0.28f;

        private Texture2D whiteTexture;
        private GUIStyle labelStyle;
        private BossSpawner bossSpawner;
        private HiddenBossSpawner hiddenBossSpawner;
        private int lastHealth = -1;
        private float delayedHealthProgress = 1f;
        private float damagePulseEndTime;

        private void Awake()
        {
            if (playerHealth == null)
                playerHealth = GetComponent<PlayerHealth>();

            bossSpawner = FindFirstObjectByType<BossSpawner>();
            hiddenBossSpawner = FindFirstObjectByType<HiddenBossSpawner>();
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
            bossFightTopMargin = Mathf.Max(0f, bossFightTopMargin);
            portraitBossFightTopMargin = Mathf.Max(40f, portraitBossFightTopMargin);
            sideMargin = Mathf.Max(0f, sideMargin);
            width = Mathf.Max(150f, width);
            height = Mathf.Max(14f, height);
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
            UpdateHealthFeedbackState();

            GUI.depth = -999;
            DrawDamageFeedback();
            DrawHealthBar();
        }

        private void DrawHealthBar()
        {
            bool isPortrait = Screen.height > Screen.width;
            bool hasActiveBoss = HasActiveBoss();
            float currentWidth = isPortrait ? Mathf.Min(260f, Screen.width * 0.42f) : width;
            float currentHeight = isPortrait ? 20f : height;
            float left = MobileSafeArea.HudLeft(isPortrait ? 24f : sideMargin);
            float baseTop = isPortrait
                ? (hasActiveBoss ? portraitBossFightTopMargin : 50f)
                : (hasActiveBoss ? bossFightTopMargin : topMargin);
            float top = MobileSafeArea.HudTop(baseTop);
            labelStyle.fontSize = isPortrait ? 13 : 14;

            Rect panelRect = new Rect(left, top, currentWidth, currentHeight + 10f);
            Rect labelRect = new Rect(panelRect.x + 8f, panelRect.y + 4f, isPortrait ? 38f : 44f, currentHeight + 2f);
            Rect backgroundRect = new Rect(labelRect.xMax + 6f, panelRect.y + 6f, panelRect.width - labelRect.width - 22f, currentHeight - 2f);
            Rect fillRect = new Rect(backgroundRect.x, backgroundRect.y, backgroundRect.width * playerHealth.HealthProgress, backgroundRect.height);
            Rect delayedRect = new Rect(backgroundRect.x, backgroundRect.y, backgroundRect.width * delayedHealthProgress, backgroundRect.height);
            float pulse = GetDamagePulse();

            Color previousColor = GUI.color;

            GUI.color = new Color(0.015f, 0.012f, 0.01f, 0.86f);
            GUI.DrawTexture(panelRect, whiteTexture);

            GUI.color = new Color(0.08f, 0.02f, 0.025f, 0.9f);
            GUI.DrawTexture(backgroundRect, whiteTexture);

            if (delayedHealthProgress > playerHealth.HealthProgress)
            {
                GUI.color = new Color(0.95f, 0.72f, 0.18f, 0.55f);
                GUI.DrawTexture(delayedRect, whiteTexture);
            }

            GUI.color = Color.Lerp(GetHealthColor(playerHealth.HealthProgress), Color.white, pulse * 0.42f);
            GUI.DrawTexture(fillRect, whiteTexture);

            GUI.color = new Color(1f, 1f, 1f, 0.18f);
            GUI.DrawTexture(new Rect(backgroundRect.x, backgroundRect.y, backgroundRect.width, Mathf.Max(2f, backgroundRect.height * 0.28f)), whiteTexture);

            GUI.color = new Color(0.9f, 0.08f, 0.08f, 0.86f);
            GUI.DrawTexture(labelRect, whiteTexture);

            if (playerHealth.HealthProgress <= lowHealthWarningRatio)
            {
                float lowPulse = 0.5f + Mathf.Sin(Time.unscaledTime * 8f) * 0.5f;
                GUI.color = new Color(1f, 0.08f, 0.04f, 0.16f + lowPulse * 0.16f);
                GUI.DrawTexture(panelRect, whiteTexture);
            }

            GUI.color = Color.white;
            GUI.Label(labelRect, "HP", labelStyle);
            GUI.Label(backgroundRect, $"{playerHealth.CurrentHealth} / {playerHealth.MaxHealth}", labelStyle);

            GUI.color = previousColor;
        }

        private void UpdateHealthFeedbackState()
        {
            if (lastHealth < 0)
            {
                lastHealth = playerHealth.CurrentHealth;
                delayedHealthProgress = playerHealth.HealthProgress;
                return;
            }

            if (playerHealth.CurrentHealth < lastHealth)
                damagePulseEndTime = Time.unscaledTime + damagePulseDuration;

            lastHealth = playerHealth.CurrentHealth;

            float targetProgress = playerHealth.HealthProgress;

            if (delayedHealthProgress < targetProgress)
                delayedHealthProgress = targetProgress;
            else
                delayedHealthProgress = Mathf.MoveTowards(delayedHealthProgress, targetProgress, Time.unscaledDeltaTime * delayedBarDrainSpeed);
        }

        private void DrawDamageFeedback()
        {
            float pulse = GetDamagePulse();
            bool lowHealth = playerHealth.HealthProgress <= lowHealthWarningRatio;

            if (pulse <= 0f && !lowHealth)
                return;

            Color previousColor = GUI.color;
            float edgeAlpha = pulse * 0.16f;

            if (lowHealth)
                edgeAlpha = Mathf.Max(edgeAlpha, 0.035f + (0.5f + Mathf.Sin(Time.unscaledTime * 5.5f) * 0.5f) * 0.035f);

            GUI.color = new Color(0.9f, 0.03f, 0.02f, edgeAlpha);
            float edgeWidth = Mathf.Max(18f, Mathf.Min(Screen.width, Screen.height) * 0.035f);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, edgeWidth), whiteTexture);
            GUI.DrawTexture(new Rect(0f, Screen.height - edgeWidth, Screen.width, edgeWidth), whiteTexture);
            GUI.DrawTexture(new Rect(0f, 0f, edgeWidth, Screen.height), whiteTexture);
            GUI.DrawTexture(new Rect(Screen.width - edgeWidth, 0f, edgeWidth, Screen.height), whiteTexture);

            GUI.color = previousColor;
        }

        private float GetDamagePulse()
        {
            if (damagePulseDuration <= 0f || Time.unscaledTime >= damagePulseEndTime)
                return 0f;

            float remaining = damagePulseEndTime - Time.unscaledTime;
            return Mathf.Clamp01(remaining / damagePulseDuration);
        }

        private bool HasActiveBoss()
        {
            if (bossSpawner == null)
                bossSpawner = FindFirstObjectByType<BossSpawner>();

            if (hiddenBossSpawner == null)
                hiddenBossSpawner = FindFirstObjectByType<HiddenBossSpawner>();

            return (bossSpawner != null && bossSpawner.HasActiveBoss)
                || (hiddenBossSpawner != null && hiddenBossSpawner.HasActiveHiddenBoss);
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
