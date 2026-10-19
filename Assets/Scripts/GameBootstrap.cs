using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    private void Awake()
    {
        SpaceShooterGame.CreateIfMissing();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void StartGameIfNeeded()
    {
        SpaceShooterGame.CreateIfMissing();
    }
}
