using UnityEngine;

public class BossController : MonoBehaviour
{
    public float CurrentHealth { get; private set; }
    public float MaxHealth { get; private set; }

    private int level;
    private float fireTimer = 1f;
    private float movePhase;
    private bool defeated;

    public void Initialize(int currentLevel)
    {
        level = currentLevel;
        // EASY MODE: Boss HP grows gently from level to level.
        MaxHealth = 300f + (currentLevel - 1) * 350f;
        CurrentHealth = MaxHealth;
        // Bosses wait longer before the first attack.
        fireTimer = 1.8f;
        movePhase = Random.Range(0f, 6.28f);
    }

    private void Update()
    {
        if (defeated || GameManagerAccess.IsPausedOrFinished())
            return;

        movePhase += Time.deltaTime * (0.55f + level * 0.06f);
        Vector3 position = transform.position;
        position.x = Mathf.Sin(movePhase) * Mathf.Max(1.4f, SpaceShooterGame.Instance.WorldHalfWidth - 1.4f);
        position.y = 3.15f + Mathf.Sin(movePhase * 1.8f) * 0.22f;
        transform.position = position;

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            FirePattern();
            fireTimer = Mathf.Max(1.15f, 1.80f - level * 0.08f);
        }
    }

    private void FirePattern()
    {
        PlayerController player = SpaceShooterGame.Instance.Player;
        if (player == null || player.Health <= 0)
            return;

        Vector2 aim = (Vector2)(player.transform.position - transform.position);
        if (aim.sqrMagnitude < 0.01f)
            aim = Vector2.down;
        aim.Normalize();

        int bulletCount = level >= 3 ? 5 : 3;
        float spread = level >= 4 ? 34f : 25f;
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = bulletCount == 1 ? 0f : Mathf.Lerp(-spread, spread, i / (float)(bulletCount - 1));
            Vector2 direction = Quaternion.Euler(0f, 0f, angle) * aim;
            BulletController.Create((Vector2)transform.position + direction * 0.72f,
                direction, BulletController.BulletOwner.Enemy, 20f, 2.8f + level * 0.12f);
        }
    }

    public void TakeDamage(float damage)
    {
        if (defeated || GameManagerAccess.IsPausedOrFinished())
            return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);
        if (CurrentHealth <= 0f)
        {
            defeated = true;
            SpaceShooterGame.Instance.BossDefeated(this);
            Destroy(gameObject);
        }
    }
}
