using UnityEngine;

public class StarfieldController : MonoBehaviour
{
    private float speed;
    private float halfHeight;

    public void Initialize(float starSpeed, float verticalLimit)
    {
        speed = starSpeed;
        halfHeight = verticalLimit;
    }

    private void Update()
    {
        if (Time.timeScale <= 0f)
            return;

        transform.position += Vector3.down * speed * Time.deltaTime;
        if (transform.position.y < -halfHeight - 0.1f)
        {
            Vector3 position = transform.position;
            position.y = halfHeight + Random.Range(0.0f, 1.5f);
            position.x = Random.Range(-SpaceShooterGame.Instance.WorldHalfWidth, SpaceShooterGame.Instance.WorldHalfWidth);
            transform.position = position;
        }
    }
}
