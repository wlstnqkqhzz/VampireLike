namespace VampireLike.Combat
{
    /// <summary>
    /// 여러 전투 스크립트가 함께 참고하는 전역 게임 진행 상태를 관리한다.
    /// </summary>
    public static class GameState
    {
        // 게임 오버가 되면 적 이동, 투사체 이동, 자동 공격 등 주요 전투 동작을 멈춘다.
        public static bool IsGameOver { get; private set; }

        /// <summary>
        /// 새 플레이를 시작할 때 게임 오버 상태를 초기화한다.
        /// </summary>
        public static void ResetGame()
        {
            IsGameOver = false;
        }

        /// <summary>
        /// 플레이어 사망 등으로 게임 오버 상태에 진입한다.
        /// </summary>
        public static void SetGameOver()
        {
            IsGameOver = true;
        }
    }
}
