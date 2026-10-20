using UnityEngine;

/// <summary>
/// Simple frame-by-frame sprite animation for the Kenney fire00-fire19 explosion sequence.
/// Uses unscaled time so the Boss-death explosion can finish behind the level-clear panel.
/// </summary>
public class SpriteAnimationController : MonoBehaviour
{
    private int frameCount = 20;
    private int currentFrame;
    private float secondsPerFrame = 1f / 24f;
    private float elapsed;
    private SpriteRenderer spriteRenderer;

    public void Initialize(int totalFrames, float framesPerSecond)
    {
        frameCount = Mathf.Max(1, totalFrames);
        secondsPerFrame = 1f / Mathf.Max(1f, framesPerSecond);
        currentFrame = 0;
        elapsed = 0f;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.sprite = SpriteFactory.ExplosionFrame(0);
    }

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (SpaceShooterGame.Instance != null &&
            SpaceShooterGame.Instance.CurrentState == SpaceShooterGame.ScreenState.Paused)
            return;

        elapsed += Time.unscaledDeltaTime;
        while (elapsed >= secondsPerFrame)
        {
            elapsed -= secondsPerFrame;
            currentFrame++;

            if (currentFrame >= frameCount)
            {
                Destroy(gameObject);
                return;
            }

            if (spriteRenderer != null)
                spriteRenderer.sprite = SpriteFactory.ExplosionFrame(currentFrame);
        }
    }
}
