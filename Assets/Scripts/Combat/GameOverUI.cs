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
    /// 플레이어 사망 후 생존 기록과 성장 결과를 보여주는 게임 오버 결과 화면입니다.
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        private const string CanvasName = "Game Over Canvas";
        private const string RootName = "Game Over";
        private const string PanelName = "Game Over Panel";
        private const string TitleName = "Game Over Title";
        private const string SubtitleName = "Game Over Subtitle";
        private const string CharacterName = "Character Result";
        private const string MainStatsName = "Main Stats";
        private const string CombatStatsName = "Combat Stats";
        private const string GrowthStatsName = "Growth Stats";
        private const string UpgradeHeaderName = "Upgrade Header";
        private const string UpgradeResultName = "Upgrade Result";
        private const string MainMenuSceneName = "MainMenuScene";

        [SerializeField]
        private GameObject gameOverRoot;

        [SerializeField]
        private Button restartButton;

        [SerializeField]
        private Button quitButton;

        private RectTransform gameOverPanel;
        private Text titleText;
        private Text subtitleText;
        private Text characterText;
        private Text mainStatsText;
        private Text combatStatsText;
        private Text growthStatsText;
        private Text upgradeHeaderText;
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
            SceneManager.LoadScene(MainMenuSceneName);
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
            backdrop.color = new Color(0f, 0f, 0f, 0.72f);

            GameObject panel = new GameObject(PanelName);
            panel.transform.SetParent(root.transform, false);
            gameOverPanel = panel.AddComponent<RectTransform>();
            CenterPanel(gameOverPanel);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0.045f, 0.052f, 0.055f, 0.97f);

            EnsureResultLabels();
            restartButton = CreateButton(panel.transform, "다시 시작", new Vector2(-144f, -252f));
            quitButton = CreateButton(panel.transform, "게임 종료", new Vector2(144f, -252f));
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
            subtitleText = panelTransform.Find(SubtitleName)?.GetComponent<Text>();
            characterText = panelTransform.Find(CharacterName)?.GetComponent<Text>();
            mainStatsText = panelTransform.Find(MainStatsName)?.GetComponent<Text>();
            combatStatsText = panelTransform.Find(CombatStatsName)?.GetComponent<Text>();
            growthStatsText = panelTransform.Find(GrowthStatsName)?.GetComponent<Text>();
            upgradeHeaderText = panelTransform.Find(UpgradeHeaderName)?.GetComponent<Text>();
            upgradeText = panelTransform.Find(UpgradeResultName)?.GetComponent<Text>();
        }

        private void EnsureResultLabels()
        {
            if (gameOverPanel == null)
                return;

            if (titleText == null)
                titleText = CreateLabel(gameOverPanel, "게임 오버", new Vector2(0f, 250f), 42, Color.white, new Vector2(560f, 54f), TitleName, FontStyle.Bold);

            if (subtitleText == null)
                subtitleText = CreateLabel(gameOverPanel, "이번 생존 기록", new Vector2(0f, 210f), 18, new Color(0.76f, 0.86f, 0.76f, 1f), new Vector2(560f, 28f), SubtitleName, FontStyle.Bold);

            if (characterText == null)
                characterText = CreateLabel(gameOverPanel, "캐릭터 -", new Vector2(0f, 166f), 22, new Color(0.92f, 0.96f, 0.9f, 1f), new Vector2(600f, 34f), CharacterName, FontStyle.Bold);

            if (mainStatsText == null)
                mainStatsText = CreateLabel(gameOverPanel, string.Empty, new Vector2(-210f, 78f), 18, new Color(0.88f, 0.94f, 0.86f, 1f), new Vector2(260f, 120f), MainStatsName, FontStyle.Bold);

            if (combatStatsText == null)
                combatStatsText = CreateLabel(gameOverPanel, string.Empty, new Vector2(0f, 78f), 18, new Color(0.88f, 0.94f, 0.86f, 1f), new Vector2(260f, 120f), CombatStatsName, FontStyle.Bold);

            if (growthStatsText == null)
                growthStatsText = CreateLabel(gameOverPanel, string.Empty, new Vector2(210f, 78f), 18, new Color(0.88f, 0.94f, 0.86f, 1f), new Vector2(260f, 120f), GrowthStatsName, FontStyle.Bold);

            if (upgradeHeaderText == null)
                upgradeHeaderText = CreateLabel(gameOverPanel, "선택한 강화", new Vector2(0f, -18f), 20, new Color(0.96f, 0.9f, 0.64f, 1f), new Vector2(560f, 30f), UpgradeHeaderName, FontStyle.Bold);

            if (upgradeText == null)
                upgradeText = CreateLabel(gameOverPanel, "선택한 강화 없음", new Vector2(0f, -108f), 16, new Color(0.8f, 0.88f, 0.76f, 1f), new Vector2(600f, 136f), UpgradeResultName, FontStyle.Normal);

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

            if (subtitleText != null)
                subtitleText.text = "이번 생존 기록";

            if (characterText != null)
                characterText.text = $"{GameSessionStats.CharacterDisplayName}  |  {GameSessionStats.CharacterRole}";

            if (mainStatsText != null)
            {
                string wave = enemySpawner == null ? "-" : enemySpawner.CurrentWave.ToString();
                mainStatsText.text = $"생존 시간\n{FormatTime(GameSessionStats.SurvivalTime)}\n\n도달 웨이브\n{wave}";
            }

            if (combatStatsText != null)
            {
                combatStatsText.text = $"총 처치\n{GameSessionStats.KillCount}\n\n일반 / 보스\n{GameSessionStats.EnemyKillCount} / {GameSessionStats.BossKillCount}";
            }

            if (growthStatsText != null)
            {
                string level = playerExperience == null ? "-" : playerExperience.CurrentLevel.ToString();
                growthStatsText.text = $"도달 레벨\n{level}\n\n획득 경험치\n{GameSessionStats.TotalExperienceGained}";
            }

            if (upgradeHeaderText != null)
                upgradeHeaderText.text = "선택한 강화";

            if (upgradeText != null)
                upgradeText.text = GameSessionStats.GetUpgradeSummary(6, "\n");
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
            rectTransform.sizeDelta = new Vector2(720f, 620f);
            rectTransform.anchoredPosition = Vector2.zero;
        }

        private void PositionResultObjects()
        {
            SetLabelPosition(titleText, new Vector2(0f, 250f), new Vector2(560f, 54f));
            SetLabelPosition(subtitleText, new Vector2(0f, 210f), new Vector2(560f, 28f));
            SetLabelPosition(characterText, new Vector2(0f, 166f), new Vector2(600f, 34f));
            SetLabelPosition(mainStatsText, new Vector2(-210f, 78f), new Vector2(260f, 120f));
            SetLabelPosition(combatStatsText, new Vector2(0f, 78f), new Vector2(260f, 120f));
            SetLabelPosition(growthStatsText, new Vector2(210f, 78f), new Vector2(260f, 120f));
            SetLabelPosition(upgradeHeaderText, new Vector2(0f, -18f), new Vector2(560f, 30f));
            SetLabelPosition(upgradeText, new Vector2(0f, -108f), new Vector2(600f, 136f));

            if (restartButton != null)
                SetRectPosition(restartButton.GetComponent<RectTransform>(), new Vector2(-144f, -252f), new Vector2(240f, 46f));

            if (quitButton != null)
                SetRectPosition(quitButton.GetComponent<RectTransform>(), new Vector2(144f, -252f), new Vector2(240f, 46f));
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

        private static Text CreateLabel(Transform parent, string text, Vector2 position, int fontSize, Color color, Vector2 size, string objectName, FontStyle fontStyle)
        {
            GameObject labelObject = new GameObject(objectName);
            labelObject.transform.SetParent(parent, false);

            RectTransform rectTransform = labelObject.AddComponent<RectTransform>();
            SetRectPosition(rectTransform, position, size);

            Text label = labelObject.AddComponent<Text>();
            label.text = text;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = fontSize;
            label.fontStyle = fontStyle;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = color;
            label.raycastTarget = false;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            return label;
        }

        private static Button CreateButton(Transform parent, string text, Vector2 position)
        {
            GameObject buttonObject = new GameObject(text);
            buttonObject.transform.SetParent(parent, false);

            RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
            SetRectPosition(rectTransform, position, new Vector2(240f, 46f));

            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.78f, 0.88f, 0.7f, 1f);

            Button button = buttonObject.AddComponent<Button>();
            CreateLabel(buttonObject.transform, text, Vector2.zero, 18, new Color(0.04f, 0.07f, 0.04f, 1f), new Vector2(220f, 42f), $"{text} Label", FontStyle.Bold);
            return button;
        }
    }
}
