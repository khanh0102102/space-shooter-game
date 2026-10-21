using UnityEngine;

/// <summary>Moves the player's laser upward and removes asteroids it hits.</summary>
public class SpaceExplorerLaser : MonoBehaviour
{
    private const float Speed = 9f;
    private float lifetime = 2.5f;

    private void Update()
    {
        if (SpaceExplorerGame.Instance == null || !SpaceExplorerGame.Instance.IsPlaying) return;
        transform.position += Vector3.up * Speed * Time.deltaTime;
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f || transform.position.y > SpaceExplorerGame.Instance.WorldTop + 1f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SpaceExplorerAsteroid asteroid = other.GetComponent<SpaceExplorerAsteroid>();
        if (asteroid == null) return;
        Destroy(asteroid.gameObject);
        Destroy(gameObject);
    }
}
