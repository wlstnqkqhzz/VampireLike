using System.Collections.Generic;
using UnityEngine;

namespace VampireLike.Audio
{
    public enum SfxType
    {
        PlayerShoot,
        EnemyHit,
        EnemyDeath,
        PlayerHit,
        ExperiencePickup,
        LevelUp,
        UpgradeSelect,
        BossAppear,
        GameOver,
        Heal,
        ShieldBlock
    }

    /// <summary>
    /// 게임 전반에서 사용하는 짧은 효과음을 한 곳에서 재생합니다.
    /// 같은 효과음이 너무 빽빽하게 겹치지 않도록 최소 재생 간격도 함께 관리합니다.
    /// </summary>
    public class GameSfx : MonoBehaviour
    {
        private const string ResourceRoot = "Sounds/";
        private static readonly Dictionary<SfxType, string> ClipNames = new Dictionary<SfxType, string>
        {
            { SfxType.PlayerShoot, "player_shoot" },
            { SfxType.EnemyHit, "enemy_hit" },
            { SfxType.EnemyDeath, "enemy_death" },
            { SfxType.PlayerHit, "player_hit" },
            { SfxType.ExperiencePickup, "experience_pickup" },
            { SfxType.LevelUp, "level_up" },
            { SfxType.UpgradeSelect, "upgrade_select" },
            { SfxType.BossAppear, "boss_appear" },
            { SfxType.GameOver, "game_over" },
            { SfxType.Heal, "heal" },
            { SfxType.ShieldBlock, "shield_block" }
        };

        private static readonly Dictionary<SfxType, float> MinIntervals = new Dictionary<SfxType, float>
        {
            { SfxType.PlayerShoot, 0.06f },
            { SfxType.EnemyHit, 0.035f },
            { SfxType.EnemyDeath, 0.05f },
            { SfxType.PlayerHit, 0.12f },
            { SfxType.ExperiencePickup, 0.025f },
            { SfxType.LevelUp, 0.25f },
            { SfxType.UpgradeSelect, 0.08f },
            { SfxType.BossAppear, 0.5f },
            { SfxType.GameOver, 0.5f },
            { SfxType.Heal, 0.1f },
            { SfxType.ShieldBlock, 0.1f }
        };

        private static readonly Dictionary<SfxType, float> Volumes = new Dictionary<SfxType, float>
        {
            { SfxType.PlayerShoot, 0.45f },
            { SfxType.EnemyHit, 0.34f },
            { SfxType.EnemyDeath, 0.42f },
            { SfxType.PlayerHit, 0.55f },
            { SfxType.ExperiencePickup, 0.32f },
            { SfxType.LevelUp, 0.62f },
            { SfxType.UpgradeSelect, 0.5f },
            { SfxType.BossAppear, 0.72f },
            { SfxType.GameOver, 0.72f },
            { SfxType.Heal, 0.48f },
            { SfxType.ShieldBlock, 0.58f }
        };

        private static readonly Dictionary<SfxType, AudioClip> LoadedClips = new Dictionary<SfxType, AudioClip>();
        private static readonly Dictionary<SfxType, float> LastPlayTimes = new Dictionary<SfxType, float>();
        private static GameSfx instance;

        [SerializeField]
        private float masterVolume = 0.8f;

        [SerializeField]
        private int audioSourceCount = 8;

        private readonly List<AudioSource> audioSources = new List<AudioSource>();
        private int nextSourceIndex;

        public static void Play(SfxType type)
        {
            EnsureInstance();
            instance.PlayInternal(type);
        }

        private static void EnsureInstance()
        {
            if (instance != null)
                return;

            GameObject sfxObject = new GameObject("Game SFX");
            instance = sfxObject.AddComponent<GameSfx>();
            DontDestroyOnLoad(sfxObject);
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            EnsureAudioSources();
        }

        private void OnValidate()
        {
            masterVolume = Mathf.Clamp01(masterVolume);
            audioSourceCount = Mathf.Clamp(audioSourceCount, 1, 24);
        }

        private void PlayInternal(SfxType type)
        {
            if (!CanPlay(type))
                return;

            AudioClip clip = LoadClip(type);

            if (clip == null)
                return;

            EnsureAudioSources();
            AudioSource source = GetNextSource();
            source.pitch = Random.Range(0.96f, 1.04f);
            source.PlayOneShot(clip, GetVolume(type) * masterVolume);
            LastPlayTimes[type] = Time.unscaledTime;
        }

        private static bool CanPlay(SfxType type)
        {
            float minInterval = MinIntervals.TryGetValue(type, out float value) ? value : 0f;

            if (!LastPlayTimes.TryGetValue(type, out float lastPlayTime))
                return true;

            return Time.unscaledTime - lastPlayTime >= minInterval;
        }

        private static float GetVolume(SfxType type)
        {
            return Volumes.TryGetValue(type, out float volume) ? volume : 0.5f;
        }

        private static AudioClip LoadClip(SfxType type)
        {
            if (LoadedClips.TryGetValue(type, out AudioClip loadedClip))
                return loadedClip;

            if (!ClipNames.TryGetValue(type, out string clipName))
                return null;

            AudioClip clip = Resources.Load<AudioClip>(ResourceRoot + clipName);
            LoadedClips[type] = clip;
            return clip;
        }

        private void EnsureAudioSources()
        {
            while (audioSources.Count < audioSourceCount)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.loop = false;
                source.spatialBlend = 0f;
                audioSources.Add(source);
            }
        }

        private AudioSource GetNextSource()
        {
            AudioSource source = audioSources[nextSourceIndex];
            nextSourceIndex = (nextSourceIndex + 1) % audioSources.Count;
            return source;
        }
    }
}
