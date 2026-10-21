using UnityEngine;

/// <summary>
/// Collectible star; the player receives 10 points when touching it.
/// </summary>
public class SpaceExplorerStarController : MonoBehaviour
{
    private Vector3 startPosition;
    private float phase;
    private bool collected;

    public void Initialize(Vector2 position)
    {
        transform.position = position;
        startPosition = transform.position;
        phase = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        SpaceExplorerSceneController game = SpaceExplorerSceneController.Instance;
        if (game == null || !game.CanRunGameplay || collected)
            return;

        phase += Time.deltaTime * 1.6f;
        transform.position = startPosition + new Vector3(0f, Mathf.Sin(phase) * 0.08f, 0f);
        transform.Rotate(0f, 0f, 45f * Time.deltaTime);
    }

    public void Collect()
    {
        if (collected)
            return;

        SpaceExplorerSceneController game = SpaceExplorerSceneController.Instance;
        if (game == null || !game.CanRunGameplay)
            return;

        collected = true;
        game.CollectStar();
        Destroy(gameObject);
    }
}
