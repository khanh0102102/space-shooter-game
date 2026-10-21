using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Boots Space Explorer in its three assignment scenes and keeps the legacy prototype isolated.
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    private void Awake()
    {
        StartCorrectGame();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void StartGameIfNeeded()
    {
        StartCorrectGame();
    }

    private static void StartCorrectGame()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Main")
            SpaceShooterGame.CreateIfMissing();
        else
            SpaceExplorerGame.CreateIfMissing();
    }
}
