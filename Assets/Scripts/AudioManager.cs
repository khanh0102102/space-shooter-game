using UnityEngine;

/// <summary>
/// Optional sound effects from the Kenney pack. Missing files are ignored safely.
/// Keep the Audio built-in module enabled in Packages/manifest.json.
/// </summary>
public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    private AudioSource source;

    public static void Initialize(Camera targetCamera)
    {
        if (instance == null)
        {
            instance = FindObjectOfType<AudioManager>();
            if (instance == null)
            {
                GameObject audioObject = new GameObject("AudioManager");
                instance = audioObject.AddComponent<AudioManager>();
            }
        }

        if (targetCamera != null && FindObjectOfType<AudioListener>() == null)
            targetCamera.gameObject.AddComponent<AudioListener>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        source = GetComponent<AudioSource>();
        if (source == null)
            source = gameObject.AddComponent<AudioSource>();

        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f;
        source.volume = 1f;
    }

    public static void Play(string clipName, float volume = 1f)
    {
        if (instance == null)
        {
            Camera camera = Camera.main;
            Initialize(camera);
        }

        if (instance == null || instance.source == null)
            return;

        AudioClip clip = Resources.Load<AudioClip>("Kenney/Bonus/" + clipName);
        if (clip != null)
            instance.source.PlayOneShot(clip, Mathf.Clamp01(volume));
    }
}
