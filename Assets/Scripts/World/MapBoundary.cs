using UnityEngine;
using UnityEngine.Tilemaps;

namespace VampireLike.World
{
    /// <summary>
    /// 바닥 Tilemap의 실제 타일 범위를 기준으로 보이지 않는 맵 경계 콜라이더를 만든다.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class MapBoundary : MonoBehaviour
    {
        private const string BoundaryRootName = "Generated Map Boundaries";

        [SerializeField]
        private Tilemap floorTilemap;

        [SerializeField]
        private float wallThickness = 1f;

        [SerializeField]
        private float spawnInset = 0.75f;

        [SerializeField]
        private bool rebuildOnStart = true;

        private Bounds worldBounds;
        private bool hasBounds;
        private static MapBoundary activeBoundary;

        public static bool TryGetWorldBounds(out Bounds bounds)
        {
            if (activeBoundary != null && activeBoundary.hasBounds)
            {
                bounds = activeBoundary.worldBounds;
                return true;
            }

            bounds = default;
            return false;
        }

        public static Vector2 ClampToPlayableArea(Vector2 position)
        {
            if (!TryGetWorldBounds(out Bounds bounds))
                return position;

            float inset = activeBoundary == null ? 0f : activeBoundary.spawnInset;
            return new Vector2(
                Mathf.Clamp(position.x, bounds.min.x + inset, bounds.max.x - inset),
                Mathf.Clamp(position.y, bounds.min.y + inset, bounds.max.y - inset));
        }

        private void Awake()
        {
            activeBoundary = this;
            BuildBounds();
        }

        private void Start()
        {
            if (rebuildOnStart)
                RebuildBoundaryColliders();
        }

        private void OnDestroy()
        {
            if (activeBoundary == this)
                activeBoundary = null;
        }

        private void OnValidate()
        {
            wallThickness = Mathf.Max(0.1f, wallThickness);
            spawnInset = Mathf.Max(0f, spawnInset);
        }

        private void BuildBounds()
        {
            if (floorTilemap == null)
                floorTilemap = GetComponentInChildren<Tilemap>();

            hasBounds = TryCalculateTileBounds(out worldBounds);
        }

        private bool TryCalculateTileBounds(out Bounds bounds)
        {
            bounds = default;

            if (floorTilemap == null)
                return false;

            BoundsInt cellBounds = floorTilemap.cellBounds;
            bool foundTile = false;
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;

            foreach (Vector3Int cellPosition in cellBounds.allPositionsWithin)
            {
                if (!floorTilemap.HasTile(cellPosition))
                    continue;

                foundTile = true;
                minX = Mathf.Min(minX, cellPosition.x);
                minY = Mathf.Min(minY, cellPosition.y);
                maxX = Mathf.Max(maxX, cellPosition.x);
                maxY = Mathf.Max(maxY, cellPosition.y);
            }

            if (!foundTile)
                return false;

            Vector3 minWorld = floorTilemap.CellToWorld(new Vector3Int(minX, minY, 0));
            Vector3 maxWorld = floorTilemap.CellToWorld(new Vector3Int(maxX + 1, maxY + 1, 0));
            bounds = new Bounds((minWorld + maxWorld) * 0.5f, maxWorld - minWorld);
            return true;
        }

        private void RebuildBoundaryColliders()
        {
            BuildBounds();

            if (!hasBounds)
            {
                Debug.LogWarning("MapBoundary could not find floor tile bounds.");
                return;
            }

            Transform boundaryRoot = transform.Find(BoundaryRootName);

            if (boundaryRoot != null)
                Destroy(boundaryRoot.gameObject);

            GameObject root = new GameObject(BoundaryRootName);
            root.transform.SetParent(transform);
            root.transform.position = Vector3.zero;
            root.transform.rotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;

            float width = worldBounds.size.x;
            float height = worldBounds.size.y;
            float thickness = wallThickness;

            CreateWall(root.transform, "Left Wall",
                new Vector2(worldBounds.min.x - thickness * 0.5f, worldBounds.center.y),
                new Vector2(thickness, height + thickness * 2f));

            CreateWall(root.transform, "Right Wall",
                new Vector2(worldBounds.max.x + thickness * 0.5f, worldBounds.center.y),
                new Vector2(thickness, height + thickness * 2f));

            CreateWall(root.transform, "Bottom Wall",
                new Vector2(worldBounds.center.x, worldBounds.min.y - thickness * 0.5f),
                new Vector2(width + thickness * 2f, thickness));

            CreateWall(root.transform, "Top Wall",
                new Vector2(worldBounds.center.x, worldBounds.max.y + thickness * 0.5f),
                new Vector2(width + thickness * 2f, thickness));
        }

        private static void CreateWall(Transform parent, string wallName, Vector2 position, Vector2 size)
        {
            GameObject wall = new GameObject(wallName);
            wall.transform.SetParent(parent);
            wall.transform.position = position;

            BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
            collider.size = size;
            collider.isTrigger = false;
        }
    }
}
