using System;
using System.Collections.Generic;
using UnityEngine;

namespace VampireLike.Save
{
    /// <summary>
    /// 최근 플레이 결과를 PlayerPrefs에 저장하고 불러오는 저장소입니다.
    /// </summary>
    public static class RunHistoryManager
    {
        private const string HistoryKey = "RunHistory";
        private const int MaxHistoryCount = 20;

        [Serializable]
        private class RunHistoryData
        {
            public List<RunHistoryEntry> entries = new List<RunHistoryEntry>();
        }

        public static void AddRun(RunHistoryEntry entry)
        {
            if (entry == null)
                return;

            RunHistoryData data = LoadData();
            data.entries.Insert(0, entry);

            if (data.entries.Count > MaxHistoryCount)
                data.entries.RemoveRange(MaxHistoryCount, data.entries.Count - MaxHistoryCount);

            SaveData(data);
        }

        public static IReadOnlyList<RunHistoryEntry> GetRecentRuns()
        {
            return LoadData().entries;
        }

        public static RunHistoryEntry GetLatestRun()
        {
            RunHistoryData data = LoadData();
            return data.entries.Count == 0 ? null : data.entries[0];
        }

        public static void ResetHistory()
        {
            PlayerPrefs.DeleteKey(HistoryKey);
            PlayerPrefs.Save();
        }

        private static RunHistoryData LoadData()
        {
            string json = PlayerPrefs.GetString(HistoryKey, string.Empty);

            if (string.IsNullOrWhiteSpace(json))
                return new RunHistoryData();

            try
            {
                RunHistoryData data = JsonUtility.FromJson<RunHistoryData>(json);
                return data ?? new RunHistoryData();
            }
            catch (Exception)
            {
                return new RunHistoryData();
            }
        }

        private static void SaveData(RunHistoryData data)
        {
            PlayerPrefs.SetString(HistoryKey, JsonUtility.ToJson(data));
            PlayerPrefs.Save();
        }
    }
}
