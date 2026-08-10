using UnityEngine;
using UnityEngine.SceneManagement;
using VampireLike.World;

namespace VampireLike.CameraSystem
{
    /// <summary>
    /// 모바일 가로 화면비에서 카메라가 너무 넓게 보이지 않도록 시야 크기를 보정합니다.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class MobileCameraViewport : MonoBehaviour
    {
        private const string GameSceneName = "SampleScene";

        [SerializeField]
        private float referenceAspect = 16f / 9f;

        [SerializeField]
        private float portraitReferenceAspect = 9f / 16f;

        [SerializeField]
        private float referenceOrthographicSize = 5f;

        [SerializeField]
        private float minimumOrthographicSize = 4.1f;

        [SerializeField]
        private float maximumOrthographicSize = 5.2f;

        [SerializeField]
        private float maximumPortraitOrthographicSize = 7.4f;

        [SerializeField]
        private float mapEdgePadding = 0.1f;

        private Camera targetCamera;
        private int lastScreenWidth;
        private int lastScreenHeight;
        private float lastAspect;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            EnsureForScene(SceneManager.GetActiveScene());
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            EnsureForScene(scene);
        }

        private static void EnsureForScene(Scene scene)
        {
            if (scene.name != GameSceneName)
                return;

            Camera mainCamera = Camera.main;

            if (mainCamera == null)
                return;

            if (mainCamera.GetComponent<MobileCameraViewport>() == null)
                mainCamera.gameObject.AddComponent<MobileCameraViewport>();
        }

        private void Awake()
        {
            targetCamera = GetComponent<Camera>();
        }

        private void OnEnable()
        {
            ApplyViewportSize(true);
        }

        private void LateUpdate()
        {
            ApplyViewportSize(false);
        }

        private void OnValidate()
        {
            referenceAspect = Mathf.Max(0.1f, referenceAspect);
            portraitReferenceAspect = Mathf.Max(0.1f, portraitReferenceAspect);
            referenceOrthographicSize = Mathf.Max(0.1f, referenceOrthographicSize);
            minimumOrthographicSize = Mathf.Max(0.1f, minimumOrthographicSize);
            maximumOrthographicSize = Mathf.Max(minimumOrthographicSize, maximumOrthographicSize);
            maximumPortraitOrthographicSize = Mathf.Max(maximumOrthographicSize, maximumPortraitOrthographicSize);
            mapEdgePadding = Mathf.Max(0f, mapEdgePadding);
        }

        private void ApplyViewportSize(bool force)
        {
            if (targetCamera == null)
                targetCamera = GetComponent<Camera>();

            if (targetCamera == null || !targetCamera.orthographic)
                return;

            int width = Mathf.Max(1, Screen.width);
            int height = Mathf.Max(1, Screen.height);
            float aspect = (float)width / height;

            if (!force && width == lastScreenWidth && height == lastScreenHeight && Mathf.Approximately(aspect, lastAspect))
                return;

            lastScreenWidth = width;
            lastScreenHeight = height;
            lastAspect = aspect;

            bool isPortrait = aspect < 1f;
            float maxSize = isPortrait ? maximumPortraitOrthographicSize : maximumOrthographicSize;
            float desiredSize = referenceOrthographicSize;

            if (isPortrait)
                desiredSize = referenceOrthographicSize * portraitReferenceAspect / aspect;
            else if (aspect > referenceAspect)
                desiredSize = referenceOrthographicSize * referenceAspect / aspect;

            desiredSize = Mathf.Clamp(desiredSize, minimumOrthographicSize, maxSize);
            desiredSize = ClampToMapBounds(desiredSize, aspect, maxSize);
            targetCamera.orthographicSize = desiredSize;
        }

        private float ClampToMapBounds(float desiredSize, float aspect, float maxSize)
        {
            if (!MapBoundary.TryGetWorldBounds(out Bounds mapBounds))
                return desiredSize;

            float maxHeightSize = Mathf.Max(minimumOrthographicSize, mapBounds.size.y * 0.5f - mapEdgePadding);
            float maxWidthSize = Mathf.Max(minimumOrthographicSize, (mapBounds.size.x * 0.5f - mapEdgePadding) / Mathf.Max(0.1f, aspect));
            float maxAllowedSize = Mathf.Min(maxSize, maxHeightSize, maxWidthSize);

            return Mathf.Min(desiredSize, Mathf.Max(minimumOrthographicSize, maxAllowedSize));
        }
    }
}
