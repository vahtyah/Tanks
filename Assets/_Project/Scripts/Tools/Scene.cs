﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene
{
    public enum SceneName
    {
        MainMenu,
        MultiDisplay,
        SingleDisplay,
        BotMatch,
        OnlineMatch,
    }
    public static void Load(string sceneToLoad)
    {
        Application.backgroundLoadingPriority = ThreadPriority.High;
        SceneManager.LoadScene(sceneToLoad);
    }
}