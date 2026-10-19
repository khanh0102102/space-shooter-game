using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private int level;
    private float health;
    private float moveSpeed;
    private float fireTimer;
    private float waveOffset;
    private bool dead;
    private int scoreValue;

    public void Initialize(int currentLevel)
    {
        level = currentLevel;
        health = 20f + currentLevel * 5f;
        moveSpeed = 1.35f + currentLevel * 0.12f;
        fireTimer = Random.Range(1.2f, 2.4f);
        waveOffset = Random.Range(0f, 6.28f);
        scoreValue = 100 + currentLevel * 20;
    }

    private void Update()
    {
        if (dead || GameManagerAccess.IsPausedOrFinished())
            return;

        waveOffset += Time.deltaTime * (1.2f + level * 0.05f);
        Vector3 position = transform.position;
        position.y -= moveSpeed * Time.deltaTime;
        position.x += Mathf.Sin(waveOffset) * 0.45f * Time.deltaTime;
        transform.position = position;

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            FireAtPlayer();
            fireTimer = Random.Range(1.7f, 2.6f) - Mathf.Min(0.6f, level * 0.06f);
        }

        if (transform.position.y < SpaceShooterGame.Instance.WorldBottom - 1f)
            Destroy(gameObject);
    }

    private void FireAtPlayer()
    {
        PlayerController player = SpaceShooterGame.Instance.Player;
        if (player == null || player.Health <= 0)
            return;

        Vector2 direction = (Vector2)(player.transform.position - transform.position);
        if (direction.sqrMagnitude < 0.01f)
            direction = Vector2.down;

        BulletController.Create((Vector2)transform.position + direction.normalized * 0.34f,
            direction, BulletController.BulletOwner.Enemy, 20f, 3.2f + level * 0.12f);
    }

    public void TakeDamage(float damage)
    {
        if (dead || GameManagerAccess.IsPausedOrFinished())
            return;

        health -= damage;
        if (health <= 0f)
        {
            dead = true;
            SpaceShooterGame.Instance.EnemyDefeated(this, scoreValue);
            Destroy(gameObject);
        }
    }

    public void ConsumeOnPlayerImpact()
    {
        dead = true;
        Destroy(gameObject);
    }
}
