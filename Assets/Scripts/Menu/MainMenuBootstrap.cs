using UnityEngine;
using UnityEngine.SceneManagement;

namespace VampireLike.Menu
{
    /// <summary>
    /// 게임 씬이 열리면 메인 메뉴 UI를 자동으로 생성합니다.
    /// </summary>
    public static class MainMenuBootstrap
    {
        private const string GameSceneName = "SampleScene";
        private const string MainMenuObjectName = "Main Menu";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateMainMenu()
        {
            Scene activeScene = SceneManager.GetActiveScene();

            if (activeScene.name != GameSceneName)
                return;

            if (UnityEngine.Object.FindFirstObjectByType<MainMenuUI>() != null)
                return;

            GameObject menuObject = new GameObject(MainMenuObjectName);
            menuObject.AddComponent<MainMenuUI>();
        }
    }
}
