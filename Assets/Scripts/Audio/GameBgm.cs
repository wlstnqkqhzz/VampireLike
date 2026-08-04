using UnityEngine;
using VampireLike.Settings;

namespace VampireLike.Audio
{
    public enum BgmType
    {
        MainMenu
    }

    public class GameBgm : MonoBehaviour
    {
        private const string MusicRoot = "Music/";
        private const string MainMenuClipName = "main_menu_bgm";

        private static GameBgm instance;

        [SerializeField]
        private float bgmVolume = 0.55f;

        private AudioSource audioSource;
        private BgmType? currentBgm;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticState()
        {
            instance = null;
        }

        public static void Play(BgmType type)
        {
            EnsureInstance();
            instance.PlayInternal(type);
        }

        public static void Stop()
        {
            if (instance == null)
                return;

            instance.currentBgm = null;

            if (instance.audioSource != null)
                instance.audioSource.Stop();
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

            audioSource.volume = bgmVolume * GameOptions.MasterVolume;
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

        private void PlayInternal(BgmType type)
        {
            EnsureAudioSource();

            if (currentBgm == type && audioSource.isPlaying)
                return;

            AudioClip clip = LoadClip(type);

            if (clip == null)
                return;

            currentBgm = type;
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.volume = bgmVolume * GameOptions.MasterVolume;
            audioSource.Play();
        }

        private static AudioClip LoadClip(BgmType type)
        {
            string clipName = type == BgmType.MainMenu ? MainMenuClipName : string.Empty;

            if (string.IsNullOrWhiteSpace(clipName))
                return null;

            return Resources.Load<AudioClip>(MusicRoot + clipName);
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
    }
}
