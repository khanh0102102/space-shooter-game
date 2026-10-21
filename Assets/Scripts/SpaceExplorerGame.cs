using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Coordinates the Space Explorer lab: separate scenes, collectible stars, asteroid hazards,
/// score tracking, instructions, and the end-game screen.
/// </summary>
public class SpaceExplorerGame : MonoBehaviour
{
    public static SpaceExplorerGame Instance { get; private set; }

    public int Score { get; private set; }
    public bool IsPlaying => SceneManager.GetActiveScene().name == "Gameplay" && !gameEnded;
    public float WorldHalfWidth => gameCamera != null ? gameCamera.orthographicSize * gameCamera.aspect : 8.8f;
    public float WorldTop => gameCamera != null ? gameCamera.orthographicSize : 5f;
    public float WorldBottom => -WorldTop;

    private Camera gameCamera;
    private Transform environmentRoot;
    private float asteroidTimer;
    private float starTimer;
    private bool gameEnded;
    private bool showInstructions;
    private GUIStyle titleStyle;
    private GUIStyle bodyStyle;
    private GUIStyle buttonStyle;

    public static void CreateIfMissing()
    {
        if (Instance != null) return;
        GameObject root = new GameObject("SpaceExplorerGame");
        root.AddComponent<SpaceExplorerGame>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SetupCamera();
        BuildEnvironment();
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void SetupCamera()
    {
        gameCamera = Camera.main;
        if (gameCamera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(transform, false);
            gameCamera = cameraObject.AddComponent<Camera>();
        }
        else if (gameCamera.transform != transform && !gameCamera.transform.IsChildOf(transform))
        {
            gameCamera.transform.SetParent(transform, true);
        }

        gameCamera.orthographic = true;
        gameCamera.orthographicSize = 5f;
        gameCamera.transform.position = new Vector3(0f, 0f, -10f);
        gameCamera.clearFlags = CameraClearFlags.SolidColor;
        gameCamera.backgroundColor = new Color(0.012f, 0.022f, 0.075f);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        gameEnded = scene.name == "EndGame";
        ClearGameplayObjects();
        BuildEnvironment();

        if (scene.name == "Gameplay")
        {
            gameEnded = false;
            CreatePlayer();
            for (int i = 0; i < 12; i++) SpawnStar(RandomWorldPosition());
            asteroidTimer = 1.2f;
            starTimer = 2f;
        }
    }

    private void BuildEnvironment()
    {
        if (environmentRoot != null) Destroy(environmentRoot.gameObject);
        GameObject root = new GameObject("Space Environment");
        root.transform.SetParent(transform, false);
        environmentRoot = root.transform;

        Sprite background = SpriteFactory.Background(1);
        if (background != null)
        {
            GameObject bg = new GameObject("Space Background");
            bg.transform.SetParent(environmentRoot, false);
            bg.transform.position = new Vector3(0f, 0f, 5f);
            SpriteRenderer renderer = bg.AddComponent<SpriteRenderer>();
            renderer.sprite = background;
            renderer.sortingOrder = -1000;
            Vector2 size = background.bounds.size;
            float scale = Mathf.Max((WorldHalfWidth * 2f) / Mathf.Max(0.01f, size.x),
                                    (WorldTop * 2f) / Mathf.Max(0.01f, size.y));
            bg.transform.localScale = Vector3.one * scale;
        }

        for (int i = 0; i < 95; i++)
        {
            GameObject star = new GameObject("Background Star");
            star.transform.SetParent(environmentRoot, false);
            star.transform.position = new Vector3(Random.Range(-WorldHalfWidth, WorldHalfWidth),
                Random.Range(WorldBottom, WorldTop), 4f);
            star.transform.localScale = Vector3.one * Random.Range(0.025f, 0.07f);
            SpriteRenderer renderer = star.AddComponent<SpriteRenderer>();
            renderer.sprite = SpriteFactory.Star();
            renderer.sortingOrder = -100;
            star.AddComponent<SpaceExplorerBackgroundStar>().Initialize(Random.Range(0.15f, 0.6f));
        }
    }

    private void Update()
    {
        if (!IsPlaying) return;

        asteroidTimer -= Time.deltaTime;
        if (asteroidTimer <= 0f)
        {
            SpawnAsteroid();
            asteroidTimer = Random.Range(1.4f, 2.5f);
        }

        starTimer -= Time.deltaTime;
        if (starTimer <= 0f)
        {
            if (FindObjectsOfType<SpaceExplorerStar>().Length < 12)
                SpawnStar(RandomWorldPosition());
            starTimer = 1.5f;
        }
    }

    private void CreatePlayer()
    {
        GameObject player = new GameObject("Player Spaceship");
        player.transform.SetParent(transform, true);
        player.transform.position = new Vector3(0f, WorldBottom + 1f, 0f);
        player.transform.localScale = Vector3.one * 0.8f;
        SpriteRenderer renderer = player.AddComponent<SpriteRenderer>();
        renderer.sprite = SpriteFactory.PlayerShip(1);
        renderer.sortingOrder = 5;

        Rigidbody2D body = player.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.useFullKinematicContacts = true;

        CircleCollider2D collider = player.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.38f;
        player.AddComponent<SpaceExplorerPlayer>();
    }

    public void SpawnLaser(Vector2 position)
    {
        if (!IsPlaying) return;
        GameObject laser = new GameObject("Player Laser");
        laser.transform.SetParent(transform, true);
        laser.transform.position = position;
        laser.transform.localScale = Vector3.one * 0.22f;
        SpriteRenderer renderer = laser.AddComponent<SpriteRenderer>();
        renderer.sprite = SpriteFactory.Projectile(true);
        renderer.sortingOrder = 10;
        Rigidbody2D body = laser.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.useFullKinematicContacts = true;
        BoxCollider2D collider = laser.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        laser.AddComponent<SpaceExplorerLaser>();
    }

    private void SpawnAsteroid()
    {
        GameObject asteroid = new GameObject("Asteroid");
        asteroid.transform.SetParent(transform, true);
        asteroid.transform.position = new Vector3(Random.Range(-WorldHalfWidth + 0.5f, WorldHalfWidth - 0.5f),
            WorldTop + 0.7f, 0f);
        float scale = Random.Range(0.45f, 0.85f);
        asteroid.transform.localScale = Vector3.one * scale;
        SpriteRenderer renderer = asteroid.AddComponent<SpriteRenderer>();
        renderer.sprite = SpriteFactory.Asteroid();
        renderer.sortingOrder = 3;
        Rigidbody2D body = asteroid.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.useFullKinematicContacts = true;
        CircleCollider2D collider = asteroid.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.45f;
        Vector2 velocity = new Vector2(Random.Range(-0.9f, 0.9f), -Random.Range(0.7f, 1.8f));
        asteroid.AddComponent<SpaceExplorerAsteroid>().Initialize(velocity);
    }

    private void SpawnStar(Vector2 position)
    {
        GameObject star = new GameObject("Collectible Star");
        star.transform.SetParent(transform, true);
        star.transform.position = position;
        star.transform.localScale = Vector3.one * 0.42f;
        SpriteRenderer renderer = star.AddComponent<SpriteRenderer>();
        renderer.sprite = SpriteFactory.Star();
        renderer.sortingOrder = 4;
        Rigidbody2D body = star.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.useFullKinematicContacts = true;
        CircleCollider2D collider = star.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.55f;
        star.AddComponent<SpaceExplorerStar>();
    }

    private Vector2 RandomWorldPosition()
    {
        return new Vector2(Random.Range(-WorldHalfWidth + 0.6f, WorldHalfWidth - 0.6f),
            Random.Range(WorldBottom + 0.8f, WorldTop - 0.8f));
    }

    public Vector3 ClampPlayerPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, -WorldHalfWidth + 0.45f, WorldHalfWidth - 0.45f);
        position.y = Mathf.Clamp(position.y, WorldBottom + 0.45f, WorldTop - 0.45f);
        position.z = 0f;
        return position;
    }

    public void CollectStar(SpaceExplorerStar star)
    {
        if (!IsPlaying || star == null) return;
        Score += 10;
        Destroy(star.gameObject);
    }

    public void HitAsteroid(SpaceExplorerAsteroid asteroid)
    {
        if (!IsPlaying) return;
        Score -= 10;
        EndGame();
    }

    public void EndGame()
    {
        if (gameEnded) return;
        gameEnded = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene("EndGame");
    }

    public void StartGame()
    {
        Score = 0;
        gameEnded = false;
        showInstructions = false;
        SceneManager.LoadScene("Gameplay");
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        showInstructions = false;
        SceneManager.LoadScene("MainMenu");
    }

    private void ClearGameplayObjects()
    {
        foreach (SpaceExplorerPlayer item in FindObjectsOfType<SpaceExplorerPlayer>()) Destroy(item.gameObject);
        foreach (SpaceExplorerAsteroid item in FindObjectsOfType<SpaceExplorerAsteroid>()) Destroy(item.gameObject);
        foreach (SpaceExplorerStar item in FindObjectsOfType<SpaceExplorerStar>()) Destroy(item.gameObject);
        foreach (SpaceExplorerLaser item in FindObjectsOfType<SpaceExplorerLaser>()) Destroy(item.gameObject);
    }

    private void OnGUI()
    {
        EnsureStyles();
        string scene = SceneManager.GetActiveScene().name;

        if (scene == "MainMenu")
        {
            DrawOverlay();
            Rect panel = Panel(520f, showInstructions ? 420f : 350f);
            GUI.Box(panel, GUIContent.none);
            GUI.Label(new Rect(panel.x + 20f, panel.y + 24f, panel.width - 40f, 55f), "SPACE EXPLORER", titleStyle);
            GUI.Label(new Rect(panel.x + 28f, panel.y + 84f, panel.width - 56f, 42f),
                "Navigate through space, collect stars, and avoid asteroids.", bodyStyle);
            if (GUI.Button(new Rect(panel.x + 100f, panel.y + 145f, panel.width - 200f, 48f), "PLAY", buttonStyle))
                StartGame();
            if (GUI.Button(new Rect(panel.x + 100f, panel.y + 205f, panel.width - 200f, 44f),
                showInstructions ? "HIDE INSTRUCTIONS" : "INSTRUCTIONS", buttonStyle))
                showInstructions = !showInstructions;
            if (showInstructions)
                GUI.Label(new Rect(panel.x + 30f, panel.y + 260f, panel.width - 60f, 120f),
                    "Arrow keys: move the spaceship\nSpace or left mouse button: shoot lasers\nCollect stars: +10 points\nTouch an asteroid: -10 points and end the game.", bodyStyle);
            return;
        }

        if (scene == "Gameplay")
        {
            GUI.Box(new Rect(12f, 12f, 250f, 48f), GUIContent.none);
            GUI.Label(new Rect(24f, 20f, 230f, 32f), "SCORE: " + Score, bodyStyle);
            GUI.Label(new Rect(Screen.width - 300f, 18f, 286f, 30f), "ARROWS: MOVE   SPACE: SHOOT", bodyStyle);
            return;
        }

        if (scene == "EndGame")
        {
            DrawOverlay();
            Rect panel = Panel(500f, 300f);
            GUI.Box(panel, GUIContent.none);
            GUI.Label(new Rect(panel.x + 20f, panel.y + 30f, panel.width - 40f, 58f), "MISSION ENDED", titleStyle);
            GUI.Label(new Rect(panel.x + 30f, panel.y + 100f, panel.width - 60f, 40f), "Your final score: " + Score, bodyStyle);
            if (GUI.Button(new Rect(panel.x + 90f, panel.y + 160f, panel.width - 180f, 44f), "MAIN MENU", buttonStyle))
                ReturnToMenu();
            if (GUI.Button(new Rect(panel.x + 90f, panel.y + 218f, panel.width - 180f, 40f), "QUIT", buttonStyle))
                Application.Quit();
        }
    }

    private void EnsureStyles()
    {
        if (titleStyle != null) return;
        titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 30, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        titleStyle.normal.textColor = new Color(0.35f, 0.9f, 1f);
        bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, alignment = TextAnchor.MiddleCenter, wordWrap = true };
        bodyStyle.normal.textColor = Color.white;
        buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 17, fontStyle = FontStyle.Bold };
        Font font = SpriteFactory.GameFont();
        if (font != null) { titleStyle.font = font; bodyStyle.font = font; buttonStyle.font = font; }
        Texture2D blue = SpriteFactory.UiTexture("buttonBlue");
        if (blue != null) buttonStyle.normal.background = blue;
        buttonStyle.border = new RectOffset(12, 12, 8, 8);
    }

    private void DrawOverlay()
    {
        Color old = GUI.color;
        GUI.color = new Color(0.01f, 0.02f, 0.07f, 0.78f);
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = old;
    }

    private Rect Panel(float width, float height)
    {
        width = Mathf.Min(width, Screen.width - 24f);
        height = Mathf.Min(height, Screen.height - 24f);
        return new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
    }
}
