using UnityEngine;
using UnityEngine.UI;

namespace VampireLike.UI
{
    /// <summary>
    /// 모바일 노치와 둥근 화면 모서리를 피하기 위한 Safe Area 보정 유틸리티입니다.
    /// </summary>
    public static class MobileSafeArea
    {
        private const float MinimumScreenSize = 1f;
        private static readonly Vector2 LandscapeReferenceResolution = new Vector2(1920f, 1080f);
        private static readonly Vector2 PortraitReferenceResolution = new Vector2(1080f, 1920f);

        public static bool IsPortrait => Screen.height > Screen.width;

        public static Vector2 CurrentReferenceResolution => IsPortrait ? PortraitReferenceResolution : LandscapeReferenceResolution;

        public static Rect GuiSafeArea
        {
            get
            {
                Rect safeArea = Screen.safeArea;
                return new Rect(
                    safeArea.xMin,
                    Screen.height - safeArea.yMax,
                    safeArea.width,
                    safeArea.height);
            }
        }

        public static float TopInset => Mathf.Max(0f, Screen.height - Screen.safeArea.yMax);
        public static float LeftInset => Mathf.Max(0f, Screen.safeArea.xMin);
        public static float RightInset => Mathf.Max(0f, Screen.width - Screen.safeArea.xMax);

        public static float HudTop(float baseMargin)
        {
            return TopInset + Mathf.Max(0f, baseMargin);
        }

        public static float HudLeft(float baseMargin)
        {
            return LeftInset + Mathf.Max(0f, baseMargin);
        }

        public static float HudRight(float baseMargin)
        {
            return RightInset + Mathf.Max(0f, baseMargin);
        }

        public static void ApplyTo(RectTransform rectTransform)
        {
            if (rectTransform == null)
                return;

            Rect safeArea = Screen.safeArea;
            Vector2 screenSize = new Vector2(Mathf.Max(MinimumScreenSize, Screen.width), Mathf.Max(MinimumScreenSize, Screen.height));

            rectTransform.anchorMin = new Vector2(safeArea.xMin / screenSize.x, safeArea.yMin / screenSize.y);
            rectTransform.anchorMax = new Vector2(safeArea.xMax / screenSize.x, safeArea.yMax / screenSize.y);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
        }

        public static Rect CenterRect(float width, float height, float padding = 24f)
        {
            Rect safeArea = GuiSafeArea;
            float targetWidth = Mathf.Min(width, Mathf.Max(1f, safeArea.width - padding * 2f));
            float targetHeight = Mathf.Min(height, Mathf.Max(1f, safeArea.height - padding * 2f));

            return new Rect(
                safeArea.x + (safeArea.width - targetWidth) * 0.5f,
                safeArea.y + (safeArea.height - targetHeight) * 0.5f,
                targetWidth,
                targetHeight);
        }

        public static float UiScale(float designWidth = 1920f, float designHeight = 1080f)
        {
            Rect safeArea = GuiSafeArea;
            float widthScale = safeArea.width / Mathf.Max(MinimumScreenSize, designWidth);
            float heightScale = safeArea.height / Mathf.Max(MinimumScreenSize, designHeight);
            return Mathf.Clamp(Mathf.Min(widthScale, heightScale), 0.72f, 1.25f);
        }

        public static void ConfigureCanvasScaler(CanvasScaler scaler)
        {
            if (scaler == null)
                return;

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = CurrentReferenceResolution;
            scaler.matchWidthOrHeight = 0.5f;
        }
    }
}
