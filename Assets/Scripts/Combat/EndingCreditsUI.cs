using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VampireLike.Enemies;
using VampireLike.Growth;
using VampireLike.Save;
using VampireLike.UI;

namespace VampireLike.Combat
{
    /// <summary>
    /// 히든 보스 처치 후 승리 결과와 엔딩 크레딧을 표시하는 전용 UI입니다.
    /// </summary>
    public class EndingCreditsUI : MonoBehaviour
    {
        private const string CanvasName = "Ending Credits Canvas";
        private const string MainMenuSceneName = "MainMenuScene";

        private static EndingCreditsUI instance;

        private GameObject root;
        private RectTransform panel;
        private Text titleText;
        private Text subtitleText;
        private Text characterText;
        private Text statsText;
        private Text upgradeText;
        private Text creditsText;
        private Button mainMenuButton;
        private Button quitButton;
        private bool isShowing;
        private bool hasSubmittedRecord;

        /// <summary>
        /// 히든 보스 클리어 결과를 화면에 표시하고 게임 진행을 멈춥니다.
        /// </summary>
        public static void ShowHiddenBossEnding(int absorbedExperience)
        {
            if (instance == null)
                instance = CreateInstance();

            instance.Show(absorbedExperience);
        }

        private static EndingCreditsUI CreateInstance()
        {
            GameObject uiObject = new GameObject(CanvasName);
            return uiObject.AddComponent<EndingCreditsUI>();
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            EnsureUI();
            EnsureEventSystem();
            Hide();
        }

        private void OnDestroy()
        {
            if (mainMenuButton != null)
                mainMenuButton.onClick.RemoveListener(GoToMainMenu);

            if (quitButton != null)
                quitButton.onClick.RemoveListener(QuitGame);

            if (instance == this)
                instance = null;
        }

        private void Show(int absorbedExperience)
        {
            if (isShowing)
                return;

            isShowing = true;
            Time.timeScale = 0f;
            GameSessionStats.EndRun();
            SubmitRecord();
            UpdateTexts(absorbedExperience);

            if (root != null)
                root.SetActive(true);
        }

        private void Hide()
        {
            isShowing = false;

            if (root != null)
                root.SetActive(false);
        }

        private void SubmitRecord()
        {
            if (hasSubmittedRecord)
                return;

            PlayerExperience playerExperience = FindFirstObjectByType<PlayerExperience>();
            EnemySpawner enemySpawner = FindFirstObjectByType<EnemySpawner>();
            int currentWave = enemySpawner == null ? 0 : enemySpawner.CurrentWave;
            int currentLevel = playerExperience == null ? 0 : playerExperience.CurrentLevel;

            HighScoreRecord runRecord = new HighScoreRecord(
                GameSessionStats.SurvivalTime,
                currentWave,
                currentLevel,
                GameSessionStats.KillCount,
                GameSessionStats.BossKillCount,
                GameSessionStats.TotalExperienceGained);

            HighScoreManager.SubmitRun(GameSessionStats.CharacterId, runRecord);
            RunHistoryManager.AddRun(new RunHistoryEntry(
                System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                GameSessionStats.CharacterId,
                GameSessionStats.CharacterDisplayName,
                GameSessionStats.CharacterRole,
                GameSessionStats.SurvivalTime,
                currentWave,
                currentLevel,
                GameSessionStats.KillCount,
                GameSessionStats.EnemyKillCount,
                GameSessionStats.BossKillCount,
                GameSessionStats.TotalExperienceGained,
                GameSessionStats.GetUpgradeSummary()));

            hasSubmittedRecord = true;
        }

        private void UpdateTexts(int absorbedExperience)
        {
            PlayerExperience playerExperience = FindFirstObjectByType<PlayerExperience>();
            EnemySpawner enemySpawner = FindFirstObjectByType<EnemySpawner>();
            int currentWave = enemySpawner == null ? 0 : enemySpawner.CurrentWave;
            int currentLevel = playerExperience == null ? 0 : playerExperience.CurrentLevel;

            if (titleText != null)
                titleText.text = "생존 완료";

            if (subtitleText != null)
                subtitleText.text = "탐욕의 군주를 쓰러뜨리고 저주받은 밤을 넘겼습니다";

            if (characterText != null)
                characterText.text = $"{GameSessionStats.CharacterDisplayName}  |  {GameSessionStats.CharacterRole}";

            if (statsText != null)
            {
                statsText.text =
                    $"생존 시간  {FormatTime(GameSessionStats.SurvivalTime)}\n" +
                    $"도달 웨이브  {FormatValue(currentWave)}\n" +
                    $"도달 레벨  {FormatValue(currentLevel)}\n" +
                    $"총 처치  {GameSessionStats.KillCount}\n" +
                    $"보스 처치  {GameSessionStats.BossKillCount}\n" +
                    $"획득 경험치  {GameSessionStats.TotalExperienceGained}\n" +
                    $"히든 보스 흡수 경험치  {absorbedExperience}";
            }

            if (upgradeText != null)
                upgradeText.text = GameSessionStats.GetUpgradeSummary(7, "\n");

            if (creditsText != null)
                creditsText.text = "Thank you for playing\nVampireLike";
        }

