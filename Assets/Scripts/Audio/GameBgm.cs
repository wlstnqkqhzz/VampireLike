using UnityEngine;
using VampireLike.Settings;

namespace VampireLike.Audio
{
    public enum BgmType
    {
        MainMenu,
        Battle,
        Boss,
        HiddenBoss,
        GameOver
    }

    public class GameBgm : MonoBehaviour
    {
        private const string MusicRoot = "Music/";
        private const string MainMenuClipName = "main_menu_bgm";
        private const string DefaultBattleClipName = "kael_battle_theme";
        private const string HiddenBossClipName = "hidden_boss_bgm";
        private const string GameOverClipName = "game_over_bgm";
        private const string BossClipPrefix = "boss_stage_";
        private const string BossClipSuffix = "_bgm";
        private const int MaxBossClipStage = 10;

        private static GameBgm instance;
        private static string battleClipName = DefaultBattleClipName;

        [SerializeField]
        private float bgmVolume = 0.55f;

        private AudioSource audioSource;
        private BgmType? currentBgm;
        private int currentBossStage;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticState()
        {
            instance = null;
            battleClipName = DefaultBattleClipName;
        }

        public static void Play(BgmType type)
        {
            EnsureInstance();
            instance.PlayInternal(type, 0);
        }

        public static void SetBattleClipName(string clipName)
        {
            if (string.IsNullOrWhiteSpace(clipName))
                clipName = DefaultBattleClipName;

            if (battleClipName == clipName)
                return;

            battleClipName = clipName;

            if (instance != null && instance.currentBgm == BgmType.Battle)
                instance.PlayInternal(BgmType.Battle, 0, true);
        }

        public static void PlayBoss(int bossStage)
        {
            EnsureInstance();
            instance.PlayInternal(BgmType.Boss, bossStage);
        }

        public static void PlayHiddenBoss()
        {
            EnsureInstance();
            instance.PlayInternal(BgmType.HiddenBoss, 0);
        }

        public static void Stop()
        {
            if (instance == null)
                return;

            instance.currentBgm = null;
            instance.currentBossStage = 0;
            instance.StopPlayback();
        }

        private static void EnsureInstance()
        {
            if (instance != null)
                return;

            GameObject bgmObject = new GameObject("Game BGM");
            instance = bgmObject.AddComponent<GameBgm>();
            DontDestroyOnLoad(bgmObject);
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                StopAllAudioSources();
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            EnsureAudioSource();
        }

        private void Update()
        {
            if (audioSource == null)
                return;

            audioSource.volume = GetAppliedVolume();
        }

        private void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }

        private void OnValidate()
        {
            bgmVolume = Mathf.Clamp01(bgmVolume);
        }

        private void PlayInternal(BgmType type, int bossStage)
        {
            PlayInternal(type, bossStage, false);
        }

        private void PlayInternal(BgmType type, int bossStage, bool forceRestart)
        {
            EnsureAudioSource();

            bossStage = Mathf.Max(0, bossStage);

            if (!forceRestart && currentBgm == type && currentBossStage == bossStage && audioSource.isPlaying)
                return;

            AudioClip clip = LoadClip(type, bossStage);

            if (clip == null)
            {
                currentBgm = type;
                currentBossStage = bossStage;
                StopPlayback();
                return;
            }

            currentBgm = type;
            currentBossStage = bossStage;
            StopPlayback();
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.volume = GetAppliedVolume();
            audioSource.Play();
        }

        private float GetAppliedVolume()
        {
            return bgmVolume * GameOptions.MasterVolume * GameOptions.BgmVolume;
        }

        private static AudioClip LoadClip(BgmType type, int bossStage)
        {
            switch (type)
            {
                case BgmType.MainMenu:
                    return Resources.Load<AudioClip>(MusicRoot + MainMenuClipName);
                case BgmType.Battle:
                    return LoadBattleClip();
                case BgmType.Boss:
                    return LoadBossClip(bossStage);
                case BgmType.HiddenBoss:
                    return Resources.Load<AudioClip>(MusicRoot + HiddenBossClipName);
                case BgmType.GameOver:
                    return Resources.Load<AudioClip>(MusicRoot + GameOverClipName);
                default:
                    return null;
            }
        }

        private static AudioClip LoadBossClip(int bossStage)
        {
            if (bossStage <= 0)
                return null;

            string clipName = $"{BossClipPrefix}{bossStage:00}{BossClipSuffix}";
            AudioClip clip = Resources.Load<AudioClip>(MusicRoot + clipName);
            if (clip != null || bossStage <= MaxBossClipStage)
                return clip;

            string fallbackClipName = $"{BossClipPrefix}{MaxBossClipStage:00}{BossClipSuffix}";
            return Resources.Load<AudioClip>(MusicRoot + fallbackClipName);
        }

        private static AudioClip LoadBattleClip()
        {
            AudioClip clip = Resources.Load<AudioClip>(MusicRoot + battleClipName);
            return clip != null ? clip : Resources.Load<AudioClip>(MusicRoot + DefaultBattleClipName);
        }

        private void EnsureAudioSource()
        {
            if (audioSource != null)
                return;

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.spatialBlend = 0f;
        }

        private void StopPlayback()
        {
            if (audioSource == null)
                return;

            audioSource.Stop();
            audioSource.clip = null;
        }

        private void StopAllAudioSources()
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            foreach (AudioSource source in sources)
            {
                source.Stop();
                source.clip = null;
            }
        }
    }
}
