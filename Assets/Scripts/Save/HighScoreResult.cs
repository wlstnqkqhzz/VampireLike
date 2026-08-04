namespace VampireLike.Save
{
    /// <summary>
    /// 한 판이 끝났을 때 최고 기록이 갱신되었는지 알려주는 결과입니다.
    /// </summary>
    public readonly struct HighScoreResult
    {
        public HighScoreResult(bool newOverallRecord, bool newCharacterRecord)
        {
            NewOverallRecord = newOverallRecord;
            NewCharacterRecord = newCharacterRecord;
        }

        public bool NewOverallRecord { get; }
        public bool NewCharacterRecord { get; }
        public bool HasNewRecord => NewOverallRecord || NewCharacterRecord;
    }
}
