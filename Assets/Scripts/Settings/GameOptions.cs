using UnityEngine;

namespace VampireLike.Settings
{
    /// <summary>
    /// 게임 옵션 값을 PlayerPrefs에 저장하고, 시작 시 다시 적용합니다.
    /// </summary>
    public static class GameOptions
    {
        private const string MasterVolumeKey = "Options.MasterVolume";
        private const string SfxVolumeKey = "Options.SfxVolume";
        private const string FullscreenKey = "Options.Fullscreen";
        private const string ResolutionIndexKey = "Options.ResolutionIndex";

        private static readonly Vector2Int[] SupportedResolutions =
        {
            new Vector2Int(1280, 720),
            new Vector2Int(1600, 900),
            new Vector2Int(1920, 1080),
            new Vector2Int(2560, 1440)
        };

        public static float MasterVolume { get; private set; } = 0.8f;
        public static float SfxVolume { get; private set; } = 0.8f;
        public static bool IsFullscreen { get; private set; } = true;
        public static int ResolutionIndex { get; private set; } = 2;
        public static Vector2Int CurrentResolution => SupportedResolutions[ResolutionIndex];
        public static int ResolutionCount => SupportedResolutions.Length;
        public static string AppliedScreenInfo => $"{Screen.width} x {Screen.height} / {GetScreenModeText()}";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadOptionsOnStart()
        {
            Load();
            ApplyScreenOptions();
        }

        public static Vector2Int GetResolution(int index)
        {
            return SupportedResolutions[Mathf.Clamp(index, 0, SupportedResolutions.Length - 1)];
        }

        public static void SetMasterVolume(float volume)
        {
            MasterVolume = Mathf.Clamp01(volume);
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
            ApplyScreenOptions();
        }

        public static void SetResolutionIndex(int index)
        {
            ResolutionIndex = Mathf.Clamp(index, 0, SupportedResolutions.Length - 1);
            Save();
            ApplyScreenOptions();
        }

        public static void ResetToDefaults()
        {
            MasterVolume = 0.8f;
            SfxVolume = 0.8f;
            IsFullscreen = true;
            ResolutionIndex = 2;
            Save();
            ApplyScreenOptions();
        }

        private static void Load()
        {
            MasterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 0.8f);
            SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 0.8f);
            IsFullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) == 1;
            ResolutionIndex = Mathf.Clamp(PlayerPrefs.GetInt(ResolutionIndexKey, 2), 0, SupportedResolutions.Length - 1);
        }

        private static void Save()
        {
            PlayerPrefs.SetFloat(MasterVolumeKey, MasterVolume);
            PlayerPrefs.SetFloat(SfxVolumeKey, SfxVolume);
            PlayerPrefs.SetInt(FullscreenKey, IsFullscreen ? 1 : 0);
            PlayerPrefs.SetInt(ResolutionIndexKey, ResolutionIndex);
            PlayerPrefs.Save();
        }

        private static void ApplyScreenOptions()
        {
            Vector2Int resolution = CurrentResolution;
            Screen.SetResolution(resolution.x, resolution.y, IsFullscreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed);
        }

        private static string GetScreenModeText()
        {
            if (!Screen.fullScreen)
                return "창 모드";

            return Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen ? "전체 화면" : "테두리 없는 전체 화면";
        }
    }
}
