using UnityEngine;

/// <summary>
/// A simple player laser that destroys an asteroid on contact.
/// </summary>
public class SpaceExplorerLaserController : MonoBehaviour
{
    private const float Speed = 10f;
    private float lifeTime = 2.5f;

    public static void Create(Vector2 position)
    {
        GameObject laser = new GameObject("Player Laser");
        laser.transform.position = position;
        laser.transform.localScale = SpriteFactory.LoadKenneySprite("PNG/Lasers/laserBlue01") != null
            ? Vector3.one * 0.52f
            : Vector3.one * 0.18f;

        SpriteRenderer renderer = laser.AddComponent<SpriteRenderer>();
        renderer.sprite = SpriteFactory.Projectile(true);
        renderer.sortingOrder = 12;

        Rigidbody2D body = laser.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.useFullKinematicContacts = true;

        BoxCollider2D collider = laser.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(0.48f, 0.75f);

        laser.AddComponent<SpaceExplorerLaserController>();
    }

    private void Update()
    {
        SpaceExplorerSceneController game = SpaceExplorerSceneController.Instance;
        if (game == null || !game.CanRunGameplay)
            return;

        transform.position += Vector3.up * (Speed * Time.deltaTime);
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f || transform.position.y > game.WorldTop + 0.8f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SpaceExplorerAsteroidController asteroid = other.GetComponent<SpaceExplorerAsteroidController>();
        if (asteroid != null)
        {
            asteroid.Consume();
            Destroy(gameObject);
        }
    }
}
