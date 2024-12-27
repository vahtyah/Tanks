#if UNITY_EDITOR

using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.IO.Path;
using static UnityEditor.AssetDatabase;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using static System.Environment;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;


public class HelpfulButtons : OdinEditorWindow
{
    [MenuItem("Tools/Helpful Buttons")]
    private static void OpenWindow()
    {
        GetWindow<HelpfulButtons>().Show();
    }

    #region Scenes

    private const string DIRECTORY_SCENES = "Assets/_Project/Scenes";
    private const string BOX_GROUP_SCENES = "Scenes";
    private const string TAB_GROUP_SCENES = BOX_GROUP_SCENES + "/TabGroup";

    // GENERATED METHODS START

        /// <summary>
        /// Loads the CaptureTheFlag scene.
        /// Path: D:\Unity\Tanks\Assets\_Project\Scenes\Game Modes\CaptureTheFlag.unity
        /// </summary>
        [BoxGroup(BOX_GROUP_SCENES)]
        [TabGroup(TAB_GROUP_SCENES, "Game Modes")]
        [ResponsiveButtonGroup(TAB_GROUP_SCENES + "/Game Modes" + "/CaptureTheFlag")]
        public void LoadCaptureTheFlag()
        {
            LoadScene(DIRECTORY_SCENES + "/Game Modes" + "/CaptureTheFlag.unity");
        }

        /// <summary>
        /// Loads the Deathmatch scene.
        /// Path: D:\Unity\Tanks\Assets\_Project\Scenes\Game Modes\Deathmatch.unity
        /// </summary>
        [BoxGroup(BOX_GROUP_SCENES)]
        [TabGroup(TAB_GROUP_SCENES, "Game Modes")]
        [ResponsiveButtonGroup(TAB_GROUP_SCENES + "/Game Modes" + "/Deathmatch")]
        public void LoadDeathmatch()
        {
            LoadScene(DIRECTORY_SCENES + "/Game Modes" + "/Deathmatch.unity");
        }

        /// <summary>
        /// Loads the MainMenu scene.
        /// Path: D:\Unity\Tanks\Assets\_Project\Scenes\Main Menu\MainMenu.unity
        /// </summary>
        [BoxGroup(BOX_GROUP_SCENES)]
        [TabGroup(TAB_GROUP_SCENES, "Main Menu")]
        [ResponsiveButtonGroup(TAB_GROUP_SCENES + "/Main Menu" + "/MainMenu")]
        public void LoadMainMenu()
        {
            LoadScene(DIRECTORY_SCENES + "/Main Menu" + "/MainMenu.unity");
        }

        /// <summary>
        /// Loads the Test scene.
        /// Path: D:\Unity\Tanks\Assets\_Project\Scenes\Test\Test.unity
        /// </summary>
        [BoxGroup(BOX_GROUP_SCENES)]
        [TabGroup(TAB_GROUP_SCENES, "Test")]
        [ResponsiveButtonGroup(TAB_GROUP_SCENES + "/Test" + "/Test")]
        public void LoadTest()
        {
            LoadScene(DIRECTORY_SCENES + "/Test" + "/Test.unity");
        }

        /// <summary>
        /// Loads the Test2 scene.
        /// Path: D:\Unity\Tanks\Assets\_Project\Scenes\Test\Test2.unity
        /// </summary>
        [BoxGroup(BOX_GROUP_SCENES)]
        [TabGroup(TAB_GROUP_SCENES, "Test")]
        [ResponsiveButtonGroup(TAB_GROUP_SCENES + "/Test" + "/Test2")]
        public void LoadTest2()
        {
            LoadScene(DIRECTORY_SCENES + "/Test" + "/Test2.unity");
        }
// GENERATED METHODS END

