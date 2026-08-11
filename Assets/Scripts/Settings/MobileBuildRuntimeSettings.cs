using UnityEngine;

namespace VampireLike.Settings
{
    /// <summary>
    /// 모바일 빌드에서 기본 성능과 화면 회전 기준을 맞춥니다.
    /// </summary>
    public static class MobileBuildRuntimeSettings
    {
        private const int MobileTargetFrameRate = 60;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Apply()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = MobileTargetFrameRate;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

#if UNITY_ANDROID || UNITY_IOS
            Screen.orientation = ScreenOrientation.Portrait;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
#endif
        }
    }
}
