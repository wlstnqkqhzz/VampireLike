namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스 스테이지 배율로 패턴 피해량을 조정할 수 있는 컴포넌트 계약이다.
    /// </summary>
    public interface IBossDamageScaler
    {
        void ScaleBossDamage(float multiplier);
    }
}
