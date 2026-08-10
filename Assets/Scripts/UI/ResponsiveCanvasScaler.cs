using UnityEngine;
using UnityEngine.UI;

namespace VampireLike.UI
{
    /// <summary>
    /// 화면 방향이 바뀌어도 동적으로 만든 UI가 눌려 보이지 않도록 Canvas 기준 해상도를 갱신합니다.
    /// </summary>
    [RequireComponent(typeof(CanvasScaler))]
    public class ResponsiveCanvasScaler : MonoBehaviour
    {
        private CanvasScaler canvasScaler;
        private int lastScreenWidth;
        private int lastScreenHeight;

        private void Awake()
        {
            canvasScaler = GetComponent<CanvasScaler>();
            Apply();
        }

        private void OnEnable()
        {
            Apply();
        }

        private void Update()
        {
            if (lastScreenWidth == Screen.width && lastScreenHeight == Screen.height)
                return;

            Apply();
        }

        private void Apply()
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            MobileSafeArea.ConfigureCanvasScaler(canvasScaler);
        }
    }
}
