using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Creates the three real Unity scenes required by the Space Explorer lab and puts MainMenu first in Build Settings.
/// The older Space Shooter prototype scene is kept as a separate legacy scene.
/// </summary>
[InitializeOnLoad]
public static class SpaceExplorerSceneSetup
{
    private const string SceneFolder = "Assets/Scenes";
    private static readonly string[] RequiredScenePaths =
    {
        "Assets/Scenes/MainMenu.unity",
        "Assets/Scenes/Gameplay.unity",
        "Assets/Scenes/EndGame.unity"
    };

    static SpaceExplorerSceneSetup()
    {
        EditorApplication.delayCall += CreateOrRepairScenes;
    }

    [MenuItem("Space Explorer/Create or Repair Assignment Scenes")]
    public static void CreateOrRepairScenes()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        Directory.CreateDirectory(SceneFolder);
        AssetDatabase.Refresh();

        foreach (string scenePath in RequiredScenePaths)
            EnsureScene(scenePath);

        List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>();
        HashSet<string> added = new HashSet<string>();

        foreach (string path in RequiredScenePaths)
        {
            buildScenes.Add(new EditorBuildSettingsScene(path, true));
            added.Add(path);
        }

        // Keep any existing scenes (including the original Space Shooter scene) after the assignment scenes.
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (added.Contains(scene.path) || !File.Exists(scene.path))
                continue;

            buildScenes.Add(new EditorBuildSettingsScene(scene.path, scene.enabled));
            added.Add(scene.path);
        }

        EditorBuildSettings.scenes = buildScenes.ToArray();
        AssetDatabase.Refresh();
        Debug.Log("Space Explorer assignment scenes are ready. MainMenu is the first Build Settings scene.");
    }

    private static void EnsureScene(string path)
    {
        Scene originalActiveScene = SceneManager.GetActiveScene();
        bool sceneAlreadyLoaded = false;
        Scene scene = SceneManager.GetSceneByPath(path);

        if (scene.IsValid() && scene.isLoaded)
        {
            sceneAlreadyLoaded = true;
        }
        else if (File.Exists(path))
        {
            scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
        }
        else
        {
            scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        }

        bool sceneWasCreated = !File.Exists(path);
        SceneManager.SetActiveScene(scene);

        bool hasController = false;
        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            if (rootObject.GetComponentInChildren<SpaceExplorerSceneController>(true) != null)
            {
                hasController = true;
                break;
            }
        }

        if (!hasController)
        {
            GameObject root = new GameObject("SpaceExplorerSceneRoot");
            SceneManager.MoveGameObjectToScene(root, scene);
            root.AddComponent<SpaceExplorerSceneController>();
        }

        if (sceneWasCreated || !hasController)
        {
            if (!EditorSceneManager.SaveScene(scene, path))
                Debug.LogError("Could not save Space Explorer scene: " + path);
        }

        // Only close scenes opened temporarily by this setup utility; don't close the user's active scene.
        if (!sceneAlreadyLoaded)
            EditorSceneManager.CloseScene(scene, true);

        if (originalActiveScene.IsValid() && originalActiveScene.isLoaded)
            SceneManager.SetActiveScene(originalActiveScene);
    }
}
