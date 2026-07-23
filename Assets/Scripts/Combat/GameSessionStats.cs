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

        public static int KillCount { get; private set; }
        public static bool HasEnded { get; private set; }
        public static float SurvivalTime => Mathf.Max(0f, (HasEnded ? endedAt : Time.time) - startedAt);

        public static void Reset()
        {
            startedAt = Time.time;
            endedAt = startedAt;
            KillCount = 0;
            HasEnded = false;
        }

        public static void RecordKill()
        {
            if (HasEnded)
                return;

            KillCount++;
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
