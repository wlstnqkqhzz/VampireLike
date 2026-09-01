using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VampireLike.Audio;
using VampireLike.Combat;
using VampireLike.VFX;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스 주변에 둔화 장판을 생성해 플레이어의 이동 공간을 제한하는 패턴이다.
    /// </summary>
    public class AreaZonePattern : BossPattern, IBossDamageScaler
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

        [Header("Center Bind")]
        [SerializeField]
        private bool centerBindEnabled;

        [SerializeField]
        private float centerBindRadius = 0.35f;

        [SerializeField]
        private float centerBindDuration = 2f;

        [SerializeField]
        private bool bindOncePerZone = true;

        [SerializeField]
        private bool spawnNearPlayer = true;

        [SerializeField]
        private bool clearZonesOnBossDeath = true;

        [SerializeField]
        private float warningDuration = 1f;

        [SerializeField]
        private Color warningColor = new Color(0.86f, 0.92f, 1f, 0.42f);

        [SerializeField]
        private Color fallbackZoneColor = new Color(0.82f, 0.82f, 0.95f, 0.45f);

        private readonly List<GameObject> activeZones = new List<GameObject>();
        private readonly List<GameObject> activeWarnings = new List<GameObject>();

        protected override bool CanExecutePattern()
        {
            RemoveMissingZones();
            return activeZones.Count < GetMaxActiveZones();
        }

        protected override IEnumerator ExecutePattern()
        {
            Boss.SetState(BossState.Preparing, false);
            RemoveMissingZones();
            CombatVFXKind vfxKind = GetVfxKind();
            CombatVFX.PlayBossCastAura(transform, vfxKind, 0.76f, 0.32f, 1500);

            int availableSlots = GetMaxActiveZones() - activeZones.Count;
            int count = Mathf.Min(availableSlots, zonesPerCast + Mathf.Max(0, Boss.CurrentPhase - 1) * phaseBonusZonesPerCast);

            Vector2[] positions = new Vector2[count];

            for (int i = 0; i < count && !Boss.IsDead; i++)
            {
                positions[i] = GetZonePosition();
                GameObject warning = CreateZoneWarning(positions[i], vfxKind);

                if (warning != null)
                    activeWarnings.Add(warning);
            }

            if (warningDuration > 0f && activeWarnings.Count > 0)
                yield return new WaitForSeconds(warningDuration);

            DestroyActiveWarnings();

            for (int i = 0; i < count && !Boss.IsDead; i++)
            {
                GameObject zone = CreateZone(positions[i]);
                activeZones.Add(zone);
            }
        }

        private void Update()
        {
            if (clearZonesOnBossDeath && Boss != null && Boss.IsDead)
                ClearZones();
        }

        private void OnDisable()
        {
            DestroyActiveWarnings();

            if (clearZonesOnBossDeath)
                ClearZones();
        }

        protected override void OnPatternCancelled()
        {
            DestroyActiveWarnings();

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

            areaZone.Initialize(duration, slowMultiplier, damagePerTick, damageInterval, radius,
                centerBindEnabled, centerBindRadius, centerBindDuration, bindOncePerZone);

            if (zonePrefab != null)
                ScaleZoneVisual(zone);

            GameSfx.Play(SfxType.BossZone);
            CombatVFXKind vfxKind = GetVfxKind();
            CombatVFX.PlayExpandingRing(position, vfxKind, radius * 0.35f, radius * 2f, 0.28f, 620);
            CombatVFX.CreateZoneVisual(zone.transform, vfxKind, radius, fallbackZoneColor);
            return zone;
        }

        private GameObject CreateZoneWarning(Vector2 position, CombatVFXKind vfxKind)
        {
            if (vfxKind == CombatVFXKind.WebZone)
                return CreateWebZoneWarning(position);

            GameObject warning = new GameObject("Boss Area Zone Warning");
            warning.transform.position = position;

            GameObject fill = new GameObject("Warning Fill");
            fill.transform.SetParent(warning.transform, false);
            fill.transform.localScale = Vector3.one * radius * 2f;

            SpriteRenderer fillRenderer = fill.AddComponent<SpriteRenderer>();
            fillRenderer.sprite = SpecialUpgradePulse.GetFilledCircleSprite();
            fillRenderer.color = new Color(warningColor.r, warningColor.g, warningColor.b, warningColor.a * 0.32f);
            fillRenderer.sortingOrder = 612;

            GameObject detail = new GameObject("Warning Detail");
            detail.transform.SetParent(warning.transform, false);
            detail.transform.localScale = Vector3.one * radius * 2f;

            SpriteRenderer detailRenderer = detail.AddComponent<SpriteRenderer>();
            detailRenderer.sprite = GetWarningDetailSprite(vfxKind);
            detailRenderer.color = warningColor;
            detailRenderer.sortingOrder = 613;

            LineRenderer lineRenderer = warning.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = false;
            lineRenderer.loop = true;
            lineRenderer.positionCount = 48;
            lineRenderer.startWidth = 0.035f;
            lineRenderer.endWidth = 0.035f;
            lineRenderer.sortingOrder = 614;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = new Color(warningColor.r, warningColor.g, warningColor.b, warningColor.a * 1.35f);
            lineRenderer.endColor = lineRenderer.startColor;

            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                float angle = Mathf.PI * 2f * i / lineRenderer.positionCount;
                lineRenderer.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f));
            }

            CombatVFX.PlayExpandingRing(position, vfxKind, radius * 0.45f, radius * 2f, Mathf.Max(0.18f, warningDuration), 615);
            return warning;
        }

        private GameObject CreateWebZoneWarning(Vector2 position)
        {
            GameObject warning = new GameObject("Boss Web Zone Warning");
            warning.transform.position = position;

            GameObject fill = new GameObject("Web Warning Fill");
            fill.transform.SetParent(warning.transform, false);
            fill.transform.localScale = Vector3.one * radius * 2f;

            SpriteRenderer fillRenderer = fill.AddComponent<SpriteRenderer>();
            fillRenderer.sprite = SpecialUpgradePulse.GetFilledCircleSprite();
            fillRenderer.color = new Color(0.58f, 0.46f, 0.78f, 0.04f);
            fillRenderer.sortingOrder = 612;

            GameObject web = new GameObject("Web Warning Shape");
            web.transform.SetParent(warning.transform, false);
            web.transform.localScale = Vector3.one * radius * 2f;

            SpriteRenderer webRenderer = web.AddComponent<SpriteRenderer>();
            webRenderer.sprite = SpecialUpgradePulse.GetWebSprite();
            webRenderer.color = new Color(0.92f, 0.94f, 1f, 0.08f);
            webRenderer.sortingOrder = 614;

            WebWarningVisual visual = warning.AddComponent<WebWarningVisual>();
            visual.Play(fillRenderer, webRenderer, warningDuration);
            return warning;
        }

        private static Sprite GetWarningDetailSprite(CombatVFXKind vfxKind)
        {
            return vfxKind == CombatVFXKind.WebZone
                ? SpecialUpgradePulse.GetWebSprite()
                : SpecialUpgradePulse.GetCircleSprite();
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

            SpriteRenderer spriteRenderer = zone.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = SpecialUpgradePulse.GetWebSprite();
            spriteRenderer.color = fallbackZoneColor;
            spriteRenderer.sortingOrder = 10;
            zone.transform.localScale = Vector3.one * radius * 2f;

            return zone;
        }

        private CombatVFXKind GetVfxKind()
        {
            string zoneName = zonePrefab == null ? string.Empty : zonePrefab.name.ToLowerInvariant();

            if (zoneName.Contains("fire") || zoneName.Contains("flame"))
                return CombatVFXKind.FireZone;

            if (zoneName.Contains("frost") || zoneName.Contains("ice"))
                return CombatVFXKind.FrostZone;

            return CombatVFXKind.WebZone;
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
            DestroyActiveWarnings();

            for (int i = activeZones.Count - 1; i >= 0; i--)
            {
                if (activeZones[i] != null)
                    Destroy(activeZones[i]);
            }

            activeZones.Clear();
        }

        private void DestroyActiveWarnings()
        {
            for (int i = activeWarnings.Count - 1; i >= 0; i--)
            {
                GameObject warning = activeWarnings[i];

                if (warning == null)
                    continue;

                LineRenderer lineRenderer = warning.GetComponent<LineRenderer>();

                if (lineRenderer != null && lineRenderer.material != null)
                    Destroy(lineRenderer.material);

                Destroy(warning);
            }

            activeWarnings.Clear();
        }

        public void ScaleBossDamage(float multiplier)
        {
            if (damagePerTick <= 0)
                return;

            damagePerTick = Mathf.Max(1, Mathf.RoundToInt(damagePerTick * Mathf.Max(0.1f, multiplier)));
        }

        public void ConfigureAreaZone(float radius, float duration, float spawnRadius, int zonesPerCast,
            int phaseBonusZonesPerCast, int maxActiveZones, int phaseBonusMaxZones, float slowMultiplier,
            int damagePerTick, float damageInterval, float warningDuration, Color fallbackZoneColor)
        {
            this.radius = radius;
            this.duration = duration;
            this.spawnRadius = spawnRadius;
            this.zonesPerCast = zonesPerCast;
            this.phaseBonusZonesPerCast = phaseBonusZonesPerCast;
            this.maxActiveZones = maxActiveZones;
            this.phaseBonusMaxZones = phaseBonusMaxZones;
            this.slowMultiplier = slowMultiplier;
            this.damagePerTick = damagePerTick;
            this.damageInterval = damageInterval;
            this.warningDuration = warningDuration;
            this.fallbackZoneColor = fallbackZoneColor;
            OnValidate();
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
            centerBindRadius = Mathf.Clamp(centerBindRadius, 0.05f, radius);
            centerBindDuration = Mathf.Max(0f, centerBindDuration);
            warningDuration = Mathf.Max(0f, warningDuration);
        }
    }

    public class WebWarningVisual : MonoBehaviour
    {
        private SpriteRenderer fillRenderer;
        private SpriteRenderer webRenderer;
        private float duration;
        private float elapsedTime;

        public void Play(SpriteRenderer fillRenderer, SpriteRenderer webRenderer, float duration)
        {
            this.fillRenderer = fillRenderer;
            this.webRenderer = webRenderer;
            this.duration = Mathf.Max(0.05f, duration);
        }

        private void Update()
        {
            if (webRenderer == null)
                return;

            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration);
            float pulse = 0.5f + Mathf.Sin(progress * Mathf.PI * 8f) * 0.5f;

            webRenderer.color = new Color(0.95f, 0.97f, 1f, Mathf.Lerp(0.08f, 0.74f, progress));
            webRenderer.transform.localScale = Vector3.one * Mathf.Lerp(0.92f, 1.02f, progress);

            if (fillRenderer != null)
                fillRenderer.color = new Color(0.58f, 0.46f, 0.78f, Mathf.Lerp(0.03f, 0.16f, progress) + pulse * 0.025f);
        }
    }
}
