using UnityEngine;

namespace VampireLike.Save
{
    /// <summary>
    /// PlayerPrefs를 이용해 전체 최고 기록과 캐릭터별 최고 기록을 저장합니다.
    /// </summary>
    public static class HighScoreManager
    {
        private const string OverallScope = "overall";
        private const string KeyPrefix = "HighScore";

        public static HighScoreRecord GetOverallRecord()
        {
            return LoadRecord(OverallScope);
        }

        public static HighScoreRecord GetCharacterRecord(string characterId)
        {
            return LoadRecord(GetCharacterScope(characterId));
        }

        public static HighScoreResult SubmitRun(string characterId, HighScoreRecord runRecord)
        {
            bool newOverallRecord = SaveBestValues(OverallScope, runRecord);
            bool newCharacterRecord = SaveBestValues(GetCharacterScope(characterId), runRecord);
            PlayerPrefs.Save();
            return new HighScoreResult(newOverallRecord, newCharacterRecord);
        }

        public static void ResetAllRecords()
        {
            DeleteRecord(OverallScope);
            DeleteRecord("character.kael");
            DeleteRecord("character.selene");
            RunHistoryManager.ResetHistory();
            PlayerPrefs.Save();
        }

        private static HighScoreRecord LoadRecord(string scope)
        {
            return new HighScoreRecord(
                PlayerPrefs.GetFloat(GetKey(scope, "SurvivalTime"), 0f),
                PlayerPrefs.GetInt(GetKey(scope, "Wave"), 0),
                PlayerPrefs.GetInt(GetKey(scope, "Level"), 0),
                PlayerPrefs.GetInt(GetKey(scope, "Kills"), 0),
                PlayerPrefs.GetInt(GetKey(scope, "BossKills"), 0),
                PlayerPrefs.GetInt(GetKey(scope, "Experience"), 0));
        }

        private static bool SaveBestValues(string scope, HighScoreRecord runRecord)
        {
            bool hasUpdated = false;
            hasUpdated |= SaveBestFloat(scope, "SurvivalTime", runRecord.SurvivalTime);
            hasUpdated |= SaveBestInt(scope, "Wave", runRecord.Wave);
            hasUpdated |= SaveBestInt(scope, "Level", runRecord.Level);
            hasUpdated |= SaveBestInt(scope, "Kills", runRecord.Kills);
            hasUpdated |= SaveBestInt(scope, "BossKills", runRecord.BossKills);
            hasUpdated |= SaveBestInt(scope, "Experience", runRecord.Experience);
            return hasUpdated;
        }

        private static bool SaveBestFloat(string scope, string metric, float value)
        {
            string key = GetKey(scope, metric);
            float current = PlayerPrefs.GetFloat(key, 0f);

            if (value <= current)
                return false;

            PlayerPrefs.SetFloat(key, value);
            return true;
        }

        private static bool SaveBestInt(string scope, string metric, int value)
        {
            string key = GetKey(scope, metric);
            int current = PlayerPrefs.GetInt(key, 0);

            if (value <= current)
                return false;

            PlayerPrefs.SetInt(key, value);
            return true;
        }

        private static void DeleteRecord(string scope)
        {
            PlayerPrefs.DeleteKey(GetKey(scope, "SurvivalTime"));
            PlayerPrefs.DeleteKey(GetKey(scope, "Wave"));
            PlayerPrefs.DeleteKey(GetKey(scope, "Level"));
            PlayerPrefs.DeleteKey(GetKey(scope, "Kills"));
            PlayerPrefs.DeleteKey(GetKey(scope, "BossKills"));
            PlayerPrefs.DeleteKey(GetKey(scope, "Experience"));
        }

        private static string GetCharacterScope(string characterId)
        {
            return $"character.{(string.IsNullOrWhiteSpace(characterId) ? "unknown" : characterId)}";
        }

        private static string GetKey(string scope, string metric)
        {
            return $"{KeyPrefix}.{scope}.{metric}";
        }
    }
}
