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
        private Collider2D targetCollider;
        private EnemyHealth enemyHealth;
        private int bloodStacks;
        private int bloodRequiredStacks;
        private float intentProgress;

        private static readonly Color BloodFilledColor = new Color(1f, 0.02f, 0.05f, 1f);
        private static readonly Color BloodEmptyColor = new Color(0.9f, 0.02f, 0.04f, 0.42f);

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
                mark.transform.localScale = Vector3.one * (filled ? 0.058f : 0.042f);
            }
        }

        private void RefreshKillingIntent()
        {
        }

        private void RefreshKillingIntentPulse()
        {
        }

        private void UpdateVisualPositions()
        {
            Bounds bounds = GetTargetBounds();
            Vector3 center = bounds.center;

            if (bloodRoot != null)
            {
                bloodRoot.position = new Vector3(center.x, bounds.max.y + 0.02f, center.z);

                for (int i = 0; i < bloodMarks.Length; i++)
                {
                    if (bloodMarks[i] == null)
                        continue;

                    float spacing = 0.036f;
                    float startX = -spacing * (bloodRequiredStacks - 1) * 0.5f;
                    Vector3 basePosition = new Vector3(startX + spacing * i, 0f, 0f);
                    bloodMarks[i].transform.localPosition = basePosition + new Vector3(0f, Mathf.Sin((Time.time * 7f) + i) * 0.0015f, 0f);
                    bloodMarks[i].transform.localRotation = Quaternion.identity;
                }
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
        }
    }
}
