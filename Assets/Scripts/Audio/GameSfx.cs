using System.Collections.Generic;
using UnityEngine;
using VampireLike.Settings;

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
        ShieldBlock,
        KaelSwordWave,
        SeleneDaggerThrow,
        ShieldReady,
        ShieldBreak,
        SkillExplosion,
        SkillRicochet,
        SkillScatter,
        SkillOrbitBlade,
        SkillShockwave,
        SkillFrost,
        SkillVampirism,
        BossDash,
        BossZone,
        BossProjectile,
        BossTeleport,
        BossDeath
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
            { SfxType.ShieldBlock, "shield_block_sfx" },
            { SfxType.KaelSwordWave, "kael_sword_wave" },
            { SfxType.SeleneDaggerThrow, "selene_dagger_throw" },
            { SfxType.ShieldReady, "shield_ready_sfx" },
            { SfxType.ShieldBreak, "shield_break_sfx" },
            { SfxType.SkillExplosion, "skill_explosion" },
            { SfxType.SkillRicochet, "skill_ricochet" },
            { SfxType.SkillScatter, "skill_scatter" },
            { SfxType.SkillOrbitBlade, "skill_orbit_blade" },
            { SfxType.SkillShockwave, "skill_shockwave" },
            { SfxType.SkillFrost, "skill_frost" },
            { SfxType.SkillVampirism, "skill_vampirism" },
            { SfxType.BossDash, "boss_dash" },
            { SfxType.BossZone, "boss_zone" },
            { SfxType.BossProjectile, "boss_projectile" },
            { SfxType.BossTeleport, "boss_teleport" },
            { SfxType.BossDeath, "boss_death" }
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
            { SfxType.ShieldBlock, 0.1f },
            { SfxType.KaelSwordWave, 0.08f },
            { SfxType.SeleneDaggerThrow, 0.05f },
            { SfxType.ShieldReady, 0.35f },
            { SfxType.ShieldBreak, 0.18f },
            { SfxType.SkillExplosion, 0.12f },
            { SfxType.SkillRicochet, 0.08f },
            { SfxType.SkillScatter, 0.08f },
            { SfxType.SkillOrbitBlade, 0.045f },
            { SfxType.SkillShockwave, 0.16f },
            { SfxType.SkillFrost, 0.12f },
            { SfxType.SkillVampirism, 0.18f },
            { SfxType.BossDash, 0.18f },
            { SfxType.BossZone, 0.2f },
            { SfxType.BossProjectile, 0.12f },
            { SfxType.BossTeleport, 0.2f },
            { SfxType.BossDeath, 0.5f }
        };

        private static readonly Dictionary<SfxType, float> Volumes = new Dictionary<SfxType, float>
        {
            { SfxType.PlayerShoot, 0.45f },
            { SfxType.EnemyHit, 0.34f },
            { SfxType.EnemyDeath, 0.62f },
            { SfxType.PlayerHit, 0.55f },
            { SfxType.ExperiencePickup, 0.32f },
            { SfxType.LevelUp, 0.62f },
            { SfxType.UpgradeSelect, 0.62f },
            { SfxType.BossAppear, 0.72f },
            { SfxType.GameOver, 0.72f },
            { SfxType.Heal, 0.48f },
            { SfxType.ShieldBlock, 0.58f },
            { SfxType.KaelSwordWave, 0.48f },
            { SfxType.SeleneDaggerThrow, 0.36f },
            { SfxType.ShieldReady, 0.48f },
            { SfxType.ShieldBreak, 0.52f },
            { SfxType.SkillExplosion, 0.5f },
            { SfxType.SkillRicochet, 0.38f },
            { SfxType.SkillScatter, 0.42f },
            { SfxType.SkillOrbitBlade, 0.58f },
            { SfxType.SkillShockwave, 0.5f },
            { SfxType.SkillFrost, 0.4f },
            { SfxType.SkillVampirism, 0.44f },
            { SfxType.BossDash, 0.52f },
            { SfxType.BossZone, 0.5f },
            { SfxType.BossProjectile, 0.42f },
            { SfxType.BossTeleport, 0.48f },
            { SfxType.BossDeath, 0.68f }
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

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticState()
        {
            instance = null;
            LoadedClips.Clear();
            LastPlayTimes.Clear();
        }

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

        private void OnDestroy()
        {
            if (instance == this)
                instance = null;
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
            source.PlayOneShot(clip, GetVolume(type) * masterVolume * GameOptions.MasterVolume * GameOptions.SfxVolume);
            LastPlayTimes[type] = Time.unscaledTime;
        }

        private static bool CanPlay(SfxType type)
        {
            float minInterval = MinIntervals.TryGetValue(type, out float value) ? value : 0f;

            if (!LastPlayTimes.TryGetValue(type, out float lastPlayTime))
                return true;

            if (Time.unscaledTime < lastPlayTime)
            {
                LastPlayTimes.Remove(type);
                return true;
            }

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
