using UnityEngine;

/// <summary>Moves an asteroid with a randomized horizontal and downward velocity.</summary>
public class SpaceExplorerAsteroid : MonoBehaviour
{
    private Vector2 velocity;

    public void Initialize(Vector2 initialVelocity) { velocity = initialVelocity; }

    private void Update()
    {
        if (SpaceExplorerGame.Instance == null || !SpaceExplorerGame.Instance.IsPlaying) return;
        transform.position += (Vector3)(velocity * Time.deltaTime);
        transform.Rotate(0f, 0f, Random.Range(18f, 42f) * Time.deltaTime);

        if (transform.position.y < SpaceExplorerGame.Instance.WorldBottom - 1.5f ||
            Mathf.Abs(transform.position.x) > SpaceExplorerGame.Instance.WorldHalfWidth + 1.5f)
            Destroy(gameObject);
    }
}
