using UnityEngine;
using UnityEngine.SceneManagement;
using VampireLike.Combat;

namespace VampireLike.Menu
{
    /// <summary>
    /// 메인 메뉴 씬에서 타이틀 화면과 캐릭터 선택 화면을 순서대로 관리한다.
    /// 현재는 빠르게 테스트할 수 있도록 즉시 모드 GUI로 구성한다.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        private const float TitlePanelWidth = 720f;
        private const float TitlePanelHeight = 520f;
        private const float CharacterPanelWidth = 860f;
        private const float CharacterPanelHeight = 640f;
        private const string GameSceneName = "SampleScene";

        private GUIStyle titleStyle;
        private GUIStyle subtitleStyle;
        private GUIStyle panelStyle;
        private GUIStyle cardStyle;
        private GUIStyle selectedCardStyle;
        private GUIStyle labelStyle;
        private GUIStyle descriptionStyle;
        private GUIStyle statStyle;
        private GUIStyle buttonStyle;
        private GUIStyle secondaryButtonStyle;
        private GUIStyle footerStyle;
        private Texture2D darkTexture;
        private Texture2D panelTexture;
        private Texture2D cardTexture;
        private Texture2D selectedCardTexture;
        private Texture2D buttonTexture;
        private Texture2D secondaryButtonTexture;

        private MenuScreen currentScreen = MenuScreen.Title;
        private int selectedIndex;
        private bool hasStarted;
        private string noticeMessage = string.Empty;
        private float noticeUntilTime;

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

            if (currentScreen == MenuScreen.Title)
                DrawTitleScreen();
            else
                DrawCharacterSelectScreen();
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
            hasStarted = true;
            IsOpen = false;
            GameState.SetMainMenuOpen(false);
            Time.timeScale = 1f;

            if (SceneManager.GetActiveScene().name == GameSceneName)
            {
                ApplySelectedCharacter();
                Destroy(gameObject);
                return;
            }

