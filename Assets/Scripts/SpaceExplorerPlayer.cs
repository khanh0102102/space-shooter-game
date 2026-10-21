using UnityEngine;

/// <summary>Arrow-key movement and player-triggered laser shooting for Space Explorer.</summary>
public class SpaceExplorerPlayer : MonoBehaviour
{
    private const float MoveSpeed = 5f;
    private float fireCooldown;

    private void Update()
    {
        if (SpaceExplorerGame.Instance == null || !SpaceExplorerGame.Instance.IsPlaying) return;

        float x = 0f;
        float y = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) x -= 1f;
        if (Input.GetKey(KeyCode.RightArrow)) x += 1f;
        if (Input.GetKey(KeyCode.DownArrow)) y -= 1f;
        if (Input.GetKey(KeyCode.UpArrow)) y += 1f;

        Vector2 movement = new Vector2(x, y);
        if (movement.sqrMagnitude > 1f) movement.Normalize();
        Vector3 next = transform.position + (Vector3)(movement * MoveSpeed * Time.deltaTime);
        transform.position = SpaceExplorerGame.Instance.ClampPlayerPosition(next);

        fireCooldown -= Time.deltaTime;
        if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && fireCooldown <= 0f)
        {
            SpaceExplorerGame.Instance.SpawnLaser(transform.position + Vector3.up * 0.42f);
            fireCooldown = 0.24f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SpaceExplorerAsteroid asteroid = other.GetComponent<SpaceExplorerAsteroid>();
        if (asteroid != null) SpaceExplorerGame.Instance.HitAsteroid(asteroid);

        SpaceExplorerStar star = other.GetComponent<SpaceExplorerStar>();
        if (star != null) SpaceExplorerGame.Instance.CollectStar(star);
    }
}
