using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene
{
    public enum SceneName
    {
        MainMenu,
        LocalMatch,
        BotMatch,
        CaptureTheFlag,
        Deathmatch,
        TeamDeathmatch,
        PvE,
    }
    public static void Load(SceneName sceneToLoad)
    {
        Application.backgroundLoadingPriority = ThreadPriority.High;
        SceneManager.LoadScene(sceneToLoad.ToString());
    }
    
    public static void LoadCurrentScene()
    {
        Application.backgroundLoadingPriority = ThreadPriority.High;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}