    private void LoadScene(string scenePath)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }

    #endregion

    #region Buttons

    const string BOX_GROUP_BUTTONS = "Buttons";
    const string TAB_GROUP_BUTTONS = BOX_GROUP_BUTTONS + "/TabGroup";

    #region FreQuently Used

    [BoxGroup(BOX_GROUP_BUTTONS)]
    [TabGroup(TAB_GROUP_BUTTONS, "Button Used Frequently", order: 10)]
    [ResponsiveButtonGroup(TAB_GROUP_BUTTONS + "/Button Used Frequently/OpenCSharpProject",
        DefaultButtonSize = ButtonSizes.Large)]
    [Obsolete("Obsolete")]
    void OpenCSharpProject()
    {
        string projectDirectory =
            Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/", StringComparison.Ordinal));
        string projectName = new DirectoryInfo(projectDirectory).Name;
        string solutionPath = System.IO.Path.Combine(projectDirectory, $"{projectName}.sln");

        if (File.Exists(solutionPath))
        {
            Process.Start(solutionPath);
        }
        else
        {
            InternalEditorUtility.RequestScriptReload();
            Refresh();
            Process.Start(solutionPath);
        }
    }

    #endregion

    #region Rarely Used

    #region Create Hierachy Folders

    [FolderPath] [TabGroup(TAB_GROUP_BUTTONS, "Button Used Rarely")]
    public string SceenPath = "Assets/_Project/Scenes";

    [TabGroup(TAB_GROUP_BUTTONS, "Button Used Rarely")]
    public string SceneName;
    
    [TabGroup(TAB_GROUP_BUTTONS, "Button Used Rarely")]
    [ResponsiveButtonGroup(TAB_GROUP_BUTTONS + "/Button Used Rarely/CreateHierarchyFolders",
        DefaultButtonSize = ButtonSizes.Large)]
    // [Button(ButtonSizes.Large, ButtonStyle.Box, Expanded = true)]
    void CreateAndSetupScene()
    {
        if (string.IsNullOrEmpty(SceneName))
        {
            EditorUtility.DisplayDialog("Error", "Scene name cannot be empty.", "OK");
            return;
        }

        string path = "";

        if (IsValidFolder(SceenPath))
        {
            path = Combine(SceenPath, SceneName + ".unity");

            if (File.Exists(path))
            {
                bool overwriteConfirmed = EditorUtility.DisplayDialog("Scene Exists",
                    $"Scene '{SceneName}' already exists. Do you want to overwrite it?", "Yes", "No");
                if (!overwriteConfirmed)
                {
                    return;
                }
            }
        }
        else
        {
            Debug.LogError($"Path: {SceenPath} does not exist.");
            return;
        }


        UnityEngine.SceneManagement.Scene newScene =
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        EditorSceneManager.SaveScene(newScene, path);
        // EditorSceneManager.OpenScene(path);

        Refresh();

        Hierarchy.Create("Display", "Managers", "Environment");
        Hierarchy.CreateChild("Display", "Camera", "GUI");
        Hierarchy.Move("Main Camera", "Camera");
        Hierarchy.Move("Directional Light", "Camera");
        Hierarchy.CreateChild("Managers", "TimeManager", "GameManager", "PoolManager", "AudioManager");

        Hierarchy.CreateCanvas();

        Debug.Log($"Scene '{SceneName}' has been created and set up.");
        
        SceneFunctionGenerator.UpdateSceneMethods();
    }
    
    [ResponsiveButtonGroup(TAB_GROUP_BUTTONS + "/Button Used Rarely/CreateHierarchyFolders",
        DefaultButtonSize = ButtonSizes.Large)]
    void ReloadScenes()
    {
        SceneFunctionGenerator.UpdateSceneMethods();
    }
    
    [BoxGroup(BOX_GROUP_BUTTONS)]
    [TabGroup(TAB_GROUP_BUTTONS, "Button Used Rarely", order: 40)]
    [ResponsiveButtonGroup(TAB_GROUP_BUTTONS + "/Button Used Rarely/CreateFolders", VisibleIf = "IsDirectoryOfScriptsDoNotExists",
        DefaultButtonSize = ButtonSizes.Large)]
    public void CreateFolders()
    {
        Folders.Create("_Project", "Animation", "Materials", "Prefabs");
        Refresh();
        Folders.Move("Scenes", "_Project"); 
        Refresh();
        // Optional: Disable Domain Reload
        // EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload | EnterPlayModeOptions.DisableSceneReload;
    }

    private bool IsDirectoryOfScriptsDoNotExists() =>
        !Directory.Exists(System.IO.Path.Combine(Application.dataPath, "_Project"));

    [ResponsiveButtonGroup(TAB_GROUP_BUTTONS + "/Button Used Rarely/ImportExtensionEditor",
        VisibleIf = "IsDirectoryOfEditorExtensionsDoNotExists",
        DefaultButtonSize = ButtonSizes.Large)]
    public void ImportExtensionEditor()
    {
        Assets.ImportAsset("vFolders 2", "kubacho lab/Editor ExtensionsUtilities");
        Assets.ImportAsset("vInspector 2", "kubacho lab/Editor ExtensionsUtilities");
        Assets.ImportAsset("vHierarchy 2", "NoLicense");
        Assets.ImportAsset("Editor Console Pro v3.975", "NoLicense");
        Folders.Create("Plugins", "Editor Enhancers");
        Folders.Move("vFolders 2", "Editor Enhancers");
        Folders.Move("vInspector 2", "Editor Enhancers");
        Folders.Move("vHierarchy 2", "Editor Enhancers");
        Folders.Move("ConsolePro", "Plugins");
        Refresh();
    }
    
    bool IsDirectoryOfEditorExtensionsDoNotExists() =>
        !Directory.Exists(System.IO.Path.Combine(Application.dataPath, "Plugins/Editor Enhancers"));
    
    #endregion

    #endregion

    #endregion
}


