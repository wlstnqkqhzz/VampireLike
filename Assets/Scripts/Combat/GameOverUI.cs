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
    /// Shows the game over panel and run result when the player dies.
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        private const string CanvasName = "Game Over Canvas";
        private const string RootName = "Game Over";
        private const string PanelName = "Game Over Panel";
        private const string SurvivalTimeName = "Survival Time";
        private const string WaveResultName = "Wave Result";
        private const string LevelResultName = "Level Result";
        private const string KillCountName = "Kill Count";

        [SerializeField]
        private GameObject gameOverRoot;

        [SerializeField]
        private Button restartButton;

        [SerializeField]
        private Button quitButton;

        private RectTransform gameOverPanel;
        private Text survivalTimeText;
        private Text waveText;
        private Text levelText;
        private Text killCountText;
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

            CreateLabel(panel.transform, "게임 오버", new Vector2(0f, 138f), 36, Color.white, new Vector2(340f, 56f));
            survivalTimeText = CreateLabel(panel.transform, "생존 시간 00:00", new Vector2(0f, 82f), 20, new Color(0.9f, 0.95f, 0.88f, 1f), new Vector2(340f, 28f), SurvivalTimeName);
            waveText = CreateLabel(panel.transform, "도달 웨이브 -", new Vector2(0f, 48f), 18, new Color(0.82f, 0.9f, 0.78f, 1f), new Vector2(340f, 26f), WaveResultName);
            levelText = CreateLabel(panel.transform, "레벨 -", new Vector2(0f, 18f), 18, new Color(0.82f, 0.9f, 0.78f, 1f), new Vector2(340f, 26f), LevelResultName);
            killCountText = CreateLabel(panel.transform, "처치 수 0", new Vector2(0f, -12f), 18, new Color(0.82f, 0.9f, 0.78f, 1f), new Vector2(340f, 26f), KillCountName);
            restartButton = CreateButton(panel.transform, "다시 시작", new Vector2(0f, -72f));
            quitButton = CreateButton(panel.transform, "게임 종료", new Vector2(0f, -130f));
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
            survivalTimeText = panelTransform.Find(SurvivalTimeName)?.GetComponent<Text>();
            waveText = panelTransform.Find(WaveResultName)?.GetComponent<Text>();
            levelText = panelTransform.Find(LevelResultName)?.GetComponent<Text>();
            killCountText = panelTransform.Find(KillCountName)?.GetComponent<Text>();
        }

        private void CenterGameOverPanel()
        {
            if (gameOverRoot == null)
                return;

            RectTransform rootRect = gameOverRoot.GetComponent<RectTransform>();

            if (rootRect != null)
                StretchToParent(rootRect);

            if (gameOverPanel != null)
                CenterPanel(gameOverPanel);
        }

        private void UpdateResultTexts()
        {
            PlayerExperience playerExperience = GetComponent<PlayerExperience>();
            EnemySpawner enemySpawner = FindFirstObjectByType<EnemySpawner>();

            if (survivalTimeText != null)
                survivalTimeText.text = $"생존 시간 {FormatTime(GameSessionStats.SurvivalTime)}";

            if (waveText != null)
                waveText.text = enemySpawner == null ? "도달 웨이브 -" : $"도달 웨이브 {enemySpawner.CurrentWave}";

            if (levelText != null)
                levelText.text = playerExperience == null ? "레벨 -" : $"레벨 {playerExperience.CurrentLevel}";

            if (killCountText != null)
                killCountText.text = $"처치 수 {GameSessionStats.KillCount}";
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
            rectTransform.sizeDelta = new Vector2(460f, 380f);
            rectTransform.anchoredPosition = Vector2.zero;
        }

        private static Text CreateLabel(Transform parent, string text, Vector2 position, int fontSize, Color color, Vector2 size, string objectName = null)
        {
            GameObject labelObject = new GameObject(string.IsNullOrEmpty(objectName) ? text : objectName);
            labelObject.transform.SetParent(parent, false);

            RectTransform rectTransform = labelObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = position;

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
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(250f, 44f);
            rectTransform.anchoredPosition = position;

            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.82f, 0.9f, 0.76f, 1f);

            Button button = buttonObject.AddComponent<Button>();
            CreateLabel(buttonObject.transform, text, Vector2.zero, 18, new Color(0.06f, 0.09f, 0.06f, 1f), new Vector2(230f, 40f));
            return button;
        }
    }
}
