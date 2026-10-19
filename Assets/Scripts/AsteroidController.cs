using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    private Vector2 velocity;

    public void Initialize(Vector2 initialVelocity)
    {
        velocity = initialVelocity;
    }

    private void Update()
    {
        if (GameManagerAccess.IsPausedOrFinished())
            return;

        transform.position += (Vector3)(velocity * Time.deltaTime);
        transform.Rotate(0f, 0f, 22f * Time.deltaTime);

        if (GameManagerAccess.IsOutsideWorld(transform.position, 1.5f))
            Destroy(gameObject);
    }

    public void ConsumeOnImpact()
    {
        Destroy(gameObject);
    }
}
