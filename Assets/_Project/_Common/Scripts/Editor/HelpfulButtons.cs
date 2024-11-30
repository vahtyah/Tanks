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

    [TabGroup("Buttons", "Main Scenes")]
    [ResponsiveButtonGroup("Buttons/Main Scenes/TestScene")]
    private void Test2Scene()
    {
        LoadScene("Assets/_Project/_Common/Scenes/Test2.unity");
    }

    [ResponsiveButtonGroup("Buttons/Main Scenes/Main Menu")]
    private void MainMenu()
    {
        LoadScene("Assets/_Project/_Common/Scenes/MainMenu.unity");
    }

    [TabGroup("Buttons", "Game Modes")]
    [ResponsiveButtonGroup("Buttons/Game Modes/Capture The Flag")]
    private void TheCaptureTheFlag()
    {
        LoadScene("Assets/_Project/OnlineMatch/CaptureTheFlag.unity");
    }

    [ResponsiveButtonGroup("Buttons/Game Modes/Deathmatch")]
    private void TheDeathmatch()
    {
        LoadScene("Assets/_Project/OnlineMatch/Deathmatch.unity");
    }

    [TabGroup("Buttons", "Trainings")]
    [ResponsiveButtonGroup("Buttons/Trainings/Deathmatch Training")]
    private void Deathmatch()
    {
        LoadScene("Assets/_Project/Training/Scenes/DeathmatchTraining.unity");
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