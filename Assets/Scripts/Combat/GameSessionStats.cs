using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VampireLike.Combat
{
    /// <summary>
    /// Tracks simple run result values that are shown on the game over screen.
    /// </summary>
    public static class GameSessionStats
    {
        private static float startedAt;
        private static float endedAt;
        private static readonly Dictionary<string, int> selectedUpgradeCounts = new Dictionary<string, int>();

        public static int KillCount { get; private set; }
        public static int EnemyKillCount { get; private set; }
        public static int BossKillCount { get; private set; }
        public static int TotalExperienceGained { get; private set; }
        public static bool HasEnded { get; private set; }
        public static float SurvivalTime => Mathf.Max(0f, (HasEnded ? endedAt : Time.time) - startedAt);

        public static void Reset()
        {
            startedAt = Time.time;
            endedAt = startedAt;
            KillCount = 0;
            EnemyKillCount = 0;
            BossKillCount = 0;
            TotalExperienceGained = 0;
            HasEnded = false;
            selectedUpgradeCounts.Clear();
        }

        public static void RecordKill(bool isBoss)
        {
            if (HasEnded)
                return;

            KillCount++;

            if (isBoss)
                BossKillCount++;
            else
                EnemyKillCount++;
        }

        public static void RecordExperience(int amount)
        {
            if (HasEnded || amount <= 0)
                return;

            TotalExperienceGained += amount;
        }

        public static void RecordUpgrade(string displayName)
        {
            if (HasEnded || string.IsNullOrWhiteSpace(displayName))
                return;

            if (!selectedUpgradeCounts.ContainsKey(displayName))
                selectedUpgradeCounts[displayName] = 0;

            selectedUpgradeCounts[displayName]++;
        }

        public static string GetUpgradeSummary()
        {
            if (selectedUpgradeCounts.Count == 0)
                return "선택한 강화 없음";

            StringBuilder builder = new StringBuilder();

            foreach (KeyValuePair<string, int> pair in selectedUpgradeCounts)
            {
                if (builder.Length > 0)
                    builder.Append(" / ");

                builder.Append(pair.Key);

                if (pair.Value > 1)
                    builder.Append(" x").Append(pair.Value);
            }

            return builder.ToString();
        }

        public static void EndRun()
        {
            if (HasEnded)
                return;

            endedAt = Time.time;
            HasEnded = true;
        }
    }
}
