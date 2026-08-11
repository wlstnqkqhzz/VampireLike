using UnityEngine;

namespace VampireLike.Settings
{
    public enum MobileOrientationMode
    {
        Auto = 0,
        Portrait = 1,
        Landscape = 2
    }

    /// <summary>
    /// 게임 옵션 값을 저장하고 시작 시 다시 적용합니다.
    /// </summary>
    public static class GameOptions
    {
        private const string MasterVolumeKey = "Options.MasterVolume";
        private const string BgmVolumeKey = "Options.BgmVolume";
        private const string SfxVolumeKey = "Options.SfxVolume";
        private const string FullscreenKey = "Options.Fullscreen";
        private const string ResolutionIndexKey = "Options.ResolutionIndex";
        private const string MobileOrientationKey = "Options.MobileOrientation";

        public const float DefaultMasterVolume = 0.8f;
        public const float DefaultBgmVolume = 0.8f;
        public const float DefaultSfxVolume = 0.8f;
        public const bool DefaultFullscreen = true;
        public const int DefaultResolutionIndex = 2;
        public const MobileOrientationMode DefaultMobileOrientation = MobileOrientationMode.Portrait;

        private static readonly Vector2Int[] SupportedResolutions =
        {
            new Vector2Int(1280, 720),
            new Vector2Int(1600, 900),
            new Vector2Int(1920, 1080),
            new Vector2Int(2560, 1440)
        };

        public static float MasterVolume { get; private set; } = DefaultMasterVolume;
        public static float BgmVolume { get; private set; } = DefaultBgmVolume;
        public static float SfxVolume { get; private set; } = DefaultSfxVolume;
        public static bool IsFullscreen { get; private set; } = DefaultFullscreen;
        public static int ResolutionIndex { get; private set; } = DefaultResolutionIndex;
        public static MobileOrientationMode MobileOrientation { get; private set; } = DefaultMobileOrientation;
        public static Vector2Int CurrentResolution => SupportedResolutions[ResolutionIndex];
        public static int ResolutionCount => SupportedResolutions.Length;
        public static string AppliedScreenInfo => IsMobileDisplayMode ? GetMobileOrientationText(MobileOrientation) : $"{Screen.width} x {Screen.height} / {GetScreenModeText()}";