            SceneManager.LoadScene(GameSceneName);
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
            GUI.color = Color.white;
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), darkTexture);

            // 타이틀 화면이 너무 평평하지 않도록 중앙에 아주 약한 분위기 빛을 깐다.
            GUI.color = new Color(0.22f, 0.05f, 0.34f, 0.16f);
            GUI.DrawTexture(new Rect(Screen.width * 0.5f - 260f, Screen.height * 0.5f - 240f, 520f, 480f), Texture2D.whiteTexture);
            GUI.color = previousColor;
        }

        private void DrawTitleScreen()
        {
            Rect panelRect = CenterRect(TitlePanelWidth, TitlePanelHeight);
            GUI.Box(panelRect, GUIContent.none, panelStyle);

            GUI.Label(new Rect(panelRect.x, panelRect.y + 50f, panelRect.width, 58f), "VampireLike", titleStyle);
            GUI.Label(new Rect(panelRect.x, panelRect.y + 110f, panelRect.width, 30f), "어둠 속에서 끝없이 몰려드는 적을 버티세요", subtitleStyle);

            Rect startButtonRect = new Rect(panelRect.center.x - 160f, panelRect.y + 210f, 320f, 52f);
            Rect optionButtonRect = new Rect(panelRect.center.x - 160f, panelRect.y + 280f, 320f, 48f);
            Rect quitButtonRect = new Rect(panelRect.center.x - 160f, panelRect.y + 344f, 320f, 48f);

            if (GUI.Button(startButtonRect, "게임 시작", buttonStyle))
            {
                noticeMessage = string.Empty;
                currentScreen = MenuScreen.CharacterSelect;
            }

            if (GUI.Button(optionButtonRect, "옵션", secondaryButtonStyle))
                ShowNotice("옵션 메뉴는 다음 단계에서 추가할 예정입니다.");

            if (GUI.Button(quitButtonRect, "게임 종료", secondaryButtonStyle))
                QuitGame();

            string notice = Time.unscaledTime <= noticeUntilTime ? noticeMessage : "캐릭터를 선택하고 생존을 시작하세요";
            GUI.Label(new Rect(panelRect.x + 48f, panelRect.yMax - 74f, panelRect.width - 96f, 28f), notice, footerStyle);
        }

        private void DrawCharacterSelectScreen()
        {
            Rect panelRect = CenterRect(CharacterPanelWidth, CharacterPanelHeight);
            GUI.Box(panelRect, GUIContent.none, panelStyle);
            GUI.Label(new Rect(panelRect.x, panelRect.y + 28f, panelRect.width, 52f), "캐릭터 선택", titleStyle);
            GUI.Label(new Rect(panelRect.x, panelRect.y + 78f, panelRect.width, 30f), "플레이 스타일에 맞는 생존자를 고르세요", subtitleStyle);

            CharacterDefinition[] characters = CharacterSelection.Characters;
            float cardWidth = 340f;
            float cardHeight = 360f;
            float gap = 48f;
            float startX = panelRect.center.x - cardWidth - gap * 0.5f;
            float cardY = panelRect.y + 132f;

            for (int i = 0; i < characters.Length; i++)
            {
                Rect cardRect = new Rect(startX + (cardWidth + gap) * i, cardY, cardWidth, cardHeight);
                DrawCharacterCard(cardRect, characters[i], i);
            }

            Rect backButtonRect = new Rect(panelRect.center.x - 254f, panelRect.yMax - 72f, 220f, 48f);
            Rect startButtonRect = new Rect(panelRect.center.x + 34f, panelRect.yMax - 72f, 220f, 48f);

            if (GUI.Button(backButtonRect, "뒤로", secondaryButtonStyle))
                currentScreen = MenuScreen.Title;

            if (GUI.Button(startButtonRect, "생존 시작", buttonStyle))
                StartGame();
        }

        private void DrawCharacterCard(Rect cardRect, CharacterDefinition character, int index)
        {
            bool isSelected = selectedIndex == index;

            if (GUI.Button(cardRect, GUIContent.none, isSelected ? selectedCardStyle : cardStyle))
                selectedIndex = index;

            GUI.Label(new Rect(cardRect.x + 18f, cardRect.y + 22f, cardRect.width - 36f, 34f), character.DisplayName, labelStyle);
            GUI.Label(new Rect(cardRect.x + 18f, cardRect.y + 58f, cardRect.width - 36f, 26f), character.Role, subtitleStyle);
            GUI.Label(new Rect(cardRect.x + 26f, cardRect.y + 96f, cardRect.width - 52f, 110f), character.Description, descriptionStyle);

            string statText = $"이동 x{character.MoveSpeedMultiplier:0.00}\n공격 간격 x{character.AttackIntervalMultiplier:0.00}\n투사체 피해 x{character.ProjectileDamageMultiplier:0.00}\n최대 레벨 {character.MaxPlayerLevel}";

            if (character.BonusProjectileCount > 0)
                statText += $"\n투사체 +{character.BonusProjectileCount}";

            if (character.BonusMaxHealth > 0)
                statText += $"\n최대 체력 +{character.BonusMaxHealth}";

            GUI.Label(new Rect(cardRect.x + 26f, cardRect.y + 222f, cardRect.width - 52f, 112f), statText, statStyle);
        }

        private void ShowNotice(string message)
        {
            noticeMessage = message;
            noticeUntilTime = Time.unscaledTime + 2f;
        }

        private static void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private static Rect CenterRect(float width, float height)
        {
            return new Rect(
                (Screen.width - width) * 0.5f,
                (Screen.height - height) * 0.5f,
                width,
                height);
        }

        private void EnsureStyles()
        {
            if (titleStyle != null)
                return;

            darkTexture = MakeTexture(new Color(0.01f, 0.015f, 0.012f, 0.78f));
            panelTexture = MakeTexture(new Color(0.035f, 0.045f, 0.04f, 0.94f));
            cardTexture = MakeTexture(new Color(0.055f, 0.075f, 0.06f, 0.96f));
            selectedCardTexture = MakeTexture(new Color(0.12f, 0.22f, 0.15f, 0.98f));
            buttonTexture = MakeTexture(new Color(0.78f, 0.88f, 0.7f, 1f));
            secondaryButtonTexture = MakeTexture(new Color(0.12f, 0.14f, 0.12f, 1f));

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
                fontSize = 17,
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

            statStyle = new GUIStyle(descriptionStyle)
            {
                fontSize = 14
            };

            footerStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                wordWrap = true
            };
            footerStyle.normal.textColor = new Color(0.82f, 0.9f, 0.78f, 1f);

            panelStyle = new GUIStyle(GUI.skin.box);
            panelStyle.normal.background = panelTexture;

            cardStyle = new GUIStyle(GUI.skin.box);
            cardStyle.normal.background = cardTexture;

            selectedCardStyle = new GUIStyle(GUI.skin.box);
            selectedCardStyle.normal.background = selectedCardTexture;

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 20,
                fontStyle = FontStyle.Bold
            };
            buttonStyle.normal.background = buttonTexture;
            buttonStyle.normal.textColor = new Color(0.04f, 0.07f, 0.04f, 1f);

            secondaryButtonStyle = new GUIStyle(buttonStyle);
            secondaryButtonStyle.normal.background = secondaryButtonTexture;
            secondaryButtonStyle.normal.textColor = new Color(0.9f, 0.95f, 0.86f, 1f);
        }

        private static Texture2D MakeTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        private enum MenuScreen
        {
            Title,
            CharacterSelect
        }
    }
}
