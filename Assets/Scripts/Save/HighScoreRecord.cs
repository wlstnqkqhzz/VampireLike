namespace VampireLike.Save
{
    /// <summary>
    /// 최고 기록 화면과 결과 화면에서 공통으로 사용하는 저장 기록 데이터입니다.
    /// </summary>
    public readonly struct HighScoreRecord
    {
        public HighScoreRecord(
            float survivalTime,
            int wave,
            int level,
            int kills,
            int bossKills,
            int experience)
        {
            SurvivalTime = survivalTime;
            Wave = wave;
            Level = level;
            Kills = kills;
            BossKills = bossKills;
            Experience = experience;
        }

        public float SurvivalTime { get; }
        public int Wave { get; }
        public int Level { get; }
        public int Kills { get; }
        public int BossKills { get; }
        public int Experience { get; }
        public bool HasAnyRecord => SurvivalTime > 0f || Wave > 0 || Level > 0 || Kills > 0 || BossKills > 0 || Experience > 0;
    }
}
