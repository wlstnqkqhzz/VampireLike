using System.Collections.Generic;
using UnityEngine;

namespace VampireLike.World
{
    /// <summary>
    /// 보스 페이즈 동안 맵 장식/장애물을 숨기고, 보스 처치 후 기존 오브젝트를 새 위치로 재배치한다.
    /// </summary>
    [DisallowMultipleComponent]
    public class BossPhaseDecorationController : MonoBehaviour
    {
        [SerializeField]
        private Transform decorationRoot;

        [SerializeField]
        private string fallbackRootName = "Decorations";

        [SerializeField]
        private float mapEdgeInset = 0.75f;

        [SerializeField]
        private float minDistanceFromPlayer = 2.25f;

        [SerializeField]
        private float minDistanceBetweenColliders = 1.1f;

        [SerializeField]
        private int placementAttemptsPerDecoration = 60;

        private readonly List<DecorationEntry> decorations = new List<DecorationEntry>();
        private readonly List<Vector2> placedColliderPositions = new List<Vector2>();
        private Transform player;
        private bool hasCapturedDecorations;
        private bool isClearedForBossPhase;

        public bool HasManagedDecorations
        {
            get
            {
                CaptureDecorationsIfNeeded();
                return decorations.Count > 0;
            }
        }

        private void Awake()
        {
            ResolveReferences();
            CaptureDecorationsIfNeeded();
        }

        private void OnValidate()
        {
            mapEdgeInset = Mathf.Max(0f, mapEdgeInset);
            minDistanceFromPlayer = Mathf.Max(0f, minDistanceFromPlayer);
            minDistanceBetweenColliders = Mathf.Max(0f, minDistanceBetweenColliders);
            placementAttemptsPerDecoration = Mathf.Max(1, placementAttemptsPerDecoration);
        }

        public void ClearForBossPhase()
        {
            CaptureDecorationsIfNeeded();

            if (decorations.Count == 0)
                return;

            foreach (DecorationEntry decoration in decorations)
            {
                if (decoration.GameObject == null)
                    continue;

                decoration.GameObject.SetActive(false);
            }

            isClearedForBossPhase = true;
        }

        public void RedistributeAfterBossPhase()
        {
            CaptureDecorationsIfNeeded();

            if (decorations.Count == 0)
                return;

            if (!MapBoundary.TryGetBaseWorldBounds(out Bounds mapBounds)
                && !MapBoundary.TryGetWorldBounds(out mapBounds))
            {
                RestoreOriginalActiveStates();
                isClearedForBossPhase = false;
                return;
            }

            ResolvePlayer();
            placedColliderPositions.Clear();

            foreach (DecorationEntry decoration in decorations)
            {
                if (decoration.GameObject == null)
                    continue;

                Vector2 position = FindPlacement(mapBounds, decoration.HasBlockingCollider);
                decoration.Transform.position = new Vector3(position.x, position.y, decoration.OriginalPosition.z);
                decoration.GameObject.SetActive(decoration.WasInitiallyActive);

                if (decoration.HasBlockingCollider)
                    placedColliderPositions.Add(position);
            }

            isClearedForBossPhase = false;
        }

        public void RestoreOriginalActiveStates()
        {
            CaptureDecorationsIfNeeded();

            foreach (DecorationEntry decoration in decorations)
            {
                if (decoration.GameObject == null)
                    continue;

                decoration.GameObject.SetActive(decoration.WasInitiallyActive);
            }
        }

        public void RestoreIfCleared()
        {
            if (!isClearedForBossPhase)
                return;

            RedistributeAfterBossPhase();
        }

        private void ResolveReferences()
        {
            if (decorationRoot != null)
                return;

            if (!string.IsNullOrWhiteSpace(fallbackRootName))
            {
                GameObject root = GameObject.Find(fallbackRootName);

                if (root != null)
                {
                    decorationRoot = root.transform;
                    return;
                }
            }

            if (transform.childCount > 0)
                decorationRoot = transform;
        }

        private void ResolvePlayer()
        {
            if (player != null)
                return;

            player = GameObject.Find("Player")?.transform;
        }

        private void CaptureDecorationsIfNeeded()
        {
            if (hasCapturedDecorations)
                return;

            ResolveReferences();

            if (decorationRoot == null)
            {
                hasCapturedDecorations = true;
                return;
            }

            decorations.Clear();

            for (int i = 0; i < decorationRoot.childCount; i++)
            {
                Transform child = decorationRoot.GetChild(i);

                if (child == null)
                    continue;

                Collider2D blockingCollider = child.GetComponent<Collider2D>();
                decorations.Add(new DecorationEntry(child, blockingCollider != null && !blockingCollider.isTrigger));
            }

            hasCapturedDecorations = true;
        }

        private Vector2 FindPlacement(Bounds mapBounds, bool avoidOtherColliders)
        {
            float minX = mapBounds.min.x + mapEdgeInset;
            float maxX = mapBounds.max.x - mapEdgeInset;
            float minY = mapBounds.min.y + mapEdgeInset;
            float maxY = mapBounds.max.y - mapEdgeInset;

            if (minX > maxX)
            {
                minX = mapBounds.center.x;
                maxX = mapBounds.center.x;
            }

            if (minY > maxY)
            {
                minY = mapBounds.center.y;
                maxY = mapBounds.center.y;
            }

            Vector2 fallback = mapBounds.center;

            for (int attempt = 0; attempt < placementAttemptsPerDecoration; attempt++)
            {
                Vector2 position = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));

                fallback = position;

                if (IsTooCloseToPlayer(position))
                    continue;

                if (avoidOtherColliders && IsTooCloseToOtherColliders(position))
                    continue;

                return position;
            }

            return fallback;
        }

        private bool IsTooCloseToPlayer(Vector2 position)
        {
            if (player == null || minDistanceFromPlayer <= 0f)
                return false;

            return ((Vector2)player.position - position).sqrMagnitude < minDistanceFromPlayer * minDistanceFromPlayer;
        }

        private bool IsTooCloseToOtherColliders(Vector2 position)
        {
            if (minDistanceBetweenColliders <= 0f)
                return false;

            float minSqrDistance = minDistanceBetweenColliders * minDistanceBetweenColliders;

            foreach (Vector2 placedPosition in placedColliderPositions)
            {
                if ((placedPosition - position).sqrMagnitude < minSqrDistance)
                    return true;
            }

            return false;
        }

        private readonly struct DecorationEntry
        {
            public DecorationEntry(Transform transform, bool hasBlockingCollider)
            {
                Transform = transform;
                GameObject = transform.gameObject;
                OriginalPosition = transform.position;
                WasInitiallyActive = transform.gameObject.activeSelf;
                HasBlockingCollider = hasBlockingCollider;
            }

            public Transform Transform { get; }
            public GameObject GameObject { get; }
            public Vector3 OriginalPosition { get; }
            public bool WasInitiallyActive { get; }
            public bool HasBlockingCollider { get; }
        }
    }
}
