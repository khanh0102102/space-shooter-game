using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public const int MaxHealth = 100;
    public int Health { get; private set; } = MaxHealth;
    public int WeaponLevel { get; private set; } = 1;

    private const float BaseMoveSpeed = 5f;
    // EASY MODE: lower interval = faster automatic firing.
    private const float BaseFireInterval = 0.30f;
    // EASY MODE: higher base damage helps clear normal enemies and Bosses faster.
    private const float BaseBulletDamage = 15f;

    private SpriteRenderer spriteRenderer;
    private SpriteRenderer shieldRenderer;
    private float fireTimer;
    private float invulnerableUntil;
    private float rapidFireRemaining;
    private float damageBoostRemaining;
    private float speedBoostRemaining;
    private float slowRemaining;
    private float slowFireRemaining;
    private float weakShotRemaining;

    public float FireInterval
    {
        get
        {
            float interval = BaseFireInterval;
            if (rapidFireRemaining > 0f) interval *= 0.62f;
            if (slowFireRemaining > 0f) interval *= 1.5f;
            return Mathf.Max(0.08f, interval);
        }
    }

    public float BulletDamage
    {
        get
        {
            float damage = BaseBulletDamage;
            if (damageBoostRemaining > 0f) damage *= 1.5f;
            if (weakShotRemaining > 0f) damage *= 0.7f;
            return Mathf.Max(1f, damage);
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Health = MaxHealth;
        WeaponLevel = 1;

        GameObject shieldObject = new GameObject("Shield Effect");
        shieldObject.transform.SetParent(transform, false);
        shieldObject.transform.localPosition = Vector3.zero;
        shieldObject.transform.localScale = Vector3.one * 1.35f;
        shieldRenderer = shieldObject.AddComponent<SpriteRenderer>();
        shieldRenderer.sprite = SpriteFactory.Shield(1);
        shieldRenderer.sortingOrder = 8;
        shieldRenderer.enabled = false;

        UpdateDamageSprite();
    }

    private void Update()
    {
        if (GameManagerAccess.IsPausedOrFinished())
            return;

        UpdateEffects(Time.deltaTime);

        float horizontal = 0f;
        float vertical = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horizontal += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) vertical -= 1f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) vertical += 1f;

        Vector2 movement = new Vector2(horizontal, vertical);
        if (movement.sqrMagnitude > 1f)
            movement.Normalize();

        float moveSpeed = BaseMoveSpeed;
        if (speedBoostRemaining > 0f) moveSpeed *= 1.30f;
        if (slowRemaining > 0f) moveSpeed *= 0.70f;

        Vector3 position = transform.position + (Vector3)(movement * moveSpeed * Time.deltaTime);
        transform.position = SpaceShooterGame.Instance.ClampPlayerPosition(position);

        AimAtMouse();
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            FireVolley();
            fireTimer = FireInterval;
        }

        if (spriteRenderer != null)
        {
            if (Time.time < invulnerableUntil)
                spriteRenderer.color = Mathf.FloorToInt(Time.time * 18f) % 2 == 0 ? Color.white : new Color(0.22f, 0.82f, 1f, 0.35f);
            else
                spriteRenderer.color = Color.white;
        }

        if (shieldRenderer != null)
        {
            bool shieldAvailable = SpriteFactory.Shield(1) != null;
            shieldRenderer.enabled = shieldAvailable && Time.time < invulnerableUntil;
            if (shieldRenderer.enabled)
                shieldRenderer.sprite = SpriteFactory.Shield(1 + Mathf.FloorToInt(Time.unscaledTime * 8f) % 3);
        }
    }

    private void AimAtMouse()
    {
        Camera camera = Camera.main;
        if (camera == null)
            return;

        Vector3 mouseWorld = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -camera.transform.position.z));
        Vector2 direction = (Vector2)(mouseWorld - transform.position);
        if (direction.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);
    }

    private void FireVolley()
    {
        int bulletCount = WeaponLevel == 1 ? 1 : WeaponLevel == 2 ? 3 : 5;
        float spread = WeaponLevel == 2 ? 16f : WeaponLevel >= 3 ? 24f : 0f;
        Vector2 forward = transform.up;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = bulletCount == 1 ? 0f : Mathf.Lerp(-spread, spread, i / (float)(bulletCount - 1));
            Vector2 direction = Quaternion.Euler(0f, 0f, angle) * forward;
            Vector2 spawnPosition = (Vector2)transform.position + direction * 0.48f;
            BulletController.Create(spawnPosition, direction, BulletController.BulletOwner.Player, BulletDamage, 9f);
        }

        AudioManager.Play("sfx_laser1", 0.14f);
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || GameManagerAccess.IsPausedOrFinished() || Time.time < invulnerableUntil || Health <= 0)
            return;

        Health = Mathf.Max(0, Health - damage);
        invulnerableUntil = Time.time + 1.0f;
        UpdateDamageSprite();
        AudioManager.Play("sfx_shieldDown", 0.45f);

        if (Health <= 0)
            SpaceShooterGame.Instance.GameOver();
        else
            SpaceShooterGame.Instance.Notify("HIT! -" + damage + " HP");
    }

    public void Heal(int amount)
    {
        Health = Mathf.Clamp(Health + Mathf.Max(0, amount), 0, MaxHealth);
        UpdateDamageSprite();
    }

    private void UpdateDamageSprite()
    {
        if (spriteRenderer == null)
            return;

        if (Health > 75)
            spriteRenderer.sprite = SpriteFactory.PlayerShip();
        else if (Health > 50)
            spriteRenderer.sprite = SpriteFactory.PlayerDamage(1);
        else if (Health > 25)
            spriteRenderer.sprite = SpriteFactory.PlayerDamage(2);
        else
            spriteRenderer.sprite = SpriteFactory.PlayerDamage(3);
    }

    public void ApplyPickup(PickupType type)
    {
        switch (type)
        {
            case PickupType.Health:
                int oldHealth = Health;
                Heal(10);
                SpaceShooterGame.Instance.Notify("HP +" + (Health - oldHealth));
                break;

            case PickupType.TripleShot:
                WeaponLevel = Mathf.Min(3, WeaponLevel + 1);
                SpaceShooterGame.Instance.Notify(WeaponLevel == 3 ? "WEAPON MAX: 5 SHOTS" : "WEAPON UPGRADE: 3 SHOTS");
                break;

            case PickupType.RapidFire:
                rapidFireRemaining = 8f;
                slowFireRemaining = 0f;
                SpaceShooterGame.Instance.Notify("RAPID FIRE: 8s");
                break;

            case PickupType.DamageBoost:
                damageBoostRemaining = 8f;
                weakShotRemaining = 0f;
                SpaceShooterGame.Instance.Notify("DAMAGE BOOST: 8s");
                break;

            case PickupType.SpeedBoost:
                speedBoostRemaining = 8f;
                slowRemaining = 0f;
                SpaceShooterGame.Instance.Notify("SPEED BOOST: 8s");
                break;

            case PickupType.Slow:
                slowRemaining = 5f;
                speedBoostRemaining = 0f;
                SpaceShooterGame.Instance.Notify("SLOW: 5s");
                break;

            case PickupType.SlowFire:
                slowFireRemaining = 5f;
                rapidFireRemaining = 0f;
                SpaceShooterGame.Instance.Notify("SLOW FIRE: 5s");
                break;

            case PickupType.WeakShot:
                weakShotRemaining = 5f;
                damageBoostRemaining = 0f;
                SpaceShooterGame.Instance.Notify("WEAK SHOT: 5s");
                break;
        }

        AudioManager.Play(type == PickupType.Health ? "sfx_shieldUp" : "sfx_zap", 0.42f);
    }

    public void ResetForLevel(int health, int weaponLevel)
    {
        Health = Mathf.Clamp(health, 1, MaxHealth);
        WeaponLevel = Mathf.Clamp(weaponLevel, 1, 3);
        rapidFireRemaining = 0f;
        damageBoostRemaining = 0f;
        speedBoostRemaining = 0f;
        slowRemaining = 0f;
        slowFireRemaining = 0f;
        weakShotRemaining = 0f;
        fireTimer = 0.1f;
        invulnerableUntil = Time.time + 1.0f;
        UpdateDamageSprite();
    }

    private void UpdateEffects(float deltaTime)
    {
        rapidFireRemaining = Mathf.Max(0f, rapidFireRemaining - deltaTime);
        damageBoostRemaining = Mathf.Max(0f, damageBoostRemaining - deltaTime);
        speedBoostRemaining = Mathf.Max(0f, speedBoostRemaining - deltaTime);
        slowRemaining = Mathf.Max(0f, slowRemaining - deltaTime);
        slowFireRemaining = Mathf.Max(0f, slowFireRemaining - deltaTime);
        weakShotRemaining = Mathf.Max(0f, weakShotRemaining - deltaTime);
    }

    public string ActiveEffectsSummary()
    {
        List<string> effects = new List<string>();
        if (rapidFireRemaining > 0f) effects.Add("Rapid " + Mathf.CeilToInt(rapidFireRemaining) + "s");
        if (damageBoostRemaining > 0f) effects.Add("Damage " + Mathf.CeilToInt(damageBoostRemaining) + "s");
        if (speedBoostRemaining > 0f) effects.Add("Speed " + Mathf.CeilToInt(speedBoostRemaining) + "s");
        if (slowRemaining > 0f) effects.Add("Slow " + Mathf.CeilToInt(slowRemaining) + "s");
        if (slowFireRemaining > 0f) effects.Add("Slow Fire " + Mathf.CeilToInt(slowFireRemaining) + "s");
        if (weakShotRemaining > 0f) effects.Add("Weak " + Mathf.CeilToInt(weakShotRemaining) + "s");
        return effects.Count == 0 ? "None" : string.Join(" | ", effects.ToArray());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManagerAccess.IsPausedOrFinished())
            return;

        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            TakeDamage(20);
            enemy.ConsumeOnPlayerImpact();
            return;
        }

        AsteroidController asteroid = other.GetComponent<AsteroidController>();
        if (asteroid != null)
        {
            TakeDamage(40);
            asteroid.ConsumeOnImpact();
            return;
        }

        PickupController pickup = other.GetComponent<PickupController>();
        if (pickup != null)
        {
            ApplyPickup(pickup.Type);
            pickup.Consume();
            return;
        }

        BossController boss = other.GetComponent<BossController>();
        if (boss != null)
            TakeDamage(40);
    }
}
