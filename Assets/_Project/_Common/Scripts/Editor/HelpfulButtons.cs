#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;

public class HelpfulButtons : OdinEditorWindow
{
    [MenuItem("Tools/Helpful Buttons")]
    private static void OpenWindow()
    {
        GetWindow<HelpfulButtons>().Show();
    }
    
    [ButtonGroup("Scenes")]
    private void Test2Scene()
    {
        LoadScene("Assets/_Project/_Common/Scenes/Test2.unity");
    }
    
    [ButtonGroup("Scenes")]
    private void MainMenu()
    {
        LoadScene("Assets/_Project/_Common/Scenes/MainMenu.unity");
    }
    
    [ButtonGroup("Scenes")]
    private void OnlineMatch()
    {
        LoadScene("Assets/_Project/OnlineMatch/OnlineMatch 1.unity");
    }

    private void LoadScene(string scenePath)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }
}
#endif