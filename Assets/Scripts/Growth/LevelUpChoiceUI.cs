using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using VampireLike.Audio;
using VampireLike.UI;

namespace VampireLike.Growth
{
    /// <summary>
    /// ?뚮젅?댁뼱 ?덈꺼????媛뺥솕 ?좏깮吏 3媛쒕? ?붾㈃??蹂댁뿬二쇨퀬, ?좏깮??媛뺥솕瑜??곸슜?쒕떎.
    /// </summary>
    public class LevelUpChoiceUI : MonoBehaviour
    {
        private const string CanvasName = "Level Up Choice Canvas";
        private const string RootName = "Level Up Choice";
        private const int ChoiceCount = 3;

        private GameObject choiceRoot;
        private Button[] choiceButtons;
        private Image[] choiceButtonImages;
        private Text[] choiceTexts;
        private bool isShowing;
        private PlayerUpgradeController upgradeController;
        private List<PlayerUpgradeController.UpgradeChoice> currentChoices = new List<PlayerUpgradeController.UpgradeChoice>();

        private static readonly Color NormalButtonColor = new Color(0.72f, 0.9f, 0.95f, 1f);
        private static readonly Color NormalTextColor = new Color(0.06f, 0.1f, 0.12f, 1f);
        private static readonly Color SpecialButtonColor = new Color(0.34f, 0.18f, 0.58f, 1f);
        private static readonly Color SpecialTextColor = new Color(1f, 0.88f, 0.42f, 1f);
        private static readonly Color KaelButtonColor = new Color(0.16f, 0.1f, 0.24f, 1f);
        private static readonly Color KaelTextColor = new Color(1f, 0.78f, 0.35f, 1f);
        private static readonly Color SeleneButtonColor = new Color(0.08f, 0.2f, 0.32f, 1f);
        private static readonly Color SeleneTextColor = new Color(0.74f, 0.95f, 1f, 1f);

        private void Awake()
        {
            upgradeController = GetComponent<PlayerUpgradeController>();

            if (upgradeController == null)
                upgradeController = gameObject.AddComponent<PlayerUpgradeController>();

            EnsureUI();
            EnsureEventSystem();
            Hide();
        }

        private void Update()
        {
            if (!isShowing || Keyboard.current == null)
                return;

            if (Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.numpad1Key.wasPressedThisFrame)
                SelectChoice(0);
            else if (Keyboard.current.digit2Key.wasPressedThisFrame || Keyboard.current.numpad2Key.wasPressedThisFrame)
                SelectChoice(1);
            else if (Keyboard.current.digit3Key.wasPressedThisFrame || Keyboard.current.numpad3Key.wasPressedThisFrame)
                SelectChoice(2);
        }

        public void Show(int level)
        {
            EnsureUI();

            // 媛뺥솕 而⑦듃濡ㅻ윭?먯꽌 ?꾩옱 ?좏깮 媛?ν븳 媛뺥솕 3媛쒕? ?쒕뜡?쇰줈 諛쏆븘?⑤떎.
            currentChoices = upgradeController.GetRandomChoices(ChoiceCount, level);

            if (currentChoices.Count == 0)
                return;

            for (int i = 0; i < choiceButtons.Length; i++)
            {
                bool hasChoice = i < currentChoices.Count;
                choiceButtons[i].gameObject.SetActive(hasChoice);

                if (!hasChoice)
                    continue;

                choiceTexts[i].text = $"{i + 1}. {currentChoices[i].ButtonText}";
                ApplyChoiceStyle(i, currentChoices[i].Definition);
                int choiceIndex = i;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => SelectChoice(choiceIndex));
            }

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

        private void SelectChoice(int choiceIndex)
        {
            if (!isShowing || choiceIndex < 0 || choiceIndex >= currentChoices.Count)
                return;

            GameSfx.Play(SfxType.UpgradeSelect);
            upgradeController.ApplyUpgrade(currentChoices[choiceIndex].Definition);
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
            panelRect.sizeDelta = new Vector2(680f, 430f);
            panelRect.anchoredPosition = Vector2.zero;

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0.1f, 0.13f, 0.15f, 0.96f);

            CreateLabel(panel.transform, "레벨 업", new Vector2(0f, 158f), 38, Color.white, new Vector2(560f, 56f));

            choiceButtons = new Button[ChoiceCount];
            choiceButtonImages = new Image[ChoiceCount];
            choiceTexts = new Text[ChoiceCount];

            for (int i = 0; i < ChoiceCount; i++)
            {
                Button button = CreateButton(panel.transform, new Vector2(0f, 72f - i * 92f), out Text buttonText, out Image buttonImage);
                choiceButtons[i] = button;
                choiceButtonImages[i] = buttonImage;
                choiceTexts[i] = buttonText;
            }
        }

        private void ApplyChoiceStyle(int index, UpgradeDefinition definition)
        {
            bool isCharacterUpgrade = definition != null && definition.IsCharacterExclusiveUpgrade;
            bool isSpecialUpgrade = definition != null && definition.IsSpecialUpgrade;

            if (choiceButtonImages != null && index < choiceButtonImages.Length && choiceButtonImages[index] != null)
            {
                if (isCharacterUpgrade)
                    choiceButtonImages[index].color = GetCharacterButtonColor(definition);
                else
                    choiceButtonImages[index].color = isSpecialUpgrade ? SpecialButtonColor : NormalButtonColor;
            }

            if (choiceTexts == null || index >= choiceTexts.Length || choiceTexts[index] == null)
                return;

            if (isCharacterUpgrade)
                choiceTexts[index].color = GetCharacterTextColor(definition);
            else
                choiceTexts[index].color = isSpecialUpgrade ? SpecialTextColor : NormalTextColor;

            choiceTexts[index].fontStyle = isCharacterUpgrade || isSpecialUpgrade ? FontStyle.Bold : FontStyle.Normal;
        }

        private static Color GetCharacterButtonColor(UpgradeDefinition definition)
        {
            if (definition != null && string.Equals(definition.RequiredCharacterId, "selene", System.StringComparison.OrdinalIgnoreCase))
                return SeleneButtonColor;

            return KaelButtonColor;
        }

        private static Color GetCharacterTextColor(UpgradeDefinition definition)
        {
            if (definition != null && string.Equals(definition.RequiredCharacterId, "selene", System.StringComparison.OrdinalIgnoreCase))
                return SeleneTextColor;

            return KaelTextColor;
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
            MobileSafeArea.ConfigureCanvasScaler(scaler);
            canvasObject.AddComponent<ResponsiveCanvasScaler>();

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

        private static void StretchToParent(RectTransform rectTransform)
        {
            MobileSafeArea.ApplyTo(rectTransform);
        }

        private static Text CreateLabel(Transform parent, string text, Vector2 position, int fontSize, Color color, Vector2 size)
        {
            GameObject labelObject = new GameObject(text);
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

        private static Button CreateButton(Transform parent, Vector2 position, out Text buttonText, out Image image)
        {
            GameObject buttonObject = new GameObject("Upgrade Choice Button");
            buttonObject.transform.SetParent(parent, false);

            RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(560f, 74f);
            rectTransform.anchoredPosition = position;

            image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.72f, 0.9f, 0.95f, 1f);

            Button button = buttonObject.AddComponent<Button>();
            buttonText = CreateLabel(buttonObject.transform, string.Empty, Vector2.zero, 22, new Color(0.06f, 0.1f, 0.12f, 1f), new Vector2(520f, 66f));
            return button;
        }
    }
}