        public static bool IsMobileDisplayMode
        {
            get
            {
#if UNITY_ANDROID || UNITY_IOS
                return true;
#else
                return Application.isMobilePlatform;
#endif
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadOptionsOnStart()
        {
            LoadAndApply();
        }

        public static void LoadAndApply()
        {
            Load();
            ApplyDisplayOptions();
        }

        public static Vector2Int GetResolution(int index)
        {
            return SupportedResolutions[Mathf.Clamp(index, 0, SupportedResolutions.Length - 1)];
        }

        public static string GetMobileOrientationText(MobileOrientationMode mode)
        {
            return mode switch
            {
                MobileOrientationMode.Auto => "자동 회전",
                MobileOrientationMode.Landscape => "가로 모드",
                _ => "세로 모드"
            };
        }

        public static MobileOrientationMode GetMobileOrientationByOffset(MobileOrientationMode mode, int offset)
        {
            const int count = 3;
            int index = ((int)mode + offset) % count;

            if (index < 0)
                index += count;

            return (MobileOrientationMode)index;
        }

        public static void SetMasterVolume(float volume)
        {
            MasterVolume = Mathf.Clamp01(volume);
            Save();
        }

        public static void SetBgmVolume(float volume)
        {
            BgmVolume = Mathf.Clamp01(volume);
            Save();
        }

        public static void SetSfxVolume(float volume)
        {
            SfxVolume = Mathf.Clamp01(volume);
            Save();
        }

        public static void SetFullscreen(bool fullscreen)
        {
            IsFullscreen = fullscreen;
            Save();
            ApplyDisplayOptions();
        }

        public static void SetResolutionIndex(int index)
        {
            ResolutionIndex = Mathf.Clamp(index, 0, SupportedResolutions.Length - 1);
            Save();
            ApplyDisplayOptions();
        }

        public static void SetMobileOrientation(MobileOrientationMode orientation)
        {
            MobileOrientation = orientation;
            Save();
            ApplyDisplayOptions();
        }

        public static void ApplyOptions(float masterVolume, float bgmVolume, float sfxVolume, bool fullscreen, int resolutionIndex)
        {
            ApplyOptions(masterVolume, bgmVolume, sfxVolume, fullscreen, resolutionIndex, MobileOrientation);
        }

        public static void ApplyOptions(float masterVolume, float bgmVolume, float sfxVolume, bool fullscreen, int resolutionIndex, MobileOrientationMode mobileOrientation)
        {
            MasterVolume = Mathf.Clamp01(masterVolume);
            BgmVolume = Mathf.Clamp01(bgmVolume);
            SfxVolume = Mathf.Clamp01(sfxVolume);
            IsFullscreen = fullscreen;
            ResolutionIndex = Mathf.Clamp(resolutionIndex, 0, SupportedResolutions.Length - 1);
            MobileOrientation = mobileOrientation;
            Save();
            ApplyDisplayOptions();
        }

        public static void ResetToDefaults()
        {
            ApplyOptions(DefaultMasterVolume, DefaultBgmVolume, DefaultSfxVolume, DefaultFullscreen, DefaultResolutionIndex, DefaultMobileOrientation);
        }

        public static void ApplyDisplayOptions()
        {
            if (IsMobileDisplayMode)
            {
                ApplyMobileOrientation();
                return;
            }

            Vector2Int resolution = CurrentResolution;
            Screen.SetResolution(resolution.x, resolution.y, IsFullscreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed);
        }

        private static void Load()
        {
            MasterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, DefaultMasterVolume);
            BgmVolume = PlayerPrefs.GetFloat(BgmVolumeKey, DefaultBgmVolume);
            SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, DefaultSfxVolume);
            IsFullscreen = PlayerPrefs.GetInt(FullscreenKey, DefaultFullscreen ? 1 : 0) == 1;
            ResolutionIndex = Mathf.Clamp(PlayerPrefs.GetInt(ResolutionIndexKey, DefaultResolutionIndex), 0, SupportedResolutions.Length - 1);
            MobileOrientation = (MobileOrientationMode)Mathf.Clamp(PlayerPrefs.GetInt(MobileOrientationKey, (int)DefaultMobileOrientation), 0, 2);
        }

        private static void Save()
        {
            PlayerPrefs.SetFloat(MasterVolumeKey, MasterVolume);
            PlayerPrefs.SetFloat(BgmVolumeKey, BgmVolume);
            PlayerPrefs.SetFloat(SfxVolumeKey, SfxVolume);
            PlayerPrefs.SetInt(FullscreenKey, IsFullscreen ? 1 : 0);
            PlayerPrefs.SetInt(ResolutionIndexKey, ResolutionIndex);
            PlayerPrefs.SetInt(MobileOrientationKey, (int)MobileOrientation);
            PlayerPrefs.Save();
        }

        private static void ApplyMobileOrientation()
        {
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = MobileOrientation != MobileOrientationMode.Portrait;
            Screen.autorotateToLandscapeRight = MobileOrientation != MobileOrientationMode.Portrait;

            Screen.orientation = MobileOrientation switch
            {
                MobileOrientationMode.Auto => ScreenOrientation.AutoRotation,
                MobileOrientationMode.Landscape => ScreenOrientation.LandscapeLeft,
                _ => ScreenOrientation.Portrait
            };
        }

        private static string GetScreenModeText()
        {
            if (!Screen.fullScreen)
                return "창 모드";

            return Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen ? "전체 화면" : "테두리 없는 전체 화면";
        }
    }
}
