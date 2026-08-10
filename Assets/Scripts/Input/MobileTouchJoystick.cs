using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VampireLike.Combat;
using VampireLike.UI;

namespace VampireLike.Mobile
{
    /// <summary>
    /// 모바일에서 터치한 위치에 나타나는 동적 이동 조이스틱입니다.
    /// </summary>
    public class MobileTouchJoystick : MonoBehaviour
    {
        private const string JoystickName = "Mobile Touch Joystick";
        private const string CanvasName = "Mobile Joystick Canvas";
        private const int CircleTextureSize = 128;

        [SerializeField]
        private bool enableInEditor = true;

        [SerializeField]
        private float maxRadius = 96f;

        [SerializeField]
        private float portraitMaxRadius = 78f;

        [SerializeField]
        private float deadZone = 0.16f;

        [SerializeField]
        private float safeAreaMargin = 24f;

        [SerializeField]
        private float portraitTopReservedArea = 132f;

        [SerializeField]
        private Vector2 backgroundSize = new Vector2(168f, 168f);

        [SerializeField]
        private Vector2 portraitBackgroundSize = new Vector2(146f, 146f);

        [SerializeField]
        private Vector2 knobSize = new Vector2(72f, 72f);

        [SerializeField]
        private Vector2 portraitKnobSize = new Vector2(62f, 62f);

        [SerializeField]
        private Color backgroundColor = new Color(0.08f, 0.12f, 0.16f, 0.38f);

        [SerializeField]
        private Color rimColor = new Color(0.65f, 0.9f, 1f, 0.42f);

        [SerializeField]
        private Color knobColor = new Color(0.75f, 0.92f, 1f, 0.72f);

        private static MobileTouchJoystick instance;
        private static Vector2 currentInput;
        private static bool hasActiveInput;

        private RectTransform canvasRect;
        private RectTransform rootRect;
        private RectTransform rimRect;
        private RectTransform knobRect;
        private int activeTouchId = -1;
        private Vector2 startScreenPosition;
        private bool isTrackingMouse;

        public static Vector2 MoveInput => currentInput;
        public static bool HasActiveInput => hasActiveInput;

        public static void EnsureExists()
        {
            if (instance != null)
                return;

            GameObject joystickObject = new GameObject(JoystickName);
            instance = joystickObject.AddComponent<MobileTouchJoystick>();
            DontDestroyOnLoad(joystickObject);
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            CreateVisuals();
            HideVisuals();
        }

        private void Update()
        {
            if (!ShouldAcceptInput())
            {
                ReleaseInput();
                return;
            }

            UpdateTouchInput();

#if UNITY_EDITOR
            if (enableInEditor)
                UpdateMouseInput();
#endif
        }

        private bool ShouldAcceptInput()
        {
            if (GameState.IsGameOver || GameState.IsMainMenuOpen || Time.timeScale <= 0f)
                return false;

#if UNITY_EDITOR
            if (enableInEditor)
                return true;
#endif

            return Touchscreen.current != null;
        }

        private void UpdateTouchInput()
        {
            Touchscreen touchscreen = Touchscreen.current;

            if (touchscreen == null)
                return;

            TouchControl touch = touchscreen.primaryTouch;
            bool isPressed = touch.press.isPressed;
            UnityEngine.InputSystem.TouchPhase phase = touch.phase.ReadValue();
            Vector2 screenPosition = touch.position.ReadValue();

            if (isPressed && activeTouchId < 0)
            {
                int touchId = touch.touchId.ReadValue();

                if (IsPointerOverUi(touchId))
                    return;

                BeginInput(screenPosition);
                activeTouchId = touchId;
                return;
            }

            if (activeTouchId < 0)
                return;

            if (!isPressed || phase == UnityEngine.InputSystem.TouchPhase.Ended || phase == UnityEngine.InputSystem.TouchPhase.Canceled)
            {
                ReleaseInput();
                return;
            }

            UpdateInput(screenPosition);
        }

#if UNITY_EDITOR
        private void UpdateMouseInput()
        {
            Mouse mouse = Mouse.current;

            if (mouse == null || Touchscreen.current != null)
                return;

            Vector2 screenPosition = mouse.position.ReadValue();

            if (mouse.leftButton.wasPressedThisFrame)
            {
                if (IsPointerOverUi())
                    return;

                isTrackingMouse = true;
                BeginInput(screenPosition);
                return;
            }

            if (!isTrackingMouse)
                return;

            if (mouse.leftButton.wasReleasedThisFrame)
            {
                isTrackingMouse = false;
                ReleaseInput();
                return;
            }

            if (mouse.leftButton.isPressed)
                UpdateInput(screenPosition);
        }
#endif

