namespace VampireLike.Combat
{
    /// <summary>
    /// 여러 전투 스크립트가 함께 참고하는 전역 게임 진행 상태입니다.
    /// </summary>
    public static class GameState
    {
        public static bool IsGameOver { get; private set; }
        public static bool IsMainMenuOpen { get; private set; }

        /// <summary>
        /// 새 플레이를 시작할 때 게임 진행 상태를 초기화합니다.
        /// </summary>
        public static void ResetGame()
        {
            IsGameOver = false;
            IsMainMenuOpen = false;
            GameSessionStats.Reset();
        }

        /// <summary>
        /// 시작 메뉴가 열려 있는 동안 일시정지 메뉴와 전투 입력이 겹치지 않도록 표시합니다.
        /// </summary>
        public static void SetMainMenuOpen(bool isOpen)
        {
            IsMainMenuOpen = isOpen;
        }

        /// <summary>
        /// 플레이어 사망으로 게임 오버 상태에 진입합니다.
        /// </summary>
        public static void SetGameOver()
        {
            IsGameOver = true;
            IsMainMenuOpen = false;
            GameSessionStats.EndRun();
        }
    }
}
