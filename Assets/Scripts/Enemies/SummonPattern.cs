using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VampireLike.Audio;
using VampireLike.VFX;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스 주변에 일반 적을 제한된 수만큼 소환하는 패턴이다.
    /// </summary>
    public class SummonPattern : BossPattern
    {
        protected override bool UseSkillAnimation => true;

        [SerializeField]
        private GameObject summonPrefab;

        [SerializeField]
        private GameObject[] summonPrefabs;

        [SerializeField]
        private int minSummonCount = 2;

        [SerializeField]
        private int summonCount = 5;

        [SerializeField]
        private int phaseBonusSummonCount = 1;

        [SerializeField]
        private int maxActiveSummons = 8;

        [SerializeField]
        private float spawnRadius = 1.6f;

        [SerializeField]
        private float summonInterval = 0.15f;

        [SerializeField]
        private float summonTelegraphDelay = 0.45f;

        [Header("소환 효과음")]
        [SerializeField]
        private bool playPrepareSfx;

        [SerializeField]
        private SfxType prepareSfxType = SfxType.BossZone;

        [SerializeField]
        private bool playSummonSfx;

        [SerializeField]
        private SfxType summonSfxType = SfxType.BossZone;

        private readonly List<BossSummonTracker> activeSummons = new List<BossSummonTracker>();

        protected override bool CanExecutePattern()
        {
            RemoveMissingSummons();
            return GetSummonPrefabCount() > 0 && activeSummons.Count < maxActiveSummons;
        }

        protected override IEnumerator ExecutePattern()
        {
            Boss.SetState(BossState.Preparing, false);
            RemoveMissingSummons();

            int availableSlots = maxActiveSummons - activeSummons.Count;
            int desiredMax = summonCount + Mathf.Max(0, Boss.CurrentPhase - 1) * phaseBonusSummonCount;
            int desiredMin = Mathf.Min(minSummonCount, desiredMax);
            int randomCount = Random.Range(desiredMin, desiredMax + 1);
            int count = Mathf.Min(availableSlots, randomCount);

            if (count > 0 && playPrepareSfx)
                GameSfx.Play(prepareSfxType);

            for (int i = 0; i < count && !Boss.IsDead; i++)
            {
                Vector2 spawnPosition = GetSummonPosition(i, count);
                CombatVFX.PlayExpandingRing(spawnPosition, CombatVFXKind.ArcaneImpact, 0.18f, 0.9f, summonTelegraphDelay, 900);

                if (summonTelegraphDelay > 0f)
                    yield return new WaitForSeconds(summonTelegraphDelay);

                if (Boss.IsDead)
                    yield break;

                GameObject selectedPrefab = GetRandomSummonPrefab();

                if (selectedPrefab == null)
                    yield break;

                if (playSummonSfx)
                    GameSfx.Play(summonSfxType);

                GameObject summon = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
                BossSummonTracker tracker = summon.AddComponent<BossSummonTracker>();
                tracker.Initialize(HandleSummonRemoved);
                activeSummons.Add(tracker);

                if (summonInterval > 0f)
                    yield return new WaitForSeconds(summonInterval);
            }
        }

        private Vector2 GetSummonPosition(int index, int count)
        {
            float angle = count <= 0 ? 0f : Mathf.PI * 2f * index / count;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            return (Vector2)transform.position + direction * spawnRadius;
        }

        private int GetSummonPrefabCount()
        {
            int count = 0;

            if (summonPrefabs != null)
            {
                for (int i = 0; i < summonPrefabs.Length; i++)
                {
                    if (summonPrefabs[i] != null)
                        count++;
                }
            }

            if (count <= 0 && summonPrefab != null)
                count = 1;

            return count;
        }

        private GameObject GetRandomSummonPrefab()
        {
            int count = GetSummonPrefabCount();

            if (count <= 0)
                return null;

            int targetIndex = Random.Range(0, count);

            if (summonPrefabs != null)
            {
                for (int i = 0; i < summonPrefabs.Length; i++)
                {
                    if (summonPrefabs[i] == null)
                        continue;

                    if (targetIndex == 0)
                        return summonPrefabs[i];

                    targetIndex--;
                }
            }

            return summonPrefab;
        }

        private void RemoveMissingSummons()
        {
            for (int i = activeSummons.Count - 1; i >= 0; i--)
            {
                if (activeSummons[i] == null)
                    activeSummons.RemoveAt(i);
            }
        }

        private void HandleSummonRemoved(BossSummonTracker summon)
        {
            activeSummons.Remove(summon);
        }

        public void ConfigureSummonSfx(SfxType prepareSfxType, SfxType summonSfxType)
        {
            playPrepareSfx = true;
            this.prepareSfxType = prepareSfxType;
            playSummonSfx = true;
            this.summonSfxType = summonSfxType;
        }

        public void ConfigureSummonSfx(SfxType summonSfxType)
        {
            playSummonSfx = true;
            this.summonSfxType = summonSfxType;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            summonCount = Mathf.Max(0, summonCount);
            minSummonCount = Mathf.Clamp(minSummonCount, 0, summonCount);
            phaseBonusSummonCount = Mathf.Max(0, phaseBonusSummonCount);
            maxActiveSummons = Mathf.Max(0, maxActiveSummons);
            spawnRadius = Mathf.Max(0f, spawnRadius);
            summonInterval = Mathf.Max(0f, summonInterval);
            summonTelegraphDelay = Mathf.Max(0f, summonTelegraphDelay);
        }
    }
}
