using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Creates and registers the three scenes required by the Space Explorer lab.
/// </summary>
[InitializeOnLoad]
public static class SpaceShooterSetup
{
    private const string SceneFolder = "Assets/Scenes";
    private static readonly string[] RequiredScenes =
    {
        "Assets/Scenes/MainMenu.unity",
        "Assets/Scenes/Gameplay.unity",
        "Assets/Scenes/EndGame.unity"
    };

    static SpaceShooterSetup()
    {
        EditorApplication.delayCall += EnsureScenes;
    }

    [MenuItem("Space Explorer/Create or Repair Scenes")]
    public static void CreateOrRepairScenes()
    {
        Directory.CreateDirectory(SceneFolder);

        foreach (string path in RequiredScenes)
        {
            if (!File.Exists(path))
            {
                Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                GameObject bootstrap = new GameObject("GameBootstrap");
                bootstrap.AddComponent<GameBootstrap>();
                EditorSceneManager.SaveScene(scene, path);
            }
            else
            {
                Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                if (Object.FindObjectOfType<GameBootstrap>() == null)
                {
                    GameObject bootstrap = new GameObject("GameBootstrap");
                    bootstrap.AddComponent<GameBootstrap>();
                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.SaveScene(scene, path);
                }
            }
        }

        var buildScenes = new List<EditorBuildSettingsScene>();
        foreach (string path in RequiredScenes)
            buildScenes.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = buildScenes.ToArray();

        AssetDatabase.Refresh();
        EditorSceneManager.OpenScene(RequiredScenes[0], OpenSceneMode.Single);
        Debug.Log("Space Explorer scenes are ready: MainMenu, Gameplay, EndGame.");
    }

    private static void EnsureScenes()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        CreateOrRepairScenes();
    }
}
