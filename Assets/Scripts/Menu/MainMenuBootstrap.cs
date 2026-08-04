using UnityEngine;
using UnityEngine.SceneManagement;

namespace VampireLike.Menu
{
    /// <summary>
    /// 메인 메뉴 씬이 열리면 메인 메뉴 UI를 자동으로 생성합니다.
    /// </summary>
    public static class MainMenuBootstrap
    {
        private const string MainMenuSceneName = "MainMenuScene";
        private const string MainMenuObjectName = "Main Menu";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateMainMenuAfterInitialSceneLoad()
        {
            CreateMainMenu(SceneManager.GetActiveScene());
        }

        private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CreateMainMenu(scene);
        }

        private static void CreateMainMenu(Scene activeScene)
        {
            if (activeScene.name != MainMenuSceneName)
                return;

            if (UnityEngine.Object.FindFirstObjectByType<MainMenuUI>() != null)
                return;

            GameObject menuObject = new GameObject(MainMenuObjectName);
            menuObject.AddComponent<MainMenuUI>();
        }
    }
}
