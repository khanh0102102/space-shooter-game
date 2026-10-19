using UnityEngine;

public enum PickupType
{
    Health,
    TripleShot,
    RapidFire,
    DamageBoost,
    SpeedBoost,
    Slow,
    SlowFire,
    WeakShot
}

public class PickupController : MonoBehaviour
{
    public PickupType Type { get; private set; }

    private float driftSpeed = 0.85f;
    private float lifeTime = 12f;
    private float phase;

    public void Initialize(PickupType type, Vector2 position)
    {
        Type = type;
        transform.position = position;
        phase = Random.Range(0f, 6.28f);

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
            renderer.sprite = SpriteFactory.Pickup(type);
    }

    private void Update()
    {
        if (GameManagerAccess.IsPausedOrFinished())
            return;

        phase += Time.deltaTime * 1.6f;
        transform.position += new Vector3(Mathf.Sin(phase) * 0.45f * Time.deltaTime, -driftSpeed * Time.deltaTime, 0f);
        transform.Rotate(0f, 0f, 35f * Time.deltaTime);
        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0f || transform.position.y < SpaceShooterGame.Instance.WorldBottom - 1f)
            Destroy(gameObject);
    }

    public void Consume()
    {
        Destroy(gameObject);
    }

    public string DisplayName
    {
        get
        {
            switch (Type)
            {
                case PickupType.Health: return "HEALTH +10";
                case PickupType.TripleShot: return "TRIPLE SHOT";
                case PickupType.RapidFire: return "RAPID FIRE";
                case PickupType.DamageBoost: return "DAMAGE BOOST";
                case PickupType.SpeedBoost: return "SPEED BOOST";
                case PickupType.Slow: return "SLOW";
                case PickupType.SlowFire: return "SLOW FIRE";
                default: return "WEAK SHOT";
            }
        }
    }
}