#else
public class HelpfulButtons : EditorWindow
{
    [MenuItem("Tools/Helpful Buttons")]
    private static void OpenWindow()
    {
        GetWindow<HelpfulButtons>().Show();
    }

    private void InstallOdinInspector()
    {
        GUILayout.BeginVertical();

        // Tạo một style đẹp cho nút
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        // buttonStyle.normal.textColor = Color.white;
        // buttonStyle.normal.background = MakeTexture(2, 2, Color.white);
        // buttonStyle.hover.background = MakeTexture(2, 2, Color.blue);
        buttonStyle.fontSize = 14;
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.padding = new RectOffset(10, 10, 5, 5);
        buttonStyle.margin = new RectOffset(10, 10, 10, 5);

        if (GUILayout.Button(new GUIContent("Create Folders", "Click to create project folders"), buttonStyle))
        {
            Folders.Create("_Project", "Animation", "Materials", "Prefabs");
            Refresh();
            Folders.Move("Scenes", "_Project");
            Refresh();
        }
        
        if (GUILayout.Button(new GUIContent("Install Odin Inspector", "Click to install Odin Inspector"), buttonStyle))
        {
            Assets.ImportAsset("Odin Inspector and Serializer.unitypackage", "Sirenix/Editor ExtensionsSystem");
        }
        
        if (GUILayout.Button(new GUIContent("Install Editor Extensions", "Click to install Editor Extensions"), buttonStyle))
        {
            Assets.ImportAsset("vFolders 2", "kubacho lab/Editor ExtensionsUtilities");
            Assets.ImportAsset("vInspector 2", "kubacho lab/Editor ExtensionsUtilities");
            Assets.ImportAsset("vHierarchy 2", "NoLicense");
        }
        
        if (GUILayout.Button(new GUIContent("Install Console Pro", "Click to install Console Pro"), buttonStyle))
        {
            Assets.ImportAsset("Editor Console Pro v3.975", "NoLicense");
        }

        GUILayout.EndVertical();
    }

    void OnGUI()
    {
        InstallOdinInspector();
    }
}

#endif
static class Assets
{
    public static void ImportAsset(string asset, string folder)
    {
        string basePath;
        if (OSVersion.Platform is PlatformID.MacOSX or PlatformID.Unix)
        {
            string homeDirectory = GetFolderPath(Environment.SpecialFolder.Personal);
            basePath = Combine(homeDirectory, "Library/Unity/Asset Store-5.x");
        }
        else
        {
            string defaultPath = Combine(GetFolderPath(Environment.SpecialFolder.ApplicationData), "Unity");
            basePath = Combine(EditorPrefs.GetString("AssetStoreCacheRootPath", defaultPath), "Asset Store-5.x");
        }

        asset = asset.EndsWith(".unitypackage") ? asset : asset + ".unitypackage";

        string fullPath = Combine(basePath, folder, asset);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"The asset package was not found at the path: {fullPath}");
        }

        ImportPackage(fullPath, false);
    }
}

