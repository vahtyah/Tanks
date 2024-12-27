using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

public static class SceneFunctionGenerator
{
    private const string DIRECTORY_SCENES = "Assets/_Project/Scenes";
    private const string SCRIPT_PATH = "Assets/_Project/Scripts/Editor/HelpfulButtons.cs";
    private const string GENERATED_START_MARKER = "// GENERATED METHODS START";
    private const string GENERATED_END_MARKER = "// GENERATED METHODS END";

    public static void UpdateSceneMethods()
    {
        var currentScenePathsInAssets = GetCurrentScenes();
        
        if(currentScenePathsInAssets == null) return;
        
        var scenes = currentScenePathsInAssets
            .Select(currentScene =>
            {
                string directory = System.IO.Path.GetDirectoryName(currentScene);
                string folderName = System.IO.Path.GetFileName(directory);
                return (Path: currentScene, Tab: folderName);
            });
        var methods = GenerateMethodsFromScenes(scenes);

        if (UpdateScriptFile(methods)) return;
        AssetDatabase.Refresh();
    }

    private static bool UpdateScriptFile(IEnumerable<string> methods)
    {
        var updatedMethods = string.Join("\n", methods);
        var fileContent = File.ReadAllText(SCRIPT_PATH);
        var pattern = new Regex($@"{Regex.Escape(GENERATED_START_MARKER)}(.*?){Regex.Escape(GENERATED_END_MARKER)}",
            RegexOptions.Singleline);
        if (!pattern.IsMatch(fileContent))
        {
            EditorUtility.DisplayDialog("Error",
                "Marker comments not found in the target script. Please add the markers.", "OK");
            return true;
        }

        fileContent = pattern.Replace(fileContent,
            $"{GENERATED_START_MARKER}\n{updatedMethods}\n{GENERATED_END_MARKER}");

        File.WriteAllText(SCRIPT_PATH, fileContent);
        return false;
    }

    private static IEnumerable<string> GenerateMethodsFromScenes(IEnumerable<(string Path, string Tab)> scenes)
    {
        return scenes
            .Select(sceneFile =>
            {
                var sceneName = System.IO.Path.GetFileNameWithoutExtension(sceneFile.Path);
                var sceneNameWithoutSpaces = char.ToUpper(sceneName[0]) + sceneName.Substring(1).Replace(" ", "");
                return $@"
        /// <summary>
        /// Loads the {sceneName} scene.
        /// Path: {sceneFile.Path}
        /// </summary>
        [BoxGroup(BOX_GROUP_SCENES)]
        [TabGroup(TAB_GROUP_SCENES, ""{sceneFile.Tab}"")]
        [ResponsiveButtonGroup(TAB_GROUP_SCENES + ""/{sceneFile.Tab}"" + ""/{sceneName}"")]
        public void Load{sceneNameWithoutSpaces}()
        {{
            LoadScene(DIRECTORY_SCENES + ""/{sceneFile.Tab}"" + ""/{sceneName}.unity"");
        }}";
            });
    }

    private static List<string> GetCurrentScenes()
    {
        if (!Directory.Exists(DIRECTORY_SCENES))
        {
            EditorUtility.DisplayDialog("Error",
                $"Scene directory not found: {DIRECTORY_SCENES}", "OK");
            return null;
        }
        
        if(!File.Exists(SCRIPT_PATH))
        {
            EditorUtility.DisplayDialog("Error",
                $"Script file not found: {SCRIPT_PATH}", "OK");
            return null;
        }

        var currentScenes = Directory.GetFiles(DIRECTORY_SCENES, "*.unity", SearchOption.AllDirectories)
            .Select(System.IO.Path.GetFullPath)
            .ToList();

        return currentScenes;
    }
}