using UnityEngine;

/// <summary>Collectible star worth ten points.</summary>
public class SpaceExplorerStar : MonoBehaviour
{
    private void Update()
    {
        if (SpaceExplorerGame.Instance == null || !SpaceExplorerGame.Instance.IsPlaying) return;
        transform.Rotate(0f, 0f, 50f * Time.deltaTime);
    }
}

/// <summary>Subtle downward drift for decorative background stars.</summary>
public class SpaceExplorerBackgroundStar : MonoBehaviour
{
    private float speed;
    public void Initialize(float value) { speed = value; }

    private void Update()
    {
        if (SpaceExplorerGame.Instance == null) return;
        transform.position += Vector3.down * speed * Time.deltaTime;
        if (transform.position.y < SpaceExplorerGame.Instance.WorldBottom)
        {
            transform.position = new Vector3(Random.Range(-SpaceExplorerGame.Instance.WorldHalfWidth,
                SpaceExplorerGame.Instance.WorldHalfWidth), SpaceExplorerGame.Instance.WorldTop, 4f);
        }
    }
}
