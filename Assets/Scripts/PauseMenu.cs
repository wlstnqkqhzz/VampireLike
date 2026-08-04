using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using VampireLike.Audio;
using VampireLike.Settings;

/// <summary>
/// Escape 키로 일시정지 메뉴를 열고 닫으며, 계속하기/게임 종료 버튼을 처리한다.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    private const string PauseCanvasName = "Pause Menu Canvas";
    private const string PauseRootName = "Pause Menu";
    private const string PausePanelName = "Pause Panel";
    private const string OptionsPanelName = "Options Panel";

    [SerializeField]
    private GameObject pauseMenuRoot;

    // 일시정지 해제 버튼이다. 자동 생성 UI 또는 Inspector 연결 둘 다 지원한다.
    [SerializeField]
    private Button resumeButton;

    [SerializeField]
    private Button optionsButton;

    // 에디터에서는 Play Mode를 종료하고, 빌드에서는 애플리케이션을 종료한다.
    [SerializeField]
    private Button quitButton;

    private RectTransform pausePanel;
    private RectTransform optionsPanel;
    private Slider masterVolumeSlider;
    private Slider sfxVolumeSlider;
    private Toggle fullscreenToggle;
    private Text resolutionText;
    private Text appliedScreenText;
    private Button previousResolutionButton;
    private Button nextResolutionButton;
    private Button optionsBackButton;
    private bool isPaused;

    private void Awake()
    {
        EnsurePauseMenu();
        EnsureEventSystem();

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (optionsButton != null)
            optionsButton.onClick.AddListener(ShowOptions);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        BindOptionsControls();

        SetPaused(false);
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (VampireLike.Combat.GameState.IsGameOver || VampireLike.Combat.GameState.IsMainMenuOpen)
            return;

        // New Input System 기준으로 Escape 입력을 직접 확인한다.
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            TogglePause();
    }

    private void OnDestroy()
    {
        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(ResumeGame);

        if (optionsButton != null)
            optionsButton.onClick.RemoveListener(ShowOptions);

        if (quitButton != null)
            quitButton.onClick.RemoveListener(QuitGame);

        UnbindOptionsControls();

        if (isPaused)
            Time.timeScale = 1f;
    }

    public void TogglePause()
    {
        GameSfx.Play(SfxType.UpgradeSelect);
        SetPaused(!isPaused);
    }

    public void ResumeGame()
    {
        GameSfx.Play(SfxType.UpgradeSelect);
        SetPaused(false);
    }

    public void QuitGame()
    {
        GameSfx.Play(SfxType.UpgradeSelect);
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

        if (isPaused)
            ShowPausePanel();
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
        resumeButton = CreateButton(panel.transform, "계속하기", new Vector2(0f, 18f));
        optionsButton = CreateButton(panel.transform, "옵션", new Vector2(0f, -36f));
        quitButton = CreateButton(panel.transform, "게임 종료", new Vector2(0f, -90f));

        CreateOptionsPanel(root.transform);
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

        Transform optionsTransform = pauseMenuRoot.transform.Find(OptionsPanelName);

        if (optionsTransform != null)
            optionsPanel = optionsTransform.GetComponent<RectTransform>();
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

        if (optionsPanel != null)
            CenterOptionsPanel(optionsPanel);
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

    private static void CenterOptionsPanel(RectTransform rectTransform)
    {
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(480f, 360f);
        rectTransform.anchoredPosition = Vector2.zero;
    }

    private void CreateOptionsPanel(Transform parent)
    {
        GameObject panel = new GameObject(OptionsPanelName);
        panel.transform.SetParent(parent, false);
        optionsPanel = panel.AddComponent<RectTransform>();
        CenterOptionsPanel(optionsPanel);

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.08f, 0.1f, 0.09f, 0.95f);

        CreateLabel(panel.transform, "옵션", new Vector2(0f, 136f), 28, Color.white);
        CreateLabel(panel.transform, "전체 음량", new Vector2(-150f, 78f), 17, new Color(0.9f, 0.95f, 0.86f, 1f));
        CreateLabel(panel.transform, "효과음", new Vector2(-150f, 28f), 17, new Color(0.9f, 0.95f, 0.86f, 1f));
        CreateLabel(panel.transform, "전체화면", new Vector2(-150f, -28f), 17, new Color(0.9f, 0.95f, 0.86f, 1f));
        CreateLabel(panel.transform, "해상도", new Vector2(-150f, -82f), 17, new Color(0.9f, 0.95f, 0.86f, 1f));

        masterVolumeSlider = CreateSlider(panel.transform, new Vector2(90f, 78f));
        sfxVolumeSlider = CreateSlider(panel.transform, new Vector2(90f, 28f));
        fullscreenToggle = CreateToggle(panel.transform, new Vector2(22f, -28f));
        previousResolutionButton = CreateButton(panel.transform, "<", new Vector2(-6f, -82f), new Vector2(46f, 36f));
        resolutionText = CreateText(panel.transform, string.Empty, new Vector2(90f, -82f), 16, Color.white, new Vector2(150f, 34f));
        nextResolutionButton = CreateButton(panel.transform, ">", new Vector2(186f, -82f), new Vector2(46f, 36f));
        appliedScreenText = CreateText(panel.transform, string.Empty, new Vector2(0f, -114f), 14, new Color(0.82f, 0.9f, 0.78f, 1f), new Vector2(360f, 26f));
        optionsBackButton = CreateButton(panel.transform, "뒤로", new Vector2(0f, -142f));
        RefreshOptionsControls();
        panel.SetActive(false);
    }

    private void ShowOptions()
    {
        GameSfx.Play(SfxType.UpgradeSelect);

        if (pausePanel != null)
            pausePanel.gameObject.SetActive(false);

        if (optionsPanel != null)
            optionsPanel.gameObject.SetActive(true);

        RefreshOptionsControls();
    }

    private void ShowPausePanel()
    {
        if (pausePanel != null)
            pausePanel.gameObject.SetActive(true);

        if (optionsPanel != null)
            optionsPanel.gameObject.SetActive(false);
    }

    private void BindOptionsControls()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(GameOptions.SetMasterVolume);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(GameOptions.SetSfxVolume);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(GameOptions.SetFullscreen);

        if (previousResolutionButton != null)
            previousResolutionButton.onClick.AddListener(SelectPreviousResolution);

        if (nextResolutionButton != null)
            nextResolutionButton.onClick.AddListener(SelectNextResolution);

        if (optionsBackButton != null)
            optionsBackButton.onClick.AddListener(ShowPausePanel);
    }

    private void UnbindOptionsControls()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.RemoveListener(GameOptions.SetMasterVolume);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.RemoveListener(GameOptions.SetSfxVolume);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.RemoveListener(GameOptions.SetFullscreen);

        if (previousResolutionButton != null)
            previousResolutionButton.onClick.RemoveListener(SelectPreviousResolution);

        if (nextResolutionButton != null)
            nextResolutionButton.onClick.RemoveListener(SelectNextResolution);

        if (optionsBackButton != null)
            optionsBackButton.onClick.RemoveListener(ShowPausePanel);
    }

    private void SelectPreviousResolution()
    {
        GameSfx.Play(SfxType.UpgradeSelect);
        GameOptions.SetResolutionIndex((GameOptions.ResolutionIndex - 1 + GameOptions.ResolutionCount) % GameOptions.ResolutionCount);
        RefreshOptionsControls();
    }

    private void SelectNextResolution()
    {
        GameSfx.Play(SfxType.UpgradeSelect);
        GameOptions.SetResolutionIndex((GameOptions.ResolutionIndex + 1) % GameOptions.ResolutionCount);
        RefreshOptionsControls();
    }

    private void RefreshOptionsControls()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.SetValueWithoutNotify(GameOptions.MasterVolume);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.SetValueWithoutNotify(GameOptions.SfxVolume);

        if (fullscreenToggle != null)
            fullscreenToggle.SetIsOnWithoutNotify(GameOptions.IsFullscreen);

        if (resolutionText != null)
        {
            Vector2Int resolution = GameOptions.CurrentResolution;
            resolutionText.text = $"{resolution.x} x {resolution.y}";
        }

        if (appliedScreenText != null)
            appliedScreenText.text = $"현재 적용: {GameOptions.AppliedScreenInfo}";
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
        return CreateButton(parent, text, position, new Vector2(220f, 44f));
    }

    private static Button CreateButton(Transform parent, string text, Vector2 position, Vector2 size)
    {
        GameObject buttonObject = new GameObject(text);
        buttonObject.transform.SetParent(parent, false);

        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = position;

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.84f, 0.92f, 0.72f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        CreateLabel(buttonObject.transform, text, Vector2.zero, 18, new Color(0.08f, 0.12f, 0.08f, 1f));

        return button;
    }

    private static Slider CreateSlider(Transform parent, Vector2 position)
    {
        GameObject sliderObject = new GameObject("Slider");
        sliderObject.transform.SetParent(parent, false);

        RectTransform rectTransform = sliderObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(220f, 20f);
        rectTransform.anchoredPosition = position;

        Slider slider = sliderObject.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;

        GameObject backgroundObject = new GameObject("Background");
        backgroundObject.transform.SetParent(sliderObject.transform, false);
        Image background = backgroundObject.AddComponent<Image>();
        background.color = new Color(0.02f, 0.025f, 0.02f, 1f);
        RectTransform backgroundRect = backgroundObject.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;

        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObject.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = new Vector2(5f, 0f);
        fillAreaRect.offsetMax = new Vector2(-5f, 0f);

        GameObject fillObject = new GameObject("Fill");
        fillObject.transform.SetParent(fillArea.transform, false);
        Image fill = fillObject.AddComponent<Image>();
        fill.color = new Color(0.5f, 0.75f, 1f, 1f);
        RectTransform fillRect = fillObject.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObject.transform, false);
        RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = new Vector2(8f, 0f);
        handleAreaRect.offsetMax = new Vector2(-8f, 0f);

        GameObject handleObject = new GameObject("Handle");
        handleObject.transform.SetParent(handleArea.transform, false);
        Image handle = handleObject.AddComponent<Image>();
        handle.color = new Color(0.86f, 0.96f, 0.78f, 1f);
        RectTransform handleRect = handleObject.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(18f, 26f);

        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handle;
        slider.direction = Slider.Direction.LeftToRight;
        return slider;
    }

    private static Toggle CreateToggle(Transform parent, Vector2 position)
    {
        GameObject toggleObject = new GameObject("Toggle");
        toggleObject.transform.SetParent(parent, false);

        RectTransform rectTransform = toggleObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(160f, 32f);
        rectTransform.anchoredPosition = position;

        Image background = toggleObject.AddComponent<Image>();
        background.color = new Color(0.02f, 0.025f, 0.02f, 1f);

        GameObject checkObject = new GameObject("Checkmark");
        checkObject.transform.SetParent(toggleObject.transform, false);
        Image checkmark = checkObject.AddComponent<Image>();
        checkmark.color = new Color(0.5f, 0.75f, 1f, 1f);
        RectTransform checkRect = checkObject.GetComponent<RectTransform>();
        checkRect.anchorMin = new Vector2(0f, 0.5f);
        checkRect.anchorMax = new Vector2(0f, 0.5f);
        checkRect.pivot = new Vector2(0.5f, 0.5f);
        checkRect.sizeDelta = new Vector2(24f, 24f);
        checkRect.anchoredPosition = new Vector2(18f, 0f);

        Text label = CreateText(toggleObject.transform, "켜짐", new Vector2(42f, 0f), 16, Color.white, new Vector2(90f, 28f));
        label.alignment = TextAnchor.MiddleLeft;

        Toggle toggle = toggleObject.AddComponent<Toggle>();
        toggle.targetGraphic = background;
        toggle.graphic = checkmark;
        return toggle;
    }

    private static Text CreateText(Transform parent, string text, Vector2 position, int fontSize, Color color, Vector2 size)
    {
        GameObject textObject = new GameObject(text);
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = position;

        Text label = textObject.AddComponent<Text>();
        label.text = text;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = fontSize;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = color;
        label.raycastTarget = false;
        return label;
    }
}
