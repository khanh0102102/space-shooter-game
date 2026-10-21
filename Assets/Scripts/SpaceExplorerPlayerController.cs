using UnityEngine;

/// <summary>
/// Arrow-key movement and Space-key laser firing for the Space Explorer lab.
/// </summary>
public class SpaceExplorerPlayerController : MonoBehaviour
{
    private const float MoveSpeed = 5f;
    private const float FireInterval = 0.26f;
    private float fireTimer;

    private void Update()
    {
        SpaceExplorerSceneController game = SpaceExplorerSceneController.Instance;
        if (game == null || !game.CanRunGameplay)
            return;

        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.LeftArrow)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.RightArrow)) horizontal += 1f;
        if (Input.GetKey(KeyCode.DownArrow)) vertical -= 1f;
        if (Input.GetKey(KeyCode.UpArrow)) vertical += 1f;

        Vector2 movement = new Vector2(horizontal, vertical);
        if (movement.sqrMagnitude > 1f)
            movement.Normalize();

        transform.position = game.ClampPlayerPosition(
            transform.position + (Vector3)(movement * MoveSpeed * Time.deltaTime));

        fireTimer -= Time.deltaTime;
        if (Input.GetKey(KeyCode.Space) && fireTimer <= 0f)
        {
            SpaceExplorerLaserController.Create((Vector2)transform.position + Vector2.up * 0.43f);
            fireTimer = FireInterval;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SpaceExplorerSceneController game = SpaceExplorerSceneController.Instance;
        if (game == null || !game.CanRunGameplay)
            return;

        SpaceExplorerAsteroidController asteroid = other.GetComponent<SpaceExplorerAsteroidController>();
        if (asteroid != null && asteroid.Consume())
        {
            game.PlayerHitAsteroid();
            return;
        }

        SpaceExplorerStarController star = other.GetComponent<SpaceExplorerStarController>();
        if (star != null)
            star.Collect();
    }
}
