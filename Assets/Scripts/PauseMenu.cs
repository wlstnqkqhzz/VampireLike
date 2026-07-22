using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

/// <summary>
/// Escape 키로 일시정지 메뉴를 열고 닫으며, 계속하기/게임 종료 버튼을 처리한다.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    private const string PauseCanvasName = "Pause Menu Canvas";
    private const string PauseRootName = "Pause Menu";
    private const string PausePanelName = "Pause Panel";

    [SerializeField]
    private GameObject pauseMenuRoot;

    // 일시정지 해제 버튼이다. 자동 생성 UI 또는 Inspector 연결 둘 다 지원한다.
    [SerializeField]
    private Button resumeButton;

    // 에디터에서는 Play Mode를 종료하고, 빌드에서는 애플리케이션을 종료한다.
    [SerializeField]
    private Button quitButton;

    private RectTransform pausePanel;
    private bool isPaused;

    private void Awake()
    {
        EnsurePauseMenu();
        EnsureEventSystem();

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        SetPaused(false);
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        // New Input System 기준으로 Escape 입력을 직접 확인한다.
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            TogglePause();
    }

    private void OnDestroy()
    {
        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(ResumeGame);

        if (quitButton != null)
            quitButton.onClick.RemoveListener(QuitGame);

        if (isPaused)
            Time.timeScale = 1f;
    }

    public void TogglePause()
    {
        SetPaused(!isPaused);
    }

    public void ResumeGame()
    {
        SetPaused(false);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void SetPaused(bool paused)
    {
        isPaused = paused;
        // Time.timeScale이 0이면 이동, 공격, 적 생성처럼 deltaTime 기반 동작이 멈춘다.
        Time.timeScale = isPaused ? 0f : 1f;
        CenterPauseMenu();

        if (pauseMenuRoot != null)
            pauseMenuRoot.SetActive(isPaused);
    }

    private void EnsurePauseMenu()
    {
        // 씬에 이미 메뉴가 있으면 재사용하고, 없으면 런타임에 기본 UI를 만든다.
        if (pauseMenuRoot != null)
        {
            CachePausePanel();
            CenterPauseMenu();
            return;
        }

        Canvas canvas = CreatePauseCanvas();

        GameObject root = new GameObject(PauseRootName);
        root.transform.SetParent(canvas.transform, false);
        pauseMenuRoot = root;

        RectTransform rootRect = root.AddComponent<RectTransform>();
        StretchToParent(rootRect);

        Image backdrop = root.AddComponent<Image>();
        backdrop.color = new Color(0f, 0f, 0f, 0.55f);

        GameObject panel = new GameObject(PausePanelName);
        panel.transform.SetParent(root.transform, false);
        pausePanel = panel.AddComponent<RectTransform>();
        CenterPausePanel(pausePanel);

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.12f, 0.14f, 0.12f, 0.92f);

        CreateLabel(panel.transform, "일시정지", new Vector2(0f, 58f), 30, Color.white);
        resumeButton = CreateButton(panel.transform, "계속하기", new Vector2(0f, 4f));
        quitButton = CreateButton(panel.transform, "게임 종료", new Vector2(0f, -52f));
    }

    private static Canvas CreatePauseCanvas()
    {
        GameObject canvasObject = new GameObject(PauseCanvasName);
        int uiLayer = LayerMask.NameToLayer("UI");

        if (uiLayer >= 0)
            canvasObject.layer = uiLayer;

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private static void EnsureEventSystem()
    {
        // 버튼 클릭을 받을 EventSystem이 없으면 New Input System용 모듈과 함께 만든다.
        if (FindFirstObjectByType<EventSystem>() != null)
            return;

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<InputSystemUIInputModule>();
    }

    private void CachePausePanel()
    {
        Transform panelTransform = pauseMenuRoot.transform.Find(PausePanelName);

        if (panelTransform != null)
            pausePanel = panelTransform.GetComponent<RectTransform>();
    }

    private void CenterPauseMenu()
    {
        if (pauseMenuRoot == null)
            return;

        RectTransform rootRect = pauseMenuRoot.GetComponent<RectTransform>();

        if (rootRect != null)
            StretchToParent(rootRect);

        if (pausePanel != null)
            CenterPausePanel(pausePanel);
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

    private static void CenterPausePanel(RectTransform rectTransform)
    {
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(320f, 210f);
        rectTransform.anchoredPosition = Vector2.zero;
    }

    private static void CreateLabel(Transform parent, string text, Vector2 position, int fontSize, Color color)
    {
        GameObject labelObject = new GameObject(text);
        labelObject.transform.SetParent(parent, false);

        RectTransform rectTransform = labelObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(260f, 44f);
        rectTransform.anchoredPosition = position;

        Text label = labelObject.AddComponent<Text>();
        label.text = text;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = fontSize;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = color;
        label.raycastTarget = false;
    }

    private static Button CreateButton(Transform parent, string text, Vector2 position)
    {
        GameObject buttonObject = new GameObject(text);
        buttonObject.transform.SetParent(parent, false);

        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(220f, 44f);
        rectTransform.anchoredPosition = position;

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.84f, 0.92f, 0.72f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        CreateLabel(buttonObject.transform, text, Vector2.zero, 18, new Color(0.08f, 0.12f, 0.08f, 1f));

        return button;
    }
}
