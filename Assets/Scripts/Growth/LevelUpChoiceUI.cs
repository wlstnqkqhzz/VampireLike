using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace VampireLike.Growth
{
    public class LevelUpChoiceUI : MonoBehaviour
    {
        private const string CanvasName = "Level Up Choice Canvas";
        private const string RootName = "Level Up Choice";

        private readonly string[] choiceLabels =
        {
            "공격 속도 강화",
            "투사체 피해 강화",
            "이동 속도 강화"
        };

        private GameObject choiceRoot;
        private bool isShowing;

        private void Awake()
        {
            EnsureUI();
            EnsureEventSystem();
            Hide();
        }

        public void Show(int level)
        {
            EnsureUI();
            isShowing = true;
            Time.timeScale = 0f;

            if (choiceRoot != null)
                choiceRoot.SetActive(true);

            Debug.Log($"Level Up Choice Opened: Level {level}");
        }

        private void Hide()
        {
            isShowing = false;

            if (choiceRoot != null)
                choiceRoot.SetActive(false);
        }

        private void SelectChoice(string choiceLabel)
        {
            if (!isShowing)
                return;

            Debug.Log($"Level Up Choice Selected: {choiceLabel}");
            Hide();
            Time.timeScale = 1f;
        }

        private void EnsureUI()
        {
            if (choiceRoot != null)
                return;

            Canvas canvas = CreateCanvas();
            choiceRoot = new GameObject(RootName);
            choiceRoot.transform.SetParent(canvas.transform, false);

            RectTransform rootRect = choiceRoot.AddComponent<RectTransform>();
            StretchToParent(rootRect);

            Image backdrop = choiceRoot.AddComponent<Image>();
            backdrop.color = new Color(0f, 0f, 0f, 0.55f);

            GameObject panel = new GameObject("Panel");
            panel.transform.SetParent(choiceRoot.transform, false);

            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(520f, 300f);
            panelRect.anchoredPosition = Vector2.zero;

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0.1f, 0.13f, 0.15f, 0.96f);

            CreateLabel(panel.transform, "레벨업", new Vector2(0f, 105f), 34, Color.white);

            for (int i = 0; i < choiceLabels.Length; i++)
            {
                string label = choiceLabels[i];
                Button button = CreateButton(panel.transform, label, new Vector2(0f, 40f - i * 68f));
                button.onClick.AddListener(() => SelectChoice(label));
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
            canvas.sortingOrder = 1100;

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

        private static void StretchToParent(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
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
            rectTransform.sizeDelta = new Vector2(420f, 52f);
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
            rectTransform.sizeDelta = new Vector2(380f, 52f);
            rectTransform.anchoredPosition = position;

            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.72f, 0.9f, 0.95f, 1f);

            Button button = buttonObject.AddComponent<Button>();
            CreateLabel(buttonObject.transform, text, Vector2.zero, 22, new Color(0.06f, 0.1f, 0.12f, 1f));

            return button;
        }
    }
}
