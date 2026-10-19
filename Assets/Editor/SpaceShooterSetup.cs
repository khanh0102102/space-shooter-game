using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class SpaceShooterSetup
{
    private const string SceneFolder = "Assets/Scenes";
    private const string ScenePath = "Assets/Scenes/Main.unity";

    static SpaceShooterSetup()
    {
        EditorApplication.delayCall += EnsureMainScene;
    }

    [MenuItem("Space Shooter/Create or Repair Main Scene")]
    public static void CreateOrRepairMainScene()
    {
        if (!File.Exists(ScenePath))
        {
            Directory.CreateDirectory(SceneFolder);
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            GameObject bootstrap = new GameObject("GameBootstrap");
            bootstrap.AddComponent<GameBootstrap>();
            EditorSceneManager.SaveScene(scene, ScenePath);
        }

        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        bool sceneRegistered = false;
        for (int i = 0; i < scenes.Count; i++)
        {
            if (scenes[i].path == ScenePath)
            {
                scenes[i] = new EditorBuildSettingsScene(ScenePath, true);
                sceneRegistered = true;
                break;
            }
        }

        if (!sceneRegistered)
            scenes.Add(new EditorBuildSettingsScene(ScenePath, true));

        EditorBuildSettings.scenes = scenes.ToArray();

        AssetDatabase.Refresh();
        Debug.Log("Space Shooter scene is ready: " + ScenePath);
    }

    private static void EnsureMainScene()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        CreateOrRepairMainScene();
    }
}
