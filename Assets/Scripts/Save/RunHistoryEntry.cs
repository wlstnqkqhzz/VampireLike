using System;

namespace VampireLike.Save
{
    /// <summary>
    /// 게임 오버 시점의 한 판 결과를 저장하기 위한 기록 데이터입니다.
    /// </summary>
    [Serializable]
    public class RunHistoryEntry
    {
        public RunHistoryEntry(
            string playedAt,
            string characterId,
            string characterName,
            string characterRole,
            float survivalTime,
            int wave,
            int level,
            int kills,
            int enemyKills,
            int bossKills,
            int experience,
            string upgradeSummary)
        {
            this.playedAt = playedAt;
            this.characterId = characterId;
            this.characterName = characterName;
            this.characterRole = characterRole;
            this.survivalTime = survivalTime;
            this.wave = wave;
            this.level = level;
            this.kills = kills;
            this.enemyKills = enemyKills;
            this.bossKills = bossKills;
            this.experience = experience;
            this.upgradeSummary = upgradeSummary;
        }

        public string playedAt;
        public string characterId;
        public string characterName;
        public string characterRole;
        public float survivalTime;
        public int wave;
        public int level;
        public int kills;
        public int enemyKills;
        public int bossKills;
        public int experience;
        public string upgradeSummary;
    }
}
