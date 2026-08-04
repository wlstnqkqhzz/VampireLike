using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VampireLike.Combat
{
    /// <summary>
    /// 게임 한 판의 결과 화면에 표시할 생존 기록을 모읍니다.
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
        public static string CharacterId { get; private set; } = "kael";
        public static string CharacterDisplayName { get; private set; } = "카엘";
        public static string CharacterRole { get; private set; } = "흑검 수호자";
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

        public static void RecordCharacter(string displayName, string role)
        {
            if (!string.IsNullOrWhiteSpace(displayName))
                CharacterDisplayName = displayName;

            if (!string.IsNullOrWhiteSpace(role))
                CharacterRole = role;
        }

        public static void RecordCharacter(string characterId, string displayName, string role)
        {
            if (!string.IsNullOrWhiteSpace(characterId))
                CharacterId = characterId;

            RecordCharacter(displayName, role);
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
            return GetUpgradeSummary(0, " / ");
        }

        public static string GetUpgradeSummary(int maxItems, string separator)
        {
            if (selectedUpgradeCounts.Count == 0)
                return "선택한 강화 없음";

            StringBuilder builder = new StringBuilder();
            int writtenCount = 0;

            foreach (KeyValuePair<string, int> pair in selectedUpgradeCounts)
            {
                if (maxItems > 0 && writtenCount >= maxItems)
                {
                    builder.Append(separator).Append($"외 {selectedUpgradeCounts.Count - writtenCount}개");
                    break;
                }

                if (builder.Length > 0)
                    builder.Append(separator);

                builder.Append(pair.Key);

                if (pair.Value > 1)
                    builder.Append(" x").Append(pair.Value);

                writtenCount++;
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
