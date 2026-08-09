using UnityEngine;
using UnityEngine.SceneManagement;
using VampireLike.Audio;
using VampireLike.Combat;
using VampireLike.Save;
using VampireLike.Settings;
using VampireLike.UI;

namespace VampireLike.Menu
{
    /// <summary>
    /// 硫붿씤 硫붾돱 ?ъ뿉????댄? ?붾㈃怨?罹먮┃???좏깮 ?붾㈃???쒖꽌?濡?愿由ы븳??
    /// ?꾩옱??鍮좊Ⅴ寃??뚯뒪?명븷 ???덈룄濡?利됱떆 紐⑤뱶 GUI濡?援ъ꽦?쒕떎.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        private const float TitlePanelWidth = 720f;
        private const float TitlePanelHeight = 520f;
        private const float CharacterPanelWidth = 860f;
        private const float CharacterPanelHeight = 640f;
        private const string GameSceneName = "SampleScene";
        private const string TitleBackgroundPath = "Menu/title_background";

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
        private GUIStyle optionLabelStyle;
        private GUIStyle optionValueStyle;
        private Texture2D darkTexture;
        private Texture2D panelTexture;
        private Texture2D cardTexture;
        private Texture2D selectedCardTexture;
        private Texture2D buttonTexture;
        private Texture2D secondaryButtonTexture;
        private Texture2D titleBackgroundTexture;

        private MenuScreen currentScreen = MenuScreen.Title;
        private int selectedIndex;
        private bool hasStarted;
        private string noticeMessage = string.Empty;
        private float noticeUntilTime;
        private float pendingMasterVolume;
        private float pendingBgmVolume;
        private float pendingSfxVolume;
        private bool pendingFullscreen;
        private int pendingResolutionIndex;

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
            else if (currentScreen == MenuScreen.CharacterSelect)
                DrawCharacterSelectScreen();
            else if (currentScreen == MenuScreen.Records)
                DrawRecordsScreen();
            else
                DrawOptionsScreen();
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

            Rect screenRect = new Rect(0f, 0f, Screen.width, Screen.height);

            if (titleBackgroundTexture != null)
                GUI.DrawTexture(screenRect, titleBackgroundTexture, ScaleMode.ScaleAndCrop);
            else
                GUI.DrawTexture(screenRect, darkTexture);

            GUI.color = new Color(0f, 0f, 0f, 0.42f);
            GUI.DrawTexture(screenRect, Texture2D.whiteTexture);

