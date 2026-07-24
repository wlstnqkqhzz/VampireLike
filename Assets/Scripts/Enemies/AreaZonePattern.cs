using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스 주변에 둔화 장판을 생성해 플레이어의 이동 공간을 제한하는 패턴이다.
    /// </summary>
    public class AreaZonePattern : BossPattern
    {
        protected override bool UseSkillAnimation => true;

        [SerializeField]
        private GameObject zonePrefab;

        [SerializeField]
        private float radius = 0.9f;

        [SerializeField]
        private float duration = 5f;

        [SerializeField]
        private float spawnRadius = 2.8f;

        [SerializeField]
        private int zonesPerCast = 1;

        [SerializeField]
        private int phaseBonusZonesPerCast = 1;

        [SerializeField]
        private int maxActiveZones = 3;

        [SerializeField]
        private int phaseBonusMaxZones = 1;

        [SerializeField]
        private float slowMultiplier = 0.55f;

        [SerializeField]
        private int damagePerTick;

        [SerializeField]
        private float damageInterval = 0.7f;

        [SerializeField]
        private bool spawnNearPlayer = true;

        [SerializeField]
        private bool clearZonesOnBossDeath = true;

        [SerializeField]
        private Color fallbackZoneColor = new Color(0.82f, 0.82f, 0.95f, 0.45f);

        private readonly List<GameObject> activeZones = new List<GameObject>();

        protected override bool CanExecutePattern()
        {
            RemoveMissingZones();
            return activeZones.Count < GetMaxActiveZones();
        }

        protected override IEnumerator ExecutePattern()
        {
            Boss.SetState(BossState.Preparing, false);
            RemoveMissingZones();

            int availableSlots = GetMaxActiveZones() - activeZones.Count;
            int count = Mathf.Min(availableSlots, zonesPerCast + Mathf.Max(0, Boss.CurrentPhase - 1) * phaseBonusZonesPerCast);

            for (int i = 0; i < count && !Boss.IsDead; i++)
            {
                GameObject zone = CreateZone(GetZonePosition());
                activeZones.Add(zone);
            }

            yield break;
        }

        private void Update()
        {
            if (clearZonesOnBossDeath && Boss != null && Boss.IsDead)
                ClearZones();
        }

        private void OnDisable()
        {
            if (clearZonesOnBossDeath)
                ClearZones();
        }

        private GameObject CreateZone(Vector2 position)
        {
            GameObject zone = zonePrefab == null ? CreateFallbackZone() : Instantiate(zonePrefab);
            zone.name = "Spider Web Zone";
            zone.transform.position = position;

            BossAreaZone areaZone = zone.GetComponent<BossAreaZone>();

            if (areaZone == null)
                areaZone = zone.AddComponent<BossAreaZone>();

            areaZone.Initialize(duration, slowMultiplier, damagePerTick, damageInterval, radius);

            if (zonePrefab != null)
                ScaleZoneVisual(zone);

            return zone;
        }

        private void ScaleZoneVisual(GameObject zone)
        {
            SpriteRenderer spriteRenderer = zone.GetComponentInChildren<SpriteRenderer>();

            if (spriteRenderer == null || spriteRenderer.sprite == null)
                return;

            float spriteSize = Mathf.Max(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
            float targetDiameter = radius * 2f;
            zone.transform.localScale = Vector3.one * (targetDiameter / Mathf.Max(0.01f, spriteSize));
        }

        private GameObject CreateFallbackZone()
        {
            GameObject zone = new GameObject("Spider Web Zone");
            zone.AddComponent<CircleCollider2D>();

            LineRenderer lineRenderer = zone.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = false;
            lineRenderer.loop = true;
            lineRenderer.positionCount = 40;
            lineRenderer.startWidth = 0.035f;
            lineRenderer.endWidth = 0.035f;
            lineRenderer.sortingOrder = 11;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = fallbackZoneColor;
            lineRenderer.endColor = fallbackZoneColor;

            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                float angle = Mathf.PI * 2f * i / lineRenderer.positionCount;
                lineRenderer.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f));
            }

            return zone;
        }

        private Vector2 GetZonePosition()
        {
            Vector2 center = spawnNearPlayer && Player != null ? Player.position : transform.position;
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(0f, spawnRadius);
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            return center + direction * distance;
        }

        private int GetMaxActiveZones()
        {
            return maxActiveZones + Mathf.Max(0, Boss.CurrentPhase - 1) * phaseBonusMaxZones;
        }

        private void RemoveMissingZones()
        {
            for (int i = activeZones.Count - 1; i >= 0; i--)
            {
                if (activeZones[i] == null)
                    activeZones.RemoveAt(i);
            }
        }

        private void ClearZones()
        {
            for (int i = activeZones.Count - 1; i >= 0; i--)
            {
                if (activeZones[i] != null)
                    Destroy(activeZones[i]);
            }

            activeZones.Clear();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            radius = Mathf.Max(0.05f, radius);
            duration = Mathf.Max(0.1f, duration);
            spawnRadius = Mathf.Max(0f, spawnRadius);
            zonesPerCast = Mathf.Max(0, zonesPerCast);
            phaseBonusZonesPerCast = Mathf.Max(0, phaseBonusZonesPerCast);
            maxActiveZones = Mathf.Max(0, maxActiveZones);
            phaseBonusMaxZones = Mathf.Max(0, phaseBonusMaxZones);
            slowMultiplier = Mathf.Clamp(slowMultiplier, 0.25f, 1f);
            damagePerTick = Mathf.Max(0, damagePerTick);
            damageInterval = Mathf.Max(0.1f, damageInterval);
        }
    }
}
