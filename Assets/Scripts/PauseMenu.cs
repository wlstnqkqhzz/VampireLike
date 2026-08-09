using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using VampireLike.Audio;
using VampireLike.Settings;
using VampireLike.UI;

/// <summary>
/// Escape ?ㅻ줈 ?쇱떆?뺤? 硫붾돱瑜??닿퀬 ?レ쑝硫? 怨꾩냽?섍린/寃뚯엫 醫낅즺 踰꾪듉??泥섎━?쒕떎.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    private const string PauseCanvasName = "Pause Menu Canvas";
    private const string PauseRootName = "Pause Menu";
    private const string PausePanelName = "Pause Panel";
    private const string OptionsPanelName = "Options Panel";

    [SerializeField]
    private GameObject pauseMenuRoot;

    // ?쇱떆?뺤? ?댁젣 踰꾪듉?대떎. ?먮룞 ?앹꽦 UI ?먮뒗 Inspector ?곌껐 ????吏?먰븳??
    [SerializeField]
    private Button resumeButton;

    [SerializeField]
    private Button optionsButton;

    // ?먮뵒?곗뿉?쒕뒗 Play Mode瑜?醫낅즺?섍퀬, 鍮뚮뱶?먯꽌???좏뵆由ъ??댁뀡??醫낅즺?쒕떎.
    [SerializeField]
    private Button quitButton;

    private RectTransform pausePanel;
    private RectTransform optionsPanel;
    private Slider masterVolumeSlider;
    private Slider bgmVolumeSlider;
    private Slider sfxVolumeSlider;
    private Toggle fullscreenToggle;
    private Text resolutionText;
    private Text appliedScreenText;
    private Text fullscreenModeText;
    private Button previousResolutionButton;
    private Button nextResolutionButton;
    private Button optionsConfirmButton;
    private Button optionsBackButton;
    private bool isPaused;
    private float pendingMasterVolume;
    private float pendingBgmVolume;
    private float pendingSfxVolume;
    private bool pendingFullscreen;
    private int pendingResolutionIndex;

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

        // New Input System 湲곗??쇰줈 Escape ?낅젰??吏곸젒 ?뺤씤?쒕떎.
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
        // Time.timeScale??0?대㈃ ?대룞, 怨듦꺽, ???앹꽦泥섎읆 deltaTime 湲곕컲 ?숈옉??硫덉텣??
        Time.timeScale = isPaused ? 0f : 1f;
        CenterPauseMenu();

        if (pauseMenuRoot != null)
            pauseMenuRoot.SetActive(isPaused);

        if (isPaused)
            ShowPausePanel();
    }

    private void EnsurePauseMenu()
    {
        // ?ъ뿉 ?대? 硫붾돱媛 ?덉쑝硫??ъ궗?⑺븯怨? ?놁쑝硫??고??꾩뿉 湲곕낯 UI瑜?留뚮뱺??
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

        CreateLabel(panel.transform, "\uC77C\uC2DC\uC815\uC9C0", new Vector2(0f, 108f), 32, Color.white);
        resumeButton = CreateButton(panel.transform, "\uACC4\uC18D\uD558\uAE30", new Vector2(0f, 54f), new Vector2(260f, 54f));
        optionsButton = CreateButton(panel.transform, "\uC635\uC158", new Vector2(0f, -18f), new Vector2(260f, 54f));
        quitButton = CreateButton(panel.transform, "\uAC8C\uC784 \uC885\uB8CC", new Vector2(0f, -90f), new Vector2(260f, 54f));

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
        // 踰꾪듉 ?대┃??諛쏆쓣 EventSystem???놁쑝硫?New Input System??紐⑤뱢怨??④퍡 留뚮뱺??
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
        MobileSafeArea.ApplyTo(rectTransform);
    }

    private static void CenterPausePanel(RectTransform rectTransform)
    {
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(420f, 320f);
        rectTransform.anchoredPosition = Vector2.zero;
    }

    private static void CenterOptionsPanel(RectTransform rectTransform)
    {
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(640f, 620f);
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

        CreateLabel(panel.transform, "\uC635\uC158", new Vector2(0f, 244f), 32, Color.white);
        CreateLabel(panel.transform, "\uAC8C\uC784 \uD654\uBA74\uACFC \uC74C\uB7C9\uC744 \uC870\uC815\uD569\uB2C8\uB2E4", new Vector2(0f, 204f), 16, new Color(0.82f, 0.9f, 0.78f, 1f));
        CreateLabel(panel.transform, "\uC804\uCCB4 \uC74C\uB7C9", new Vector2(-210f, 132f), 18, new Color(0.9f, 0.95f, 0.86f, 1f));
        CreateLabel(panel.transform, "\uBC30\uACBD \uC74C\uC545", new Vector2(-210f, 72f), 18, new Color(0.9f, 0.95f, 0.86f, 1f));
        CreateLabel(panel.transform, "\uD6A8\uACFC\uC74C", new Vector2(-210f, 12f), 18, new Color(0.9f, 0.95f, 0.86f, 1f));
        CreateLabel(panel.transform, "\uD654\uBA74 \uBAA8\uB4DC", new Vector2(-210f, -60f), 18, new Color(0.9f, 0.95f, 0.86f, 1f));
        CreateLabel(panel.transform, "\uD574\uC0C1\uB3C4", new Vector2(-210f, -122f), 18, new Color(0.9f, 0.95f, 0.86f, 1f));

        masterVolumeSlider = CreateSlider(panel.transform, new Vector2(112f, 132f));
        bgmVolumeSlider = CreateSlider(panel.transform, new Vector2(112f, 72f));
        sfxVolumeSlider = CreateSlider(panel.transform, new Vector2(112f, 12f));
        fullscreenToggle = CreateToggle(panel.transform, new Vector2(84f, -60f), out fullscreenModeText);
        previousResolutionButton = CreateButton(panel.transform, "<", new Vector2(-40f, -122f), new Vector2(56f, 44f));
        resolutionText = CreateText(panel.transform, string.Empty, new Vector2(92f, -122f), 17, Color.white, new Vector2(190f, 38f));
        nextResolutionButton = CreateButton(panel.transform, ">", new Vector2(226f, -122f), new Vector2(56f, 44f));
        appliedScreenText = CreateText(panel.transform, string.Empty, new Vector2(0f, -174f), 14, new Color(0.82f, 0.9f, 0.78f, 1f), new Vector2(500f, 30f));
        CreateButton(panel.transform, "\uAE30\uBCF8\uAC12", new Vector2(0f, -214f), new Vector2(180f, 40f)).onClick.AddListener(ResetOptions);
        optionsConfirmButton = CreateButton(panel.transform, "\uD655\uC778", new Vector2(-140f, -268f), new Vector2(220f, 52f));
        optionsBackButton = CreateButton(panel.transform, "\uB4A4\uB85C", new Vector2(140f, -268f), new Vector2(220f, 52f));
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

        LoadPendingOptions();
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
            masterVolumeSlider.onValueChanged.AddListener(SetPendingMasterVolume);

        if (bgmVolumeSlider != null)
            bgmVolumeSlider.onValueChanged.AddListener(SetPendingBgmVolume);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(SetPendingSfxVolume);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(SetPendingFullscreen);

        if (previousResolutionButton != null)
            previousResolutionButton.onClick.AddListener(SelectPreviousResolution);

        if (nextResolutionButton != null)
            nextResolutionButton.onClick.AddListener(SelectNextResolution);

        if (optionsConfirmButton != null)
            optionsConfirmButton.onClick.AddListener(ConfirmOptions);

        if (optionsBackButton != null)
            optionsBackButton.onClick.AddListener(ShowPausePanel);
    }

    private void UnbindOptionsControls()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.RemoveListener(SetPendingMasterVolume);

        if (bgmVolumeSlider != null)
            bgmVolumeSlider.onValueChanged.RemoveListener(SetPendingBgmVolume);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.RemoveListener(SetPendingSfxVolume);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.RemoveListener(SetPendingFullscreen);

        if (previousResolutionButton != null)
            previousResolutionButton.onClick.RemoveListener(SelectPreviousResolution);

        if (nextResolutionButton != null)
            nextResolutionButton.onClick.RemoveListener(SelectNextResolution);

        if (optionsConfirmButton != null)
            optionsConfirmButton.onClick.RemoveListener(ConfirmOptions);

        if (optionsBackButton != null)
            optionsBackButton.onClick.RemoveListener(ShowPausePanel);
    }

    private void LoadPendingOptions()
    {
        pendingMasterVolume = GameOptions.MasterVolume;
        pendingBgmVolume = GameOptions.BgmVolume;
        pendingSfxVolume = GameOptions.SfxVolume;
        pendingFullscreen = GameOptions.IsFullscreen;
        pendingResolutionIndex = GameOptions.ResolutionIndex;
    }

    private void SetPendingMasterVolume(float value)
    {
        pendingMasterVolume = Mathf.Clamp01(value);
    }

    private void SetPendingBgmVolume(float value)
    {
        pendingBgmVolume = Mathf.Clamp01(value);
    }

    private void SetPendingSfxVolume(float value)
    {
        pendingSfxVolume = Mathf.Clamp01(value);
    }

    private void SetPendingFullscreen(bool value)
    {
        pendingFullscreen = value;
        RefreshOptionsControls();
    }

    private void SelectPreviousResolution()
    {
        GameSfx.Play(SfxType.UpgradeSelect);
        pendingResolutionIndex = (pendingResolutionIndex - 1 + GameOptions.ResolutionCount) % GameOptions.ResolutionCount;
        RefreshOptionsControls();
    }

    private void SelectNextResolution()
    {
        GameSfx.Play(SfxType.UpgradeSelect);
        pendingResolutionIndex = (pendingResolutionIndex + 1) % GameOptions.ResolutionCount;
        RefreshOptionsControls();
    }

    private void ResetOptions()
    {
        GameSfx.Play(SfxType.UpgradeSelect);
        pendingMasterVolume = GameOptions.DefaultMasterVolume;
        pendingBgmVolume = GameOptions.DefaultBgmVolume;
        pendingSfxVolume = GameOptions.DefaultSfxVolume;
        pendingFullscreen = GameOptions.DefaultFullscreen;
        pendingResolutionIndex = GameOptions.DefaultResolutionIndex;
        RefreshOptionsControls();
    }

    private void ConfirmOptions()
    {
        GameSfx.Play(SfxType.UpgradeSelect);
        GameOptions.ApplyOptions(pendingMasterVolume, pendingBgmVolume, pendingSfxVolume, pendingFullscreen, pendingResolutionIndex);
        RefreshOptionsControls();
        ShowPausePanel();
    }

    private void RefreshOptionsControls()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.SetValueWithoutNotify(pendingMasterVolume);

        if (bgmVolumeSlider != null)
            bgmVolumeSlider.SetValueWithoutNotify(pendingBgmVolume);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.SetValueWithoutNotify(pendingSfxVolume);

        if (fullscreenToggle != null)
            fullscreenToggle.SetIsOnWithoutNotify(pendingFullscreen);

        if (fullscreenModeText != null)
            fullscreenModeText.text = pendingFullscreen ? "\uC804\uCCB4 \uD654\uBA74" : "\uCC3D \uBAA8\uB4DC";

        if (resolutionText != null)
        {
            Vector2Int resolution = GameOptions.GetResolution(pendingResolutionIndex);
            resolutionText.text = $"{resolution.x} x {resolution.y}";
        }

        if (appliedScreenText != null)
            appliedScreenText.text = $"\uD604\uC7AC \uC801\uC6A9: {GameOptions.AppliedScreenInfo}";
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
        CreateText(
            buttonObject.transform,
            text,
            Vector2.zero,
            18,
            new Color(0.08f, 0.12f, 0.08f, 1f),
            new Vector2(Mathf.Max(1f, size.x - 12f), Mathf.Max(1f, size.y - 8f)));

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
        rectTransform.sizeDelta = new Vector2(260f, 28f);
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
        handleRect.sizeDelta = new Vector2(28f, 38f);

        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handle;
        slider.direction = Slider.Direction.LeftToRight;
        return slider;
    }

    private static Toggle CreateToggle(Transform parent, Vector2 position, out Text label)
    {
        GameObject toggleObject = new GameObject("Toggle");
        toggleObject.transform.SetParent(parent, false);

        RectTransform rectTransform = toggleObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(220f, 42f);
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
        checkRect.sizeDelta = new Vector2(30f, 30f);
        checkRect.anchoredPosition = new Vector2(22f, 0f);

        label = CreateText(toggleObject.transform, "\uC804\uCCB4 \uD654\uBA74", new Vector2(56f, 0f), 18, Color.white, new Vector2(150f, 34f));
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
