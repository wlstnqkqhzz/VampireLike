using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VampireLike.Enemies;
using VampireLike.Growth;

namespace VampireLike.Combat
{
    /// <summary>
    /// Shows the game over result panel when the player dies.
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        private const string CanvasName = "Game Over Canvas";
        private const string RootName = "Game Over";
        private const string PanelName = "Game Over Panel";
        private const string TitleName = "Game Over Title";
        private const string SurvivalTimeName = "Survival Time";
        private const string WaveResultName = "Wave Result";
        private const string LevelResultName = "Level Result";
        private const string ExperienceResultName = "Experience Result";
        private const string KillCountName = "Kill Count";
        private const string BossKillCountName = "Boss Kill Count";
        private const string UpgradeResultName = "Upgrade Result";

        [SerializeField]
        private GameObject gameOverRoot;

        [SerializeField]
        private Button restartButton;

        [SerializeField]
        private Button quitButton;

        private RectTransform gameOverPanel;
        private Text titleText;
        private Text survivalTimeText;
        private Text waveText;
        private Text levelText;
        private Text experienceText;
        private Text killCountText;
        private Text bossKillCountText;
        private Text upgradeText;
        private bool isShowing;

        private void Awake()
        {
            EnsureUI();
            EnsureEventSystem();
            Hide();
        }

        private void Update()
        {
            if (!isShowing && GameState.IsGameOver)
                Show();
        }

        private void OnDestroy()
        {
            if (restartButton != null)
                restartButton.onClick.RemoveListener(RestartGame);

            if (quitButton != null)
                quitButton.onClick.RemoveListener(QuitGame);
        }

        public void Show()
        {
            isShowing = true;
            Time.timeScale = 0f;
            CenterGameOverPanel();
            UpdateResultTexts();

            if (gameOverRoot != null)
                gameOverRoot.SetActive(true);
        }

        private void Hide()
        {
            isShowing = false;

            if (gameOverRoot != null)
                gameOverRoot.SetActive(false);
        }

        private void RestartGame()
        {
            Time.timeScale = 1f;
            GameState.ResetGame();
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
        }

        private void QuitGame()
        {
            Time.timeScale = 1f;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void EnsureUI()
        {
            if (gameOverRoot != null)
            {
                CacheGameOverPanel();
                CenterGameOverPanel();
                EnsureResultLabels();
                BindButtons();
                return;
            }

            Canvas canvas = CreateCanvas();

            GameObject root = new GameObject(RootName);
            root.transform.SetParent(canvas.transform, false);
            gameOverRoot = root;

            RectTransform rootRect = root.AddComponent<RectTransform>();
            StretchToParent(rootRect);

            Image backdrop = root.AddComponent<Image>();
            backdrop.color = new Color(0f, 0f, 0f, 0.62f);

            GameObject panel = new GameObject(PanelName);
            panel.transform.SetParent(root.transform, false);
            gameOverPanel = panel.AddComponent<RectTransform>();
            CenterPanel(gameOverPanel);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0.08f, 0.08f, 0.09f, 0.96f);

            EnsureResultLabels();
            restartButton = CreateButton(panel.transform, "다시 시작", new Vector2(0f, -158f));
            quitButton = CreateButton(panel.transform, "게임 종료", new Vector2(0f, -216f));
            BindButtons();
        }

        private void BindButtons()
        {
            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(RestartGame);
                restartButton.onClick.AddListener(RestartGame);
            }