        private void GoToMainMenu()
        {
            Time.timeScale = 1f;
            GameState.ResetGame();
            SceneManager.LoadScene(MainMenuSceneName);
            Hide();
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
            Canvas canvas = GetComponent<Canvas>();

            if (canvas == null)
                canvas = gameObject.AddComponent<Canvas>();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 2500;

            CanvasScaler scaler = GetComponent<CanvasScaler>();

            if (scaler == null)
                scaler = gameObject.AddComponent<CanvasScaler>();

            MobileSafeArea.ConfigureCanvasScaler(scaler);

            if (GetComponent<ResponsiveCanvasScaler>() == null)
                gameObject.AddComponent<ResponsiveCanvasScaler>();

            if (GetComponent<GraphicRaycaster>() == null)
                gameObject.AddComponent<GraphicRaycaster>();

            root = new GameObject("Ending Credits");
            root.transform.SetParent(transform, false);

            RectTransform rootRect = root.AddComponent<RectTransform>();
            MobileSafeArea.ApplyTo(rootRect);

            Image backdrop = root.AddComponent<Image>();
            backdrop.color = new Color(0f, 0f, 0f, 0.76f);

            GameObject panelObject = new GameObject("Ending Credits Panel");
            panelObject.transform.SetParent(root.transform, false);
            panel = panelObject.AddComponent<RectTransform>();
            SetRect(panel, Vector2.zero, new Vector2(820f, 720f));

            Image panelImage = panelObject.AddComponent<Image>();
            panelImage.color = new Color(0.025f, 0.034f, 0.036f, 0.96f);

            titleText = CreateLabel(panel, "생존 완료", new Vector2(0f, 300f), 46, Color.white, new Vector2(720f, 58f), FontStyle.Bold);
            subtitleText = CreateLabel(panel, string.Empty, new Vector2(0f, 252f), 19, new Color(0.74f, 0.88f, 0.78f, 1f), new Vector2(720f, 34f), FontStyle.Bold);
            characterText = CreateLabel(panel, string.Empty, new Vector2(0f, 204f), 23, new Color(0.92f, 0.96f, 0.9f, 1f), new Vector2(720f, 38f), FontStyle.Bold);
            statsText = CreateLabel(panel, string.Empty, new Vector2(-220f, 66f), 18, new Color(0.88f, 0.94f, 0.86f, 1f), new Vector2(300f, 220f), FontStyle.Bold);
            upgradeText = CreateLabel(panel, string.Empty, new Vector2(190f, 66f), 17, new Color(0.86f, 0.92f, 0.82f, 1f), new Vector2(380f, 220f), FontStyle.Normal);
            creditsText = CreateLabel(panel, string.Empty, new Vector2(0f, -116f), 22, new Color(0.82f, 0.9f, 0.72f, 1f), new Vector2(640f, 88f), FontStyle.Bold);
            mainMenuButton = CreateButton(panel, "메인 메뉴", new Vector2(-150f, -274f));
            quitButton = CreateButton(panel, "게임 종료", new Vector2(150f, -274f));

            mainMenuButton.onClick.AddListener(GoToMainMenu);
            quitButton.onClick.AddListener(QuitGame);
        }

        private static void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null)
                return;

            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }

        private static Text CreateLabel(Transform parent, string text, Vector2 position, int fontSize, Color color, Vector2 size, FontStyle fontStyle)
        {
            GameObject labelObject = new GameObject(text);
            labelObject.transform.SetParent(parent, false);

            RectTransform rectTransform = labelObject.AddComponent<RectTransform>();
            SetRect(rectTransform, position, size);

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
            SetRect(rectTransform, position, new Vector2(250f, 54f));

            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.78f, 0.88f, 0.7f, 1f);

            Button button = buttonObject.AddComponent<Button>();
            CreateLabel(buttonObject.transform, text, Vector2.zero, 18, new Color(0.04f, 0.07f, 0.04f, 1f), new Vector2(230f, 44f), FontStyle.Bold);
            return button;
        }

        private static void SetRect(RectTransform rectTransform, Vector2 position, Vector2 size)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = size;
        }

        private static string FormatTime(float seconds)
        {
            int totalSeconds = Mathf.FloorToInt(seconds);
            int minutes = totalSeconds / 60;
            int remainingSeconds = totalSeconds % 60;
            return $"{minutes:00}:{remainingSeconds:00}";
        }

        private static string FormatValue(int value)
        {
            return value <= 0 ? "-" : value.ToString();
        }
    }
}
