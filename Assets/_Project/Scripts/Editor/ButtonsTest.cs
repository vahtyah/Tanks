#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SceneButton
{
    public string SceneName;
    public string ScenePath;
    public string GroupPath => ButtonsTest.TAB_GROUP_SCENES + "/" + SceneName;

    public SceneButton(string name, string path)
    {
        SceneName = name;
        ScenePath = path;
    }
    
    [TabGroup(ButtonsTest.TAB_GROUP_SCENES, "@SceneName")]
    [ResponsiveButtonGroup("@GroupPath")]
    public void LoadScene()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(ScenePath);
        }
    }

}

[System.Serializable]

public class Buttons
{
    public List<SceneButton> scenes = new List<SceneButton>();
}

public class ButtonsTest : OdinEditorWindow
{
    
    private const string DIRECTORY_SCENES = "Assets/_Project/Scenes";
    public const string BOX_GROUP_SCENES = "Scenes";
    public const string TAB_GROUP_SCENES = BOX_GROUP_SCENES + "/TabGroup";

    [MenuItem("Tools/Helpful Buttons1")]
    private static void OpenWindow()
    {
        GetWindow<ButtonsTest>().Show();
    }
    void AddScenesButtons()
    {
        string[] sceneFiles = Directory.GetFiles(DIRECTORY_SCENES, "*.unity");

        // Các danh sách scene phân loại theo chủ đề
        var mainScenes = new System.Collections.Generic.List<string>();
        var gameModes = new System.Collections.Generic.List<string>();
        var trainings = new System.Collections.Generic.List<string>();

        // Phân loại các scene vào các danh sách tương ứng
        foreach (string scenePath in sceneFiles)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath); // Lấy tên scene

            if (sceneName.Contains("MainMenu") || sceneName.Contains("Test"))
                mainScenes.Add(scenePath);
            else if (sceneName.Contains("Capture") || sceneName.Contains("Deathmatch"))
                gameModes.Add(scenePath);
            else if (sceneName.Contains("Training"))
                trainings.Add(scenePath);
        }

        // Tạo tab và nhóm nút cho các scene
        CreateSceneTab("Main Scenes", mainScenes);
        CreateSceneTab("Game Modes", gameModes);
        CreateSceneTab("Trainings", trainings);
    }

    // Tạo tab cho các scene trong mỗi nhóm
    private void CreateSceneTab(string tabName, System.Collections.Generic.List<string> scenes)
    {
        if (scenes.Count > 0)
        {
            GUILayout.BeginVertical();
            GUILayout.Label(tabName, EditorStyles.boldLabel);
            foreach (var scenePath in scenes)
            {
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                string buttonPath = TAB_GROUP_SCENES + "/" + tabName + "/" + sceneName;

                // Tạo nút cho mỗi scene
                if (GUILayout.Button(sceneName))
                {
                    LoadScene(scenePath);
                }
            }
            GUILayout.EndVertical();
        }
    }

    private void LoadScene(string scenePath)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }


    // [Obsolete("Rename this to OnImGUI()", true)]
    protected override void OnImGUI()
    {
        AddScenesButtons();  // Thêm các nút scene tự động vào cửa sổ
        base.OnImGUI();
    }
}

#endif