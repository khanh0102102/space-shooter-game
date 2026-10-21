using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Keeps the original Space Shooter prototype isolated to its legacy Main scene.
/// Space Explorer scenes own their SpaceExplorerSceneController directly.
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
