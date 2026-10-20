using UnityEngine;

public class BulletController : MonoBehaviour
{
    public enum BulletOwner { Player, Enemy }

    public BulletOwner Owner { get; private set; }
    public float Damage { get; private set; }

    private Vector2 direction;
    private float speed;
    private float lifetime = 5f;
    private bool consumed;

    public static BulletController Create(Vector2 position, Vector2 shotDirection, BulletOwner owner, float damage, float bulletSpeed)
    {
        GameObject bulletObject = new GameObject(owner == BulletOwner.Player ? "Player Bullet" : "Enemy Bullet");
        bulletObject.transform.position = position;
        bool playerOwned = owner == BulletOwner.Player;
        string laserPath = playerOwned ? "Kenney/PNG/Lasers/laserBlue01" : "Kenney/PNG/Lasers/laserRed01";
        bool hasLaserAsset = Resources.Load<Sprite>(laserPath) != null;
        bulletObject.transform.localScale = hasLaserAsset
            ? Vector3.one
            : (playerOwned ? Vector3.one * 0.20f : Vector3.one * 0.24f);

        SpriteRenderer renderer = bulletObject.AddComponent<SpriteRenderer>();
        renderer.sprite = SpriteFactory.Projectile(playerOwned);
        renderer.sortingOrder = 10;

        Rigidbody2D body = bulletObject.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.useFullKinematicContacts = true;
        body.gravityScale = 0f;
        body.simulated = true;

        if (hasLaserAsset)
        {
            BoxCollider2D collider = bulletObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.22f, 0.60f);
        }
        else
        {
            CircleCollider2D collider = bulletObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.45f;
        }

        BulletController controller = bulletObject.AddComponent<BulletController>();
        controller.Initialize(shotDirection, owner, damage, bulletSpeed);
        return controller;
    }

    public void Initialize(Vector2 shotDirection, BulletOwner owner, float damage, float bulletSpeed)
    {
        direction = shotDirection.sqrMagnitude > 0.001f ? shotDirection.normalized : Vector2.up;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
        Owner = owner;
        Damage = damage;
        speed = bulletSpeed;
    }

    private void Update()
    {
        if (GameManagerAccess.IsPausedOrFinished())
            return;

        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        lifetime -= Time.deltaTime;

        if (lifetime <= 0f || GameManagerAccess.IsOutsideWorld(transform.position, 1f))
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (consumed || GameManagerAccess.IsPausedOrFinished())
            return;

        if (Owner == BulletOwner.Player)
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                consumed = true;
                enemy.TakeDamage(Damage);
                Destroy(gameObject);
                return;
            }

            BossController boss = other.GetComponent<BossController>();
            if (boss != null)
            {
                consumed = true;
                boss.TakeDamage(Damage);
                Destroy(gameObject);
            }
        }
        else
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                consumed = true;
                player.TakeDamage(Mathf.RoundToInt(Damage));
                Destroy(gameObject);
            }
        }
    }
}

/// <summary>
/// Small null-safe access layer prevents projectiles from throwing errors during startup/scene teardown.
/// </summary>
internal static class GameManagerAccess
{
    public static bool IsPausedOrFinished()
    {
        return SpaceShooterGame.Instance == null || !SpaceShooterGame.Instance.IsGameplayActive;
    }

    public static bool IsOutsideWorld(Vector3 position, float margin)
    {
        if (SpaceShooterGame.Instance == null)
            return false;

        SpaceShooterGame game = SpaceShooterGame.Instance;
        return position.y < game.WorldBottom - margin ||
               position.y > game.WorldTop + margin ||
               position.x < -game.WorldHalfWidth - margin ||
               position.x > game.WorldHalfWidth + margin;
    }
}