            if (quitButton != null)
            {
                quitButton.onClick.RemoveListener(QuitGame);
                quitButton.onClick.AddListener(QuitGame);
            }
        }

        private static Canvas CreateCanvas()
        {
            GameObject canvasObject = new GameObject(CanvasName);
            int uiLayer = LayerMask.NameToLayer("UI");

            if (uiLayer >= 0)
                canvasObject.layer = uiLayer;

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1500;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null)
                return;

            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }

        private void CacheGameOverPanel()
        {
            Transform panelTransform = gameOverRoot.transform.Find(PanelName);

            if (panelTransform == null)
                return;

            gameOverPanel = panelTransform.GetComponent<RectTransform>();
            titleText = panelTransform.Find(TitleName)?.GetComponent<Text>();
            survivalTimeText = panelTransform.Find(SurvivalTimeName)?.GetComponent<Text>();
            waveText = panelTransform.Find(WaveResultName)?.GetComponent<Text>();
            levelText = panelTransform.Find(LevelResultName)?.GetComponent<Text>();
            experienceText = panelTransform.Find(ExperienceResultName)?.GetComponent<Text>();
            killCountText = panelTransform.Find(KillCountName)?.GetComponent<Text>();
            bossKillCountText = panelTransform.Find(BossKillCountName)?.GetComponent<Text>();
            upgradeText = panelTransform.Find(UpgradeResultName)?.GetComponent<Text>();
        }

        private void EnsureResultLabels()
        {
            if (gameOverPanel == null)
                return;

            if (titleText == null)
                titleText = CreateLabel(gameOverPanel, "게임 오버", new Vector2(0f, 220f), 36, Color.white, new Vector2(420f, 56f), TitleName);

            if (survivalTimeText == null)
                survivalTimeText = CreateLabel(gameOverPanel, "생존 시간 00:00", new Vector2(0f, 164f), 20, new Color(0.9f, 0.95f, 0.88f, 1f), new Vector2(420f, 28f), SurvivalTimeName);

            if (waveText == null)
                waveText = CreateLabel(gameOverPanel, "도달 웨이브 -", new Vector2(0f, 124f), 18, new Color(0.82f, 0.9f, 0.78f, 1f), new Vector2(420f, 26f), WaveResultName);

            if (levelText == null)
                levelText = CreateLabel(gameOverPanel, "레벨 -", new Vector2(0f, 92f), 18, new Color(0.82f, 0.9f, 0.78f, 1f), new Vector2(420f, 26f), LevelResultName);

            if (experienceText == null)
                experienceText = CreateLabel(gameOverPanel, "총 경험치 0", new Vector2(0f, 60f), 18, new Color(0.82f, 0.9f, 0.78f, 1f), new Vector2(420f, 26f), ExperienceResultName);

            if (killCountText == null)
                killCountText = CreateLabel(gameOverPanel, "일반 적 처치 0", new Vector2(0f, 28f), 18, new Color(0.82f, 0.9f, 0.78f, 1f), new Vector2(420f, 26f), KillCountName);

            if (bossKillCountText == null)
                bossKillCountText = CreateLabel(gameOverPanel, "보스 처치 0", new Vector2(0f, -4f), 18, new Color(0.82f, 0.9f, 0.78f, 1f), new Vector2(420f, 26f), BossKillCountName);

            if (upgradeText == null)
                upgradeText = CreateLabel(gameOverPanel, "선택한 강화 없음", new Vector2(0f, -66f), 16, new Color(0.78f, 0.86f, 0.74f, 1f), new Vector2(420f, 72f), UpgradeResultName);
            PositionResultObjects();
        }

        private void CenterGameOverPanel()
        {
            if (gameOverRoot == null)
                return;

            RectTransform rootRect = gameOverRoot.GetComponent<RectTransform>();

            if (rootRect != null)
                StretchToParent(rootRect);

            if (gameOverPanel != null)
            {
                CenterPanel(gameOverPanel);
                PositionResultObjects();
            }
        }

        private void UpdateResultTexts()
        {
            PlayerExperience playerExperience = GetComponent<PlayerExperience>();
            EnemySpawner enemySpawner = FindFirstObjectByType<EnemySpawner>();

            if (titleText != null)
                titleText.text = "게임 오버";

            if (survivalTimeText != null)
                survivalTimeText.text = $"생존 시간 {FormatTime(GameSessionStats.SurvivalTime)}";

            if (waveText != null)
                waveText.text = enemySpawner == null ? "도달 웨이브 -" : $"도달 웨이브 {enemySpawner.CurrentWave}";

            if (levelText != null)
                levelText.text = playerExperience == null ? "레벨 -" : $"레벨 {playerExperience.CurrentLevel}";

            if (experienceText != null)
                experienceText.text = $"총 경험치 {GameSessionStats.TotalExperienceGained}";

            if (killCountText != null)
                killCountText.text = $"일반 적 처치 {GameSessionStats.EnemyKillCount}";

            if (bossKillCountText != null)
                bossKillCountText.text = $"보스 처치 {GameSessionStats.BossKillCount}";

            if (upgradeText != null)
                upgradeText.text = $"선택 강화\n{GameSessionStats.GetUpgradeSummary()}";
        }

        private static string FormatTime(float seconds)
        {
            int totalSeconds = Mathf.FloorToInt(seconds);
            int minutes = totalSeconds / 60;
            int remainingSeconds = totalSeconds % 60;
            return $"{minutes:00}:{remainingSeconds:00}";
        }

        private static void StretchToParent(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
        }

        private static void CenterPanel(RectTransform rectTransform)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(540f, 560f);
            rectTransform.anchoredPosition = Vector2.zero;
        }

        private void PositionResultObjects()
        {
            SetLabelPosition(titleText, new Vector2(0f, 220f), new Vector2(420f, 56f));
            SetLabelPosition(survivalTimeText, new Vector2(0f, 164f), new Vector2(420f, 28f));
            SetLabelPosition(waveText, new Vector2(0f, 124f), new Vector2(420f, 26f));
            SetLabelPosition(levelText, new Vector2(0f, 92f), new Vector2(420f, 26f));
            SetLabelPosition(experienceText, new Vector2(0f, 60f), new Vector2(420f, 26f));
            SetLabelPosition(killCountText, new Vector2(0f, 28f), new Vector2(420f, 26f));
            SetLabelPosition(bossKillCountText, new Vector2(0f, -4f), new Vector2(420f, 26f));
            SetLabelPosition(upgradeText, new Vector2(0f, -66f), new Vector2(420f, 72f));

            if (restartButton != null)
                SetRectPosition(restartButton.GetComponent<RectTransform>(), new Vector2(0f, -158f), new Vector2(250f, 44f));

            if (quitButton != null)
                SetRectPosition(quitButton.GetComponent<RectTransform>(), new Vector2(0f, -216f), new Vector2(250f, 44f));
        }

        private static void SetLabelPosition(Text label, Vector2 position, Vector2 size)
        {
            if (label == null)
                return;

            SetRectPosition(label.GetComponent<RectTransform>(), position, size);
        }

        private static void SetRectPosition(RectTransform rectTransform, Vector2 position, Vector2 size)
        {
            if (rectTransform == null)
                return;

            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = position;
        }

        private static Text CreateLabel(Transform parent, string text, Vector2 position, int fontSize, Color color, Vector2 size, string objectName = null)
        {
            GameObject labelObject = new GameObject(string.IsNullOrEmpty(objectName) ? text : objectName);
            labelObject.transform.SetParent(parent, false);

            RectTransform rectTransform = labelObject.AddComponent<RectTransform>();
            SetRectPosition(rectTransform, position, size);

            Text label = labelObject.AddComponent<Text>();
            label.text = text;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = fontSize;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = color;
            label.raycastTarget = false;
            return label;
        }

        private static Button CreateButton(Transform parent, string text, Vector2 position)
        {
            GameObject buttonObject = new GameObject(text);
            buttonObject.transform.SetParent(parent, false);

            RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
            SetRectPosition(rectTransform, position, new Vector2(250f, 44f));

            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.82f, 0.9f, 0.76f, 1f);

            Button button = buttonObject.AddComponent<Button>();
            CreateLabel(buttonObject.transform, text, Vector2.zero, 18, new Color(0.06f, 0.09f, 0.06f, 1f), new Vector2(230f, 40f));
            return button;
        }
    }
}
