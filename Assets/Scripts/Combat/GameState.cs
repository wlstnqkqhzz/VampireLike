namespace VampireLike.Combat
{
    public static class GameState
    {
        public static bool IsGameOver { get; private set; }

        public static void ResetGame()
        {
            IsGameOver = false;
        }

        public static void SetGameOver()
        {
            IsGameOver = true;
        }
    }
}