        private void BeginInput(Vector2 screenPosition)
        {
            UpdateVisualSizes();
            startScreenPosition = ClampScreenPositionToSafeArea(screenPosition);
            hasActiveInput = true;
            SetVisualPosition(rootRect, startScreenPosition);
            SetVisualPosition(knobRect, startScreenPosition);
            ShowVisuals();
            UpdateInput(screenPosition);
        }

        private void UpdateInput(Vector2 screenPosition)
        {
            float currentMaxRadius = GetCurrentMaxRadius();
            Vector2 delta = Vector2.ClampMagnitude(screenPosition - startScreenPosition, currentMaxRadius);
            Vector2 normalized = delta / currentMaxRadius;

            if (normalized.magnitude < deadZone)
                normalized = Vector2.zero;

            currentInput = normalized;
            SetVisualPosition(knobRect, startScreenPosition + delta);
        }

        private Vector2 ClampScreenPositionToSafeArea(Vector2 screenPosition)
        {
            Rect safeArea = Screen.safeArea;
            Vector2 currentBackgroundSize = GetCurrentBackgroundSize();
            float margin = Mathf.Max(GetCurrentMaxRadius(), Mathf.Max(currentBackgroundSize.x, currentBackgroundSize.y) * 0.5f) + safeAreaMargin;
            float topReservedArea = MobileSafeArea.IsPortrait ? portraitTopReservedArea : 0f;

            if (safeArea.width <= margin * 2f || safeArea.height <= margin * 2f + topReservedArea)
                return screenPosition;

            return new Vector2(
                Mathf.Clamp(screenPosition.x, safeArea.xMin + margin, safeArea.xMax - margin),
                Mathf.Clamp(screenPosition.y, safeArea.yMin + margin, safeArea.yMax - margin - topReservedArea));
        }

        private static bool IsPointerOverUi(int pointerId = -1)
        {
            if (EventSystem.current == null)
                return false;

            return pointerId >= 0
                ? EventSystem.current.IsPointerOverGameObject(pointerId)
                : EventSystem.current.IsPointerOverGameObject();
        }

        private void ReleaseInput()
        {
            activeTouchId = -1;
            isTrackingMouse = false;
            hasActiveInput = false;
            currentInput = Vector2.zero;
            HideVisuals();
        }

        private void CreateVisuals()
        {
            GameObject canvasObject = new GameObject(CanvasName);
            canvasObject.transform.SetParent(transform, false);

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 950;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            MobileSafeArea.ConfigureCanvasScaler(scaler);
            canvasObject.AddComponent<ResponsiveCanvasScaler>();

            canvasRect = canvasObject.GetComponent<RectTransform>();

            rootRect = CreateImage("Joystick Root", canvasObject.transform, CreateCircleSprite(0.5f), backgroundColor, backgroundSize);
            rimRect = CreateImage("Joystick Rim", rootRect, CreateRingSprite(), rimColor, backgroundSize);
            knobRect = CreateImage("Joystick Knob", canvasObject.transform, CreateCircleSprite(0.5f), knobColor, knobSize);
            UpdateVisualSizes();
        }

        private float GetCurrentMaxRadius()
        {
            return MobileSafeArea.IsPortrait ? portraitMaxRadius : maxRadius;
        }

