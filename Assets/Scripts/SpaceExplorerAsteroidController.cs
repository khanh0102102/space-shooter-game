using UnityEngine;

/// <summary>
/// A collectible-game hazard that drifts and randomly changes direction inside the camera bounds.
/// </summary>
public class SpaceExplorerAsteroidController : MonoBehaviour
{
    private Vector2 velocity;
    private float directionChangeTimer;
    private bool consumed;

    public void Initialize(Vector2 position, Vector2 initialVelocity)
    {
        transform.position = position;
        velocity = initialVelocity;
        directionChangeTimer = Random.Range(1.2f, 2.8f);
    }

    private void Update()
    {
        SpaceExplorerSceneController game = SpaceExplorerSceneController.Instance;
        if (game == null || !game.CanRunGameplay || consumed)
            return;

        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0f)
        {
            float angle = Random.Range(-75f, 75f);
            velocity = (Quaternion.Euler(0f, 0f, angle) * velocity.normalized) * Random.Range(0.45f, 1.25f);
            directionChangeTimer = Random.Range(1.2f, 2.8f);
        }

        Vector3 position = transform.position + (Vector3)(velocity * Time.deltaTime);
        float xLimit = game.WorldHalfWidth - 0.35f;
        float yLimit = game.WorldTop - 0.35f;

        if (position.x < -xLimit || position.x > xLimit)
        {
            velocity.x = -velocity.x;
            position.x = Mathf.Clamp(position.x, -xLimit, xLimit);
        }

        if (position.y < -yLimit || position.y > yLimit)
        {
            velocity.y = -velocity.y;
            position.y = Mathf.Clamp(position.y, -yLimit, yLimit);
        }

        transform.position = position;
        transform.Rotate(0f, 0f, 28f * Time.deltaTime);
    }

    public bool Consume()
    {
        if (consumed)
            return false;

        consumed = true;
        Destroy(gameObject);
        return true;
    }
}
