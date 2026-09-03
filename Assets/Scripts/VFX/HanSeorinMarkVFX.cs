using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.VFX
{
    public class HanSeorinMarkVFX : MonoBehaviour
    {
        private const int MaxBloodMarks = 5;
        private const int SortingOrder = 1450;

        private readonly SpriteRenderer[] bloodMarks = new SpriteRenderer[MaxBloodMarks];
        private Transform bloodRoot;
        private Transform intentRoot;
        private SpriteRenderer intentRing;
        private SpriteRenderer intentFocus;
        private SpriteRenderer intentCore;
        private Collider2D targetCollider;
        private EnemyHealth enemyHealth;
        private int bloodStacks;
        private int bloodRequiredStacks;
        private float intentProgress;

        private static readonly Color BloodFilledColor = new Color(1f, 0.02f, 0.06f, 1f);
        private static readonly Color BloodEmptyColor = new Color(1f, 0.05f, 0.08f, 0.52f);
        private static readonly Color IntentRingColor = new Color(1f, 0.02f, 0.08f, 0.78f);
        private static readonly Color IntentCoreColor = new Color(1f, 0.16f, 0.2f, 0.88f);

        public void SetBloodMark(int stacks, int requiredStacks)
        {
            bloodStacks = Mathf.Max(0, stacks);
            bloodRequiredStacks = Mathf.Clamp(requiredStacks, 1, MaxBloodMarks);
            EnsureVisuals();
            RefreshBloodMark();
        }

        public void ClearBloodMark()
        {
            bloodStacks = 0;
            RefreshBloodMark();
            DestroyIfUnused();
        }

        public void SetKillingIntent(float progress)
        {
            intentProgress = Mathf.Clamp01(progress);
            EnsureVisuals();
            RefreshKillingIntent();
        }

        public void ClearKillingIntent()
        {
            intentProgress = 0f;
            RefreshKillingIntent();
            DestroyIfUnused();
        }

        private void LateUpdate()
        {
            if (enemyHealth != null && enemyHealth.IsDead)
            {
                ClearVisualObjects();
                Destroy(this);
                return;
            }

            UpdateVisualPositions();
            RefreshKillingIntentPulse();
        }

        private void EnsureVisuals()
        {
            if (enemyHealth == null)
                enemyHealth = GetComponent<EnemyHealth>();

            if (targetCollider == null)
                targetCollider = GetComponent<Collider2D>();

            if (bloodRoot == null)
            {
                GameObject root = new GameObject("HanSeorin Blood Seal");
                root.transform.SetParent(transform, false);
                bloodRoot = root.transform;
            }

            for (int i = 0; i < bloodMarks.Length; i++)
            {
                if (bloodMarks[i] != null)
                    continue;

                GameObject markObject = new GameObject($"Blood Seal Mark {i + 1}");
                markObject.transform.SetParent(bloodRoot, false);
                SpriteRenderer mark = markObject.AddComponent<SpriteRenderer>();
                mark.sprite = VFXSprites.SoftDisc;
                mark.sortingOrder = SortingOrder + 10 + i;
                bloodMarks[i] = mark;
            }

            if (intentRoot == null)
            {
                GameObject root = new GameObject("HanSeorin Killing Intent");
                root.transform.SetParent(transform, false);
                intentRoot = root.transform;
            }

            if (intentRing == null)
                intentRing = CreateRenderer("Killing Intent Ring", VFXSprites.WarningRing, SortingOrder + 24);

            if (intentFocus == null)
                intentFocus = CreateRenderer("Killing Intent Focus", VFXSprites.LineCore, SortingOrder + 25);

            if (intentCore == null)
                intentCore = CreateRenderer("Killing Intent Core", VFXSprites.SoftDisc, SortingOrder + 26);
        }

        private SpriteRenderer CreateRenderer(string objectName, Sprite sprite, int sortingOrder)
        {
            GameObject marker = new GameObject(objectName);
            marker.transform.SetParent(intentRoot != null && objectName.StartsWith("Killing Intent") ? intentRoot : transform, false);
            SpriteRenderer renderer = marker.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        private void RefreshBloodMark()
        {
            bool sealVisible = bloodStacks > 0;
            float progress = bloodRequiredStacks <= 0 ? 0f : Mathf.Clamp01((float)bloodStacks / bloodRequiredStacks);

            for (int i = 0; i < bloodMarks.Length; i++)
            {
                SpriteRenderer mark = bloodMarks[i];

                if (mark == null)
                    continue;

                bool visible = sealVisible && i < bloodRequiredStacks;
                mark.enabled = visible;

                if (!visible)
                    continue;

                bool filled = i < bloodStacks;
                mark.color = filled ? BloodFilledColor : BloodEmptyColor;
                mark.transform.localScale = Vector3.one * (filled ? 0.118f : 0.078f);
            }
        }

        private void RefreshKillingIntent()
        {
            bool visible = intentProgress > 0f;

            if (intentRoot != null)
                intentRoot.gameObject.SetActive(visible);

            if (!visible)
                return;

            RefreshKillingIntentPulse();
        }

        private void RefreshKillingIntentPulse()
        {
            if (intentRoot == null || !intentRoot.gameObject.activeSelf)
                return;

            float pulse = 0.5f + Mathf.Sin(Time.time * 12f) * 0.5f;
            float strength = Mathf.Clamp01(0.35f + intentProgress * 0.65f);
            float size = Mathf.Lerp(0.18f, 0.32f, strength) + pulse * 0.018f;

            if (intentRing != null)
            {
                intentRing.enabled = true;
                intentRing.color = new Color(IntentRingColor.r, IntentRingColor.g, IntentRingColor.b, Mathf.Lerp(0.42f, 0.88f, strength));
                intentRing.transform.localScale = Vector3.one * size;
                intentRing.transform.localRotation = Quaternion.Euler(0f, 0f, Time.time * -120f);
            }

            if (intentFocus != null)
            {
                intentFocus.enabled = true;
                intentFocus.color = new Color(IntentCoreColor.r, IntentCoreColor.g, IntentCoreColor.b, Mathf.Lerp(0.58f, 0.96f, strength));
                intentFocus.transform.localScale = new Vector3(size * 0.82f, 0.022f, 1f);
                intentFocus.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            }

            if (intentCore != null)
            {
                intentCore.enabled = true;
                intentCore.color = new Color(IntentCoreColor.r, IntentCoreColor.g, IntentCoreColor.b, Mathf.Lerp(0.16f, 0.34f, strength));
                intentCore.transform.localScale = Vector3.one * (size * 0.42f);
            }
        }

        private void UpdateVisualPositions()
        {
            Bounds bounds = GetTargetBounds();
            Vector3 center = bounds.center;

            if (bloodRoot != null)
            {
                bloodRoot.position = new Vector3(center.x, bounds.max.y + 0.08f, center.z);

                for (int i = 0; i < bloodMarks.Length; i++)
                {
                    if (bloodMarks[i] == null)
                        continue;

                    float spacing = 0.068f;
                    float startX = -spacing * (bloodRequiredStacks - 1) * 0.5f;
                    Vector3 basePosition = new Vector3(startX + spacing * i, 0f, 0f);
                    bloodMarks[i].transform.localPosition = basePosition + new Vector3(0f, Mathf.Sin((Time.time * 7f) + i) * 0.003f, 0f);
                    bloodMarks[i].transform.localRotation = Quaternion.identity;
                }
            }

            if (intentRoot != null)
            {
                float markerY = Mathf.Lerp(center.y, bounds.max.y, 0.42f);
                intentRoot.position = new Vector3(center.x, markerY, center.z);
            }
        }

        private Bounds GetTargetBounds()
        {
            if (targetCollider == null)
                targetCollider = GetComponent<Collider2D>();

            if (targetCollider != null)
                return targetCollider.bounds;

            return new Bounds(transform.position, Vector3.one * 0.5f);
        }

        private void DestroyIfUnused()
        {
            // Keep the component alive while the enemy lives. Blood Mark can explode
            // and Killing Intent can update again during the same hit event.
        }

        private void ClearVisualObjects()
        {
            if (bloodRoot != null)
                Destroy(bloodRoot.gameObject);

            if (intentRoot != null)
                Destroy(intentRoot.gameObject);
        }
    }
}