static class Folders
{
    public static void Create(string root, params string[] folders)
    {
        var fullpath = Combine(Application.dataPath, root);
        if (!Directory.Exists(fullpath))
        {
            Directory.CreateDirectory(fullpath);
        }

        foreach (var folder in folders)
        {
            CreateSubFolders(fullpath, folder);
        }
    }

    static void CreateSubFolders(string rootPath, string folderHierarchy)
    {
        var folders = folderHierarchy.Split('/');
        var currentPath = rootPath;

        foreach (var folder in folders)
        {
            currentPath = Combine(currentPath, folder);
            if (!Directory.Exists(currentPath))
            {
                Directory.CreateDirectory(currentPath);
            }
        }
    }

    public static void Move(string folderName, string newParent)
    {
        var sourcePath = $"Assets/{folderName}";
        if (IsValidFolder(sourcePath))
        {
            var destinationPath = $"Assets/{newParent}/{folderName}";
            var error = MoveAsset(sourcePath, destinationPath);

            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError($"Failed to move {folderName}: {error}");
            }
        }
        else
        {
            Debug.LogError($"Folder {folderName} does not exist.");
        }
    }

    public static void Delete(string folderName)
    {
        var pathToDelete = $"Assets/{folderName}";

        if (IsValidFolder(pathToDelete))
        {
            DeleteAsset(pathToDelete);
        }
    }
}

static class Hierarchy
{
    public static void CreateCanvas(string canvasName = "Canvas")
    {
        GameObject existingCanvas = GameObject.Find(canvasName);
        if (existingCanvas != null)
        {
            Debug.LogWarning($"Canvas '{canvasName}' already exists in the Hierarchy.");
            return;
        }

        GameObject canvasGO = new GameObject(canvasName);

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();
        Undo.RegisterCreatedObjectUndo(canvas, $"Create Canvas '{canvasName}'");
        Debug.Log($"Canvas '{canvasName}' has been created in the Hierarchy.");

        if (Object.FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
            Undo.RegisterCreatedObjectUndo(eventSystem, "Create EventSystem");
            Debug.Log("EventSystem has been created in the Hierarchy.");
        }

        Hierarchy.Move("Canvas", "GUI");
        Hierarchy.Move("EventSystem", "GUI");
    }

    public static void Create(params string[] folders)
    {
        foreach (var folder in folders)
        {
            CreateFolder(folder);
        }
    }

    public static void CreateChild(string root, params string[] folders)
    {
        GameObject existingFolder = GameObject.Find(root);
        if (existingFolder != null)
        {
            Create(root);
        }

        foreach (var folder in folders)
        {
            var newFolder = CreateFolder(folder);
            newFolder.transform.SetParent(existingFolder.transform);
        }
    }

    static GameObject CreateFolder(string folderName)
    {
        GameObject existingFolder = GameObject.Find(folderName);
        if (existingFolder != null)
        {
            Debug.LogWarning($"Folder '{folderName}' already exists in the Hierarchy.");
            return null;
        }

        GameObject folder = new GameObject(folderName);
        Undo.RegisterCreatedObjectUndo(folder, $"Create Folder '{folderName}'");
        Debug.Log($"Folder '{folderName}' has been created in the Hierarchy.");
        return folder;
    }

    public static void Move(string folderName, string newParent)
    {
        GameObject folder = GameObject.Find(folderName);

        if (folder == null)
        {
            Debug.LogError($"Folder '{folderName}' does not exist.");
            return;
        }

        GameObject parent = GameObject.Find(newParent);

        if (parent == null)
        {
            Debug.LogError($"Parent '{newParent}' does not exist.");
            return;
        }

        folder.transform.SetParent(parent.transform);

        Debug.Log($"Folder '{folderName}' has been moved to '{newParent}'.");
    }
}

#endif