        private Vector2 GetCurrentBackgroundSize()
        {
            return MobileSafeArea.IsPortrait ? portraitBackgroundSize : backgroundSize;
        }

        private Vector2 GetCurrentKnobSize()
        {
            return MobileSafeArea.IsPortrait ? portraitKnobSize : knobSize;
        }

        private void UpdateVisualSizes()
        {
            Vector2 currentBackgroundSize = GetCurrentBackgroundSize();
            Vector2 currentKnobSize = GetCurrentKnobSize();

            if (rootRect != null)
                rootRect.sizeDelta = currentBackgroundSize;

            if (rimRect != null)
                rimRect.sizeDelta = currentBackgroundSize;

            if (knobRect != null)
                knobRect.sizeDelta = currentKnobSize;
        }

        private RectTransform CreateImage(string objectName, Transform parent, Sprite sprite, Color color, Vector2 size)
        {
            GameObject imageObject = new GameObject(objectName);
            imageObject.transform.SetParent(parent, false);

            RectTransform rectTransform = imageObject.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            Image image = imageObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.raycastTarget = false;
            return rectTransform;
        }

        private void SetVisualPosition(RectTransform rectTransform, Vector2 screenPosition)
        {
            if (rectTransform == null || canvasRect == null)
                return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, null, out Vector2 localPoint);
            rectTransform.anchoredPosition = localPoint;
        }

        private void ShowVisuals()
        {
            if (rootRect != null)
                rootRect.gameObject.SetActive(true);

            if (rimRect != null)
                rimRect.gameObject.SetActive(true);

            if (knobRect != null)
                knobRect.gameObject.SetActive(true);
        }

        private void HideVisuals()
        {
            if (rootRect != null)
                rootRect.gameObject.SetActive(false);

            if (rimRect != null)
                rimRect.gameObject.SetActive(false);

            if (knobRect != null)
                knobRect.gameObject.SetActive(false);
        }

        private static Sprite CreateCircleSprite(float fillRadius)
        {
            Texture2D texture = new Texture2D(CircleTextureSize, CircleTextureSize, TextureFormat.RGBA32, false);
            Color32[] pixels = new Color32[CircleTextureSize * CircleTextureSize];
            Vector2 center = new Vector2((CircleTextureSize - 1) * 0.5f, (CircleTextureSize - 1) * 0.5f);
            float radius = CircleTextureSize * fillRadius;
            float feather = CircleTextureSize * 0.045f;

            for (int y = 0; y < CircleTextureSize; y++)
            {
                for (int x = 0; x < CircleTextureSize; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    float alpha = Mathf.Clamp01((radius - distance) / feather);
                    pixels[y * CircleTextureSize + x] = new Color32(255, 255, 255, (byte)(alpha * 255f));
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, CircleTextureSize, CircleTextureSize), new Vector2(0.5f, 0.5f), 100f);
        }

        private static Sprite CreateRingSprite()
        {
            Texture2D texture = new Texture2D(CircleTextureSize, CircleTextureSize, TextureFormat.RGBA32, false);
            Color32[] pixels = new Color32[CircleTextureSize * CircleTextureSize];
            Vector2 center = new Vector2((CircleTextureSize - 1) * 0.5f, (CircleTextureSize - 1) * 0.5f);
            float outerRadius = CircleTextureSize * 0.49f;
            float innerRadius = CircleTextureSize * 0.43f;
            float feather = CircleTextureSize * 0.035f;

            for (int y = 0; y < CircleTextureSize; y++)
            {
                for (int x = 0; x < CircleTextureSize; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    float outerAlpha = Mathf.Clamp01((outerRadius - distance) / feather);
                    float innerAlpha = Mathf.Clamp01((distance - innerRadius) / feather);
                    float alpha = outerAlpha * innerAlpha;
                    pixels[y * CircleTextureSize + x] = new Color32(255, 255, 255, (byte)(alpha * 255f));
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, CircleTextureSize, CircleTextureSize), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
