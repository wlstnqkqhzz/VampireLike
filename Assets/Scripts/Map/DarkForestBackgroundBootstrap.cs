using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using VampireLike.World;

namespace VampireLike.Map
{
    /// <summary>
    /// Replaces the visible floor tilemap with one large background sprite and syncs map bounds to it.
    /// </summary>
    public static class DarkForestBackgroundBootstrap
    {
        private const string GameSceneName = "SampleScene";
        private const string BackgroundObjectName = "Dark Forest Background";
        private const string BackgroundResourcePath = "Tiles/DarkForestBackground";
        private const string TilemapObjectName = "Tilemap";
        private const int BackgroundSortingOrder = -100;
        private const float TargetWorldWidth = 56f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;

            ApplyToScene(SceneManager.GetActiveScene());
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ApplyToScene(scene);
        }

        private static void ApplyToScene(Scene scene)
        {
            if (scene.name != GameSceneName)
                return;

            ConfigureCameraBackground();
            HideTilemapRenderer();
            CreateBackgroundIfNeeded();
        }

        private static void HideTilemapRenderer()
        {
            TilemapRenderer tilemapRenderer = null;
            GameObject tilemapObject = GameObject.Find(TilemapObjectName);

            if (tilemapObject != null)
                tilemapRenderer = tilemapObject.GetComponent<TilemapRenderer>();

            if (tilemapRenderer == null)
                tilemapRenderer = Object.FindFirstObjectByType<TilemapRenderer>();

            if (tilemapRenderer != null)
                tilemapRenderer.enabled = false;
        }

        private static void CreateBackgroundIfNeeded()
        {
            Sprite backgroundSprite = Resources.Load<Sprite>(BackgroundResourcePath);
            if (backgroundSprite == null)
            {
                Debug.LogWarning($"Dark forest background sprite not found: Resources/{BackgroundResourcePath}");
                return;
            }

            GameObject backgroundObject = GameObject.Find(BackgroundObjectName);

            if (backgroundObject == null)
                backgroundObject = new GameObject(BackgroundObjectName);

            SpriteRenderer renderer = backgroundObject.GetComponent<SpriteRenderer>();

            if (renderer == null)
                renderer = backgroundObject.AddComponent<SpriteRenderer>();

            Bounds backgroundBounds = ConfigureBackground(backgroundObject, renderer, backgroundSprite);
            MapBoundary.OverrideActiveBounds(backgroundBounds);
        }

        private static Bounds ConfigureBackground(GameObject backgroundObject, SpriteRenderer renderer, Sprite backgroundSprite)
        {
            backgroundObject.transform.position = Vector3.zero;
            backgroundObject.transform.rotation = Quaternion.identity;

            renderer.sprite = backgroundSprite;
            renderer.sortingOrder = BackgroundSortingOrder;

            float spriteWorldWidth = backgroundSprite.bounds.size.x;
            float scale = spriteWorldWidth <= 0f ? 1f : TargetWorldWidth / spriteWorldWidth;
            backgroundObject.transform.localScale = Vector3.one * scale;

            Vector3 scaledSize = backgroundSprite.bounds.size * scale;
            return new Bounds(backgroundObject.transform.position, scaledSize);
        }

        private static void ConfigureCameraBackground()
        {
            Camera mainCamera = Camera.main;

            if (mainCamera == null)
                return;

            mainCamera.backgroundColor = new Color(0.02f, 0.06f, 0.07f, 1f);
        }
    }
}
