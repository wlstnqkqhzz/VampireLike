using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.VFX
{
    public class HanSeorinMarkVFX : MonoBehaviour
    {
        private const int MaxBloodMarks = 5;
        private const int SortingOrder = 1320;

        private readonly SpriteRenderer[] bloodMarks = new SpriteRenderer[MaxBloodMarks];
        private Transform bloodRoot;
        private SpriteRenderer bloodSealRing;
        private SpriteRenderer bloodSealCore;
        private SpriteRenderer intentRing;
        private SpriteRenderer intentCore;
        private Collider2D targetCollider;
        private EnemyHealth enemyHealth;
        private int bloodStacks;
        private int bloodRequiredStacks;
        private float intentProgress;

        private static readonly Color BloodFilledColor = new Color(1f, 0.02f, 0.07f, 0.92f);
        private static readonly Color BloodEmptyColor = new Color(0.55f, 0.02f, 0.04f, 0.24f);
        private static readonly Color IntentRingColor = new Color(1f, 0.02f, 0.08f, 0.42f);

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

            if (bloodSealRing == null)
                bloodSealRing = CreateRenderer("Blood Seal Ring", VFXSprites.WarningRing, SortingOrder + 8);

            if (bloodSealCore == null)
                bloodSealCore = CreateRenderer("Blood Seal Core", VFXSprites.SoftDisc, SortingOrder + 7);

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

            if (intentRing == null)
                intentRing = CreateRenderer("HanSeorin Killing Intent Ring", VFXSprites.WarningRing, SortingOrder - 2);

            if (intentCore == null)
                intentCore = CreateRenderer("HanSeorin Killing Intent Core", VFXSprites.SoftDisc, SortingOrder - 3);

            if (intentCore != null)
                intentCore.enabled = false;
        }

        private SpriteRenderer CreateRenderer(string objectName, Sprite sprite, int sortingOrder)
        {
            GameObject marker = new GameObject(objectName);
            marker.transform.SetParent(transform, false);
            SpriteRenderer renderer = marker.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        private void RefreshBloodMark()
        {
            bool sealVisible = bloodStacks > 0;
            float progress = bloodRequiredStacks <= 0 ? 0f : Mathf.Clamp01((float)bloodStacks / bloodRequiredStacks);

            if (bloodSealRing != null)
            {
                bloodSealRing.enabled = false;
            }

            if (bloodSealCore != null)
                bloodSealCore.enabled = false;

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
                mark.transform.localScale = Vector3.one * (filled ? 0.034f : 0.025f);
            }
        }

        private void RefreshKillingIntent()
        {
            bool visible = intentProgress > 0.01f;

            if (intentRing != null)
                intentRing.enabled = visible;

            if (intentCore != null)
                intentCore.enabled = false;
        }

        private void RefreshKillingIntentPulse()
        {
            if (intentProgress <= 0.01f || intentRing == null)
                return;

            float pulse = 1f + Mathf.Sin(Time.time * 8f) * 0.025f;
            float scale = Mathf.Lerp(0.2f, 0.32f, intentProgress) * pulse;
            intentRing.transform.localScale = Vector3.one * scale;
            intentRing.color = new Color(IntentRingColor.r, IntentRingColor.g, IntentRingColor.b, Mathf.Lerp(0.24f, 0.48f, intentProgress));
        }

        private void UpdateVisualPositions()
        {
            Bounds bounds = GetTargetBounds();
            Vector3 center = bounds.center;

            if (bloodRoot != null)
            {
                bloodRoot.position = new Vector3(center.x, bounds.max.y + 0.035f, center.z);

                if (bloodSealRing != null)
                {
                    bloodSealRing.enabled = false;
                }

                if (bloodSealCore != null)
                {
                    bloodSealCore.enabled = false;
                }

                for (int i = 0; i < bloodMarks.Length; i++)
                {
                    if (bloodMarks[i] == null)
                        continue;

                    float spacing = 0.028f;
                    float startX = -spacing * (bloodRequiredStacks - 1) * 0.5f;
                    Vector3 basePosition = new Vector3(startX + spacing * i, 0f, 0f);
                    bloodMarks[i].transform.localPosition = basePosition + new Vector3(0f, Mathf.Sin((Time.time * 7f) + i) * 0.002f, 0f);
                    bloodMarks[i].transform.localRotation = Quaternion.identity;
                }
            }

            Vector3 intentPosition = new Vector3(center.x, center.y, center.z);

            if (intentRing != null)
                intentRing.transform.position = intentPosition;

            if (intentCore != null)
                intentCore.transform.position = intentPosition;
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

            if (bloodSealRing != null)
                Destroy(bloodSealRing.gameObject);

            if (bloodSealCore != null)
                Destroy(bloodSealCore.gameObject);

            if (intentRing != null)
                Destroy(intentRing.gameObject);

            if (intentCore != null)
                Destroy(intentCore.gameObject);
        }
    }
}
