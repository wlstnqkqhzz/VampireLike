namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스의 현재 행동 상태다. 패턴 실행 중 기본 추적 이동과 충돌 처리를 구분하는 기준으로 사용한다.
    /// </summary>
    public enum BossState
    {
        Chasing,
        Preparing,
        Attacking,
        Recovering,
        Teleporting,
        Burrowed,
        PhaseChanging,
        Dead
    }
}
