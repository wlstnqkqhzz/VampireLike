using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스 주변에 일반 적을 제한된 수만큼 소환하는 패턴이다.
    /// </summary>
    public class SummonPattern : BossPattern
    {
        [SerializeField]
        private GameObject summonPrefab;

        [SerializeField]
        private int summonCount = 3;

        [SerializeField]
        private int phaseBonusSummonCount = 1;

        [SerializeField]
        private int maxActiveSummons = 8;

        [SerializeField]
        private float spawnRadius = 1.6f;

        [SerializeField]
        private float summonInterval = 0.15f;

        private readonly List<BossSummonTracker> activeSummons = new List<BossSummonTracker>();

        protected override bool CanExecutePattern()
        {
            RemoveMissingSummons();
            return summonPrefab != null && activeSummons.Count < maxActiveSummons;
        }

        protected override IEnumerator ExecutePattern()
        {
            Boss.SetState(BossState.Preparing, false);
            RemoveMissingSummons();

            int availableSlots = maxActiveSummons - activeSummons.Count;
            int count = Mathf.Min(availableSlots, summonCount + Mathf.Max(0, Boss.CurrentPhase - 1) * phaseBonusSummonCount);

            for (int i = 0; i < count && !Boss.IsDead; i++)
            {
                Vector2 spawnPosition = GetSummonPosition(i, count);
                GameObject summon = Instantiate(summonPrefab, spawnPosition, Quaternion.identity);
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

        protected override void OnValidate()
        {
            base.OnValidate();
            summonCount = Mathf.Max(0, summonCount);
            phaseBonusSummonCount = Mathf.Max(0, phaseBonusSummonCount);
            maxActiveSummons = Mathf.Max(0, maxActiveSummons);
            spawnRadius = Mathf.Max(0f, spawnRadius);
            summonInterval = Mathf.Max(0f, summonInterval);
        }
    }
}