            // ??댄? ?붾㈃???덈Т ?됲룊?섏? ?딅룄濡?以묒븰???꾩＜ ?쏀븳 遺꾩쐞湲?鍮쏆쓣 源먮떎.
            GUI.color = new Color(0.12f, 0.28f, 0.24f, 0.16f);
            GUI.DrawTexture(new Rect(Screen.width * 0.5f - 360f, Screen.height * 0.5f - 280f, 720f, 560f), Texture2D.whiteTexture);
            GUI.color = previousColor;
        }

        private void DrawTitleScreen()
        {
            Rect panelRect = CenterRect(TitlePanelWidth, TitlePanelHeight);
            GUI.Box(panelRect, GUIContent.none, panelStyle);

            GUI.Label(new Rect(panelRect.x, panelRect.y + 50f, panelRect.width, 58f), "VampireLike", titleStyle);
            GUI.Label(new Rect(panelRect.x, panelRect.y + 110f, panelRect.width, 30f), "\uC5B4\uB460 \uC18D\uC5D0\uC11C \uB05D\uC5C6\uC774 \uBAB0\uB824\uB4DC\uB294 \uC801\uC744 \uBC84\uD2F0\uC138\uC694", subtitleStyle);

            Rect startButtonRect = new Rect(panelRect.center.x - 160f, panelRect.y + 190f, 320f, 52f);
            Rect recordButtonRect = new Rect(panelRect.center.x - 160f, panelRect.y + 258f, 320f, 48f);
            Rect optionButtonRect = new Rect(panelRect.center.x - 160f, panelRect.y + 320f, 320f, 48f);
            Rect quitButtonRect = new Rect(panelRect.center.x - 160f, panelRect.y + 382f, 320f, 48f);

            if (GUI.Button(startButtonRect, "\uAC8C\uC784 \uC2DC\uC791", buttonStyle))
            {
                PlayMenuSfx();
                noticeMessage = string.Empty;
                currentScreen = MenuScreen.CharacterSelect;
            }

            if (GUI.Button(recordButtonRect, "\uAE30\uB85D", secondaryButtonStyle))
            {
                PlayMenuSfx();
                noticeMessage = string.Empty;
                currentScreen = MenuScreen.Records;
            }

            if (GUI.Button(optionButtonRect, "\uC635\uC158", secondaryButtonStyle))
            {
                PlayMenuSfx();
                noticeMessage = string.Empty;
                LoadPendingOptions();
                currentScreen = MenuScreen.Options;
            }

            if (GUI.Button(quitButtonRect, "\uAC8C\uC784 \uC885\uB8CC", secondaryButtonStyle))
            {
                PlayMenuSfx();
                QuitGame();
            }

            string notice = Time.unscaledTime <= noticeUntilTime ? noticeMessage : "\uCE90\uB9AD\uD130\uB97C \uC120\uD0DD\uD558\uACE0 \uC0DD\uC874\uC744 \uC2DC\uC791\uD558\uC138\uC694";
            GUI.Label(new Rect(panelRect.x + 48f, panelRect.yMax - 74f, panelRect.width - 96f, 28f), notice, footerStyle);
        }

        private void DrawCharacterSelectScreen()
        {
            Rect panelRect = CenterRect(CharacterPanelWidth, CharacterPanelHeight);
            GUI.Box(panelRect, GUIContent.none, panelStyle);
            GUI.Label(new Rect(panelRect.x, panelRect.y + 28f, panelRect.width, 52f), "\uCE90\uB9AD\uD130 \uC120\uD0DD", titleStyle);
            GUI.Label(new Rect(panelRect.x, panelRect.y + 78f, panelRect.width, 30f), "\uD50C\uB808\uC774 \uC2A4\uD0C0\uC77C\uC5D0 \uB9DE\uB294 \uC0DD\uC874\uC790\uB97C \uACE0\uB974\uC138\uC694", subtitleStyle);

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
            {
                PlayMenuSfx();
                currentScreen = MenuScreen.Title;
            }

            if (GUI.Button(startButtonRect, "생존 시작", buttonStyle))
            {
                PlayMenuSfx();
                StartGame();
            }
        }

        private void DrawOptionsScreen()
        {
            Rect panelRect = CenterRect(TitlePanelWidth, 560f);
            GUI.Box(panelRect, GUIContent.none, panelStyle);
            GUI.Label(new Rect(panelRect.x, panelRect.y + 34f, panelRect.width, 52f), "\uC635\uC158", titleStyle);
            GUI.Label(new Rect(panelRect.x, panelRect.y + 82f, panelRect.width, 30f), "\uAC8C\uC784 \uD654\uBA74\uACFC \uC74C\uB7C9\uC744 \uC870\uC815\uD569\uB2C8\uB2E4", subtitleStyle);

            DrawVolumeOption(panelRect, "\uC804\uCCB4 \uC74C\uB7C9", ref pendingMasterVolume, 132f);
            DrawVolumeOption(panelRect, "\uBC30\uACBD \uC74C\uC545", ref pendingBgmVolume, 188f);
            DrawVolumeOption(panelRect, "\uD6A8\uACFC\uC74C", ref pendingSfxVolume, 244f);
            DrawFullscreenOption(panelRect, 308f);
            DrawResolutionOption(panelRect, 364f);
            GUI.Label(new Rect(panelRect.x + 88f, panelRect.y + 420f, panelRect.width - 176f, 28f), $"\uD604\uC7AC \uC801\uC6A9: {GameOptions.AppliedScreenInfo}", footerStyle);

            Rect resetRect = new Rect(panelRect.center.x - 80f, panelRect.yMax - 112f, 160f, 34f);
            Rect confirmRect = new Rect(panelRect.center.x - 238f, panelRect.yMax - 66f, 214f, 46f);
            Rect backRect = new Rect(panelRect.center.x + 24f, panelRect.yMax - 66f, 214f, 46f);

            if (GUI.Button(resetRect, "\uAE30\uBCF8\uAC12", secondaryButtonStyle))
            {
                PlayMenuSfx();
                LoadDefaultOptions();
            }

            if (GUI.Button(confirmRect, "\uD655\uC778", buttonStyle))
            {
                PlayMenuSfx();
                GameOptions.ApplyOptions(pendingMasterVolume, pendingBgmVolume, pendingSfxVolume, pendingFullscreen, pendingResolutionIndex);
                currentScreen = MenuScreen.Title;
            }

            if (GUI.Button(backRect, "\uB4A4\uB85C", buttonStyle))
            {
                PlayMenuSfx();
                currentScreen = MenuScreen.Title;
            }
        }

        private void DrawRecordsScreen()
        {
            Rect panelRect = CenterRect(TitlePanelWidth, TitlePanelHeight);
            GUI.Box(panelRect, GUIContent.none, panelStyle);
            GUI.Label(new Rect(panelRect.x, panelRect.y + 34f, panelRect.width, 52f), "\uAE30\uB85D", titleStyle);
            GUI.Label(new Rect(panelRect.x, panelRect.y + 84f, panelRect.width, 30f), "\uC9C0\uAE08\uAE4C\uC9C0\uC758 \uCD5C\uACE0 \uC0DD\uC874 \uAE30\uB85D\uC785\uB2C8\uB2E4", subtitleStyle);

            float cardWidth = 188f;
            float cardHeight = 240f;
            float gap = 22f;
            float startX = panelRect.center.x - cardWidth * 1.5f - gap;
            float cardY = panelRect.y + 142f;

            DrawRecordCard(new Rect(startX, cardY, cardWidth, cardHeight), "\uC804\uCCB4 \uCD5C\uACE0", HighScoreManager.GetOverallRecord());

            CharacterDefinition[] characters = CharacterSelection.Characters;
            for (int i = 0; i < characters.Length; i++)
            {
                CharacterDefinition character = characters[i];
                Rect cardRect = new Rect(startX + (cardWidth + gap) * (i + 1), cardY, cardWidth, cardHeight);
                DrawRecordCard(cardRect, character.DisplayName, HighScoreManager.GetCharacterRecord(character.Id));
            }

            Rect resetRect = new Rect(panelRect.center.x - 244f, panelRect.yMax - 76f, 220f, 46f);
            Rect backRect = new Rect(panelRect.center.x + 24f, panelRect.yMax - 76f, 220f, 46f);

            if (GUI.Button(resetRect, "\uAE30\uB85D \uCD08\uAE30\uD654", secondaryButtonStyle))
            {
                PlayMenuSfx();
                HighScoreManager.ResetAllRecords();
            }

            if (GUI.Button(backRect, "\uB4A4\uB85C", buttonStyle))
            {
                PlayMenuSfx();
                currentScreen = MenuScreen.Title;
            }
        }

        private void DrawRecordCard(Rect cardRect, string title, HighScoreRecord record)
        {
            GUI.Box(cardRect, GUIContent.none, cardStyle);
            GUI.Label(new Rect(cardRect.x + 12f, cardRect.y + 18f, cardRect.width - 24f, 32f), title, labelStyle);

            string recordText = record.HasAnyRecord
                ? $"생존 {FormatTime(record.SurvivalTime)}\n웨이브 {record.Wave}\n레벨 {record.Level}\n처치 {record.Kills}\n보스 {record.BossKills}\n경험치 {record.Experience}"
                : "기록 없음";

            GUI.Label(new Rect(cardRect.x + 18f, cardRect.y + 72f, cardRect.width - 36f, 140f), recordText, statStyle);
        }

        private void LoadPendingOptions()
        {
            pendingMasterVolume = GameOptions.MasterVolume;
            pendingBgmVolume = GameOptions.BgmVolume;
            pendingSfxVolume = GameOptions.SfxVolume;
            pendingFullscreen = GameOptions.IsFullscreen;
            pendingResolutionIndex = GameOptions.ResolutionIndex;
        }

        private void LoadDefaultOptions()
        {
            pendingMasterVolume = GameOptions.DefaultMasterVolume;
            pendingBgmVolume = GameOptions.DefaultBgmVolume;
            pendingSfxVolume = GameOptions.DefaultSfxVolume;
            pendingFullscreen = GameOptions.DefaultFullscreen;
            pendingResolutionIndex = GameOptions.DefaultResolutionIndex;
        }

        private void DrawVolumeOption(Rect panelRect, string label, ref float value, float yOffset)
        {
            Rect rowRect = new Rect(panelRect.x + 76f, panelRect.y + yOffset - 8f, panelRect.width - 152f, 44f);
            Rect labelRect = new Rect(rowRect.x + 22f, panelRect.y + yOffset, 160f, 30f);
            Rect sliderRect = new Rect(rowRect.x + 218f, panelRect.y + yOffset + 6f, 254f, 24f);
            Rect valueRect = new Rect(rowRect.xMax - 78f, panelRect.y + yOffset, 58f, 30f);

            GUI.Box(rowRect, GUIContent.none, cardStyle);
            GUI.Label(labelRect, label, optionLabelStyle);
            float nextValue = GUI.HorizontalSlider(sliderRect, value, 0f, 1f);
            GUI.Label(valueRect, $"{Mathf.RoundToInt(nextValue * 100f)}%", optionValueStyle);
            value = nextValue;
        }

        private void DrawFullscreenOption(Rect panelRect, float yOffset)
        {
            Rect rowRect = new Rect(panelRect.x + 76f, panelRect.y + yOffset - 8f, panelRect.width - 152f, 44f);
            Rect labelRect = new Rect(rowRect.x + 22f, panelRect.y + yOffset, 160f, 30f);
            Rect buttonRect = new Rect(rowRect.x + 218f, panelRect.y + yOffset - 4f, 214f, 40f);

            GUI.Box(rowRect, GUIContent.none, cardStyle);
            GUI.Label(labelRect, "화면 모드", optionLabelStyle);

            if (GUI.Button(buttonRect, pendingFullscreen ? "전체 화면" : "창 모드", secondaryButtonStyle))
            {
                PlayMenuSfx();
                pendingFullscreen = !pendingFullscreen;
            }
        }

        private void DrawResolutionOption(Rect panelRect, float yOffset)
        {
            Rect rowRect = new Rect(panelRect.x + 76f, panelRect.y + yOffset - 8f, panelRect.width - 152f, 44f);
            Rect labelRect = new Rect(rowRect.x + 22f, panelRect.y + yOffset, 160f, 30f);
            Rect previousRect = new Rect(rowRect.x + 218f, panelRect.y + yOffset - 4f, 48f, 40f);
            Rect valueRect = new Rect(rowRect.x + 276f, panelRect.y + yOffset, 156f, 30f);
            Rect nextRect = new Rect(rowRect.x + 442f, panelRect.y + yOffset - 4f, 48f, 40f);
            Vector2Int resolution = GameOptions.GetResolution(pendingResolutionIndex);

            GUI.Box(rowRect, GUIContent.none, cardStyle);
            GUI.Label(labelRect, "해상도", optionLabelStyle);

            if (GUI.Button(previousRect, "<", secondaryButtonStyle))
            {
                PlayMenuSfx();
                pendingResolutionIndex = (pendingResolutionIndex - 1 + GameOptions.ResolutionCount) % GameOptions.ResolutionCount;
            }

            GUI.Label(valueRect, $"{resolution.x} x {resolution.y}", optionValueStyle);

            if (GUI.Button(nextRect, ">", secondaryButtonStyle))
            {
                PlayMenuSfx();
                pendingResolutionIndex = (pendingResolutionIndex + 1) % GameOptions.ResolutionCount;
            }
        }

        private void DrawCharacterCard(Rect cardRect, CharacterDefinition character, int index)
        {
            bool isSelected = selectedIndex == index;

            if (GUI.Button(cardRect, GUIContent.none, isSelected ? selectedCardStyle : cardStyle))
            {
                PlayMenuSfx();
                selectedIndex = index;
            }

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

        private static void PlayMenuSfx()
        {
            GameSfx.Play(SfxType.UpgradeSelect);
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
            return MobileSafeArea.CenterRect(width, height);
        }

        private static string FormatTime(float seconds)
        {
            int totalSeconds = Mathf.FloorToInt(seconds);
            int minutes = totalSeconds / 60;
            int remainingSeconds = totalSeconds % 60;
            return $"{minutes:00}:{remainingSeconds:00}";
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
            titleBackgroundTexture = Resources.Load<Texture2D>(TitleBackgroundPath);

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

            optionLabelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };
            optionLabelStyle.normal.textColor = new Color(0.9f, 0.95f, 0.86f, 1f);

            optionValueStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };
            optionValueStyle.normal.textColor = Color.white;

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
            CharacterSelect,
            Records,
            Options
        }
    }
}
