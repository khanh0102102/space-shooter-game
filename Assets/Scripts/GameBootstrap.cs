using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Starts the original Space Shooter prototype only when its legacy Main scene is opened.
/// The Space Explorer assignment uses separate MainMenu, Gameplay and EndGame scenes.
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    private void Awake()
    {
        StartLegacyGameIfNeeded();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void StartGameIfNeeded()
    {
        StartLegacyGameIfNeeded();
    }

    private static void StartLegacyGameIfNeeded()
    {
        if (SceneManager.GetActiveScene().name == "Main")
            SpaceShooterGame.CreateIfMissing();
    }
}
