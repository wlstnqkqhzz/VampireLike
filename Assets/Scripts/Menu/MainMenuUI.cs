using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Menu
{
    /// <summary>
    /// 게임 시작 전에 캐릭터를 고르고 시작할 수 있는 간단한 메인 메뉴입니다.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        private const float PanelWidth = 760f;
        private const float PanelHeight = 520f;

        private GUIStyle titleStyle;
        private GUIStyle subtitleStyle;
        private GUIStyle cardStyle;
        private GUIStyle selectedCardStyle;
        private GUIStyle labelStyle;
        private GUIStyle descriptionStyle;
        private GUIStyle buttonStyle;
        private int selectedIndex;
        private bool hasStarted;

        public static bool IsOpen { get; private set; }

        private void Awake()
        {
            selectedIndex = CharacterSelection.SelectedIndex;
            OpenMenu();
        }

        private void OnDestroy()
        {
            if (!hasStarted)
                IsOpen = false;
        }

        private void OnGUI()
        {
            if (!IsOpen)
                return;

            EnsureStyles();
            DrawBackdrop();
            DrawMenuPanel();
        }

        private void OpenMenu()
        {
            IsOpen = true;
            GameState.SetMainMenuOpen(true);
            Time.timeScale = 0f;
        }

        private void StartGame()
        {
            CharacterSelection.Select(selectedIndex);
            ApplySelectedCharacter();
            hasStarted = true;
            IsOpen = false;
            GameState.SetMainMenuOpen(false);
            Time.timeScale = 1f;
            Destroy(gameObject);
        }

        private static void ApplySelectedCharacter()
        {
            GameObject player = GameObject.Find("Player");

            if (player == null)
                return;

            PlayerCharacterApplier applier = player.GetComponent<PlayerCharacterApplier>();

            if (applier == null)
                applier = player.AddComponent<PlayerCharacterApplier>();

            applier.ApplySelectedCharacter();
        }

        private void DrawBackdrop()
        {
            Color previousColor = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.72f);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = previousColor;
        }

        private void DrawMenuPanel()
        {
            Rect panelRect = new Rect(
                (Screen.width - PanelWidth) * 0.5f,
                (Screen.height - PanelHeight) * 0.5f,
                PanelWidth,
                PanelHeight);

            GUI.Box(panelRect, GUIContent.none, cardStyle);
            GUI.Label(new Rect(panelRect.x, panelRect.y + 34f, panelRect.width, 54f), "VampireLike", titleStyle);
            GUI.Label(new Rect(panelRect.x, panelRect.y + 88f, panelRect.width, 32f), "캐릭터를 선택하고 생존을 시작하세요", subtitleStyle);

            CharacterDefinition[] characters = CharacterSelection.Characters;
            float cardWidth = 300f;
            float cardHeight = 220f;
            float gap = 36f;
            float startX = panelRect.center.x - cardWidth - gap * 0.5f;
            float cardY = panelRect.y + 154f;

            for (int i = 0; i < characters.Length; i++)
            {
                Rect cardRect = new Rect(startX + (cardWidth + gap) * i, cardY, cardWidth, cardHeight);
                DrawCharacterCard(cardRect, characters[i], i);
            }

            GUI.enabled = true;
            Rect startButtonRect = new Rect(panelRect.center.x - 140f, panelRect.yMax - 86f, 280f, 48f);

            if (GUI.Button(startButtonRect, "게임 시작", buttonStyle))
                StartGame();
        }

        private void DrawCharacterCard(Rect cardRect, CharacterDefinition character, int index)
        {
            bool isSelected = selectedIndex == index;

            if (GUI.Button(cardRect, GUIContent.none, isSelected ? selectedCardStyle : cardStyle))
                selectedIndex = index;

            GUI.Label(new Rect(cardRect.x + 18f, cardRect.y + 22f, cardRect.width - 36f, 34f), character.DisplayName, labelStyle);
            GUI.Label(new Rect(cardRect.x + 18f, cardRect.y + 58f, cardRect.width - 36f, 26f), character.Role, subtitleStyle);
            GUI.Label(new Rect(cardRect.x + 24f, cardRect.y + 98f, cardRect.width - 48f, 68f), character.Description, descriptionStyle);

            string statText = $"이동 x{character.MoveSpeedMultiplier:0.00}\n공격 간격 x{character.AttackIntervalMultiplier:0.00}";

            if (character.BonusMaxHealth > 0)
                statText += $"\n최대 체력 +{character.BonusMaxHealth}";

            GUI.Label(new Rect(cardRect.x + 24f, cardRect.y + 164f, cardRect.width - 48f, 44f), statText, descriptionStyle);
        }

        private void EnsureStyles()
        {
            if (titleStyle != null)
                return;

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 42,
                fontStyle = FontStyle.Bold
            };
            titleStyle.normal.textColor = Color.white;

            subtitleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };
            subtitleStyle.normal.textColor = new Color(0.82f, 0.9f, 0.78f, 1f);

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 26,
                fontStyle = FontStyle.Bold
            };
            labelStyle.normal.textColor = Color.white;

            descriptionStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 15,
                wordWrap = true
            };
            descriptionStyle.normal.textColor = new Color(0.9f, 0.95f, 0.86f, 1f);

            cardStyle = new GUIStyle(GUI.skin.box);
            cardStyle.normal.background = MakeTexture(new Color(0.05f, 0.07f, 0.06f, 0.94f));

            selectedCardStyle = new GUIStyle(GUI.skin.box);
            selectedCardStyle.normal.background = MakeTexture(new Color(0.13f, 0.26f, 0.16f, 0.96f));

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold
            };
        }

        private static Texture2D MakeTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
    }
}
