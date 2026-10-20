using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Loads Kenney Space Shooter Remastered sprites from Assets/Resources/Kenney.
/// Keeps procedural fallbacks so the game still runs if an optional asset has not been copied yet.
/// </summary>
public static class SpriteFactory
{
    private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();
    private const int TextureSize = 64;
    private const string Root = "Kenney/";

    public static Sprite LoadKenneySprite(string relativePath)
    {
        return Resources.Load<Sprite>(Root + relativePath);
    }

    public static Sprite PlayerShip()
    {
        Sprite imported = LoadKenneySprite("PNG/playerShip1_blue");
        if (imported != null)
            return imported;

        return Polygon("player-ship", new[]
        {
            new Vector2(0f, 0.50f),
            new Vector2(0.30f, -0.28f),
            new Vector2(0.09f, -0.20f),
            new Vector2(0f, -0.38f),
            new Vector2(-0.09f, -0.20f),
            new Vector2(-0.30f, -0.28f)
        }, new Color(0.18f, 0.84f, 1f));
    }

    public static Sprite PlayerDamage(int tier)
    {
        tier = Mathf.Clamp(tier, 1, 3);
        Sprite imported = LoadKenneySprite("PNG/Damage/playerShip1_damage" + tier);
        return imported != null ? imported : PlayerShip();
    }

    public static Sprite EnemyShip()
    {
        string[] families = { "Black", "Blue", "Green", "Red" };
        string family = families[Random.Range(0, families.Length)];
        int variant = Random.Range(1, 6);
        Sprite imported = LoadKenneySprite("PNG/Enemies/enemy" + family + variant);
        if (imported != null)
            return imported;

        return Polygon("enemy-ship", new[]
        {
            new Vector2(0f, -0.48f),
            new Vector2(0.38f, 0.28f),
            new Vector2(0.10f, 0.18f),
            new Vector2(0f, 0.36f),
            new Vector2(-0.10f, 0.18f),
            new Vector2(-0.38f, 0.28f)
        }, new Color(1f, 0.23f, 0.31f));
    }

    public static Sprite BossShip(int level)
    {
        string[] bossPaths =
        {
            "PNG/Enemies/enemyBlack5",
            "PNG/Enemies/enemyBlue5",
            "PNG/Enemies/enemyGreen5",
            "PNG/Enemies/enemyRed5",
            "PNG/ufoYellow"
        };

        Sprite imported = LoadKenneySprite(bossPaths[Mathf.Clamp(level, 1, bossPaths.Length) - 1]);
        if (imported != null)
            return imported;

        Color color = level % 3 == 1
            ? new Color(1f, 0.22f, 0.34f)
            : level % 3 == 2
                ? new Color(0.72f, 0.28f, 1f)
                : new Color(1f, 0.48f, 0.12f);

        return Polygon("boss-" + level, new[]
        {
            new Vector2(0f, 0.43f),
            new Vector2(0.18f, 0.25f),
            new Vector2(0.48f, 0.28f),
            new Vector2(0.37f, -0.02f),
            new Vector2(0.26f, -0.34f),
            new Vector2(0.08f, -0.24f),
            new Vector2(0f, -0.42f),
            new Vector2(-0.08f, -0.24f),
            new Vector2(-0.26f, -0.34f),
            new Vector2(-0.37f, -0.02f),
            new Vector2(-0.48f, 0.28f),
            new Vector2(-0.18f, 0.25f)
        }, color);
    }

    public static Sprite Asteroid()
    {
        string[] paths =
        {
            "PNG/Meteors/meteorBrown_big1", "PNG/Meteors/meteorBrown_big2",
            "PNG/Meteors/meteorBrown_big3", "PNG/Meteors/meteorBrown_big4",
            "PNG/Meteors/meteorGrey_big1", "PNG/Meteors/meteorGrey_big2",
            "PNG/Meteors/meteorGrey_big3", "PNG/Meteors/meteorGrey_big4",
            "PNG/Meteors/meteorBrown_med1", "PNG/Meteors/meteorBrown_med3",
            "PNG/Meteors/meteorGrey_med1", "PNG/Meteors/meteorGrey_med2",
            "PNG/Meteors/meteorBrown_small1", "PNG/Meteors/meteorBrown_small2",
            "PNG/Meteors/meteorGrey_small1", "PNG/Meteors/meteorGrey_small2",
            "PNG/Meteors/meteorBrown_tiny1", "PNG/Meteors/meteorBrown_tiny2",
            "PNG/Meteors/meteorGrey_tiny1", "PNG/Meteors/meteorGrey_tiny2"
        };

        // A random pick gives the asteroid field more visual variety.
        for (int attempt = 0; attempt < paths.Length; attempt++)
        {
            Sprite imported = LoadKenneySprite(paths[Random.Range(0, paths.Length)]);
            if (imported != null)
                return imported;
        }

        return Polygon("asteroid", new[]
        {
            new Vector2(-0.05f, 0.48f),
            new Vector2(0.28f, 0.39f),
            new Vector2(0.47f, 0.12f),
            new Vector2(0.36f, -0.22f),
            new Vector2(0.12f, -0.44f),
            new Vector2(-0.24f, -0.40f),
            new Vector2(-0.46f, -0.13f),
            new Vector2(-0.39f, 0.20f),
            new Vector2(-0.22f, 0.35f)
        }, new Color(0.58f, 0.53f, 0.48f));
    }

    public static Sprite Background(int level)
    {
        string[] paths =
        {
            "Backgrounds/blue",
            "Backgrounds/darkPurple",
            "Backgrounds/purple",
            "Backgrounds/blue",
            "Backgrounds/black"
        };

        return Resources.Load<Sprite>(Root + paths[Mathf.Clamp(level, 1, paths.Length) - 1]);
    }

    public static Sprite Star()
    {
        Sprite imported = LoadKenneySprite("PNG/Effects/star" + Random.Range(1, 4));
        return imported != null ? imported : Circle(new Color(0.72f, 0.83f, 1f));
    }

    public static Sprite Shield(int frame)
    {
        Sprite imported = LoadKenneySprite("PNG/Effects/shield" + Mathf.Clamp(frame, 1, 3));
        return imported;
    }

    public static Sprite ExplosionFrame(int frame)
    {
        string frameName = Mathf.Clamp(frame, 0, 19).ToString("00");
        Sprite imported = LoadKenneySprite("PNG/Effects/fire" + frameName);
        if (imported != null)
            return imported;

        float amount = Mathf.Clamp01(frame / 19f);
        return Circle(Color.Lerp(new Color(1f, 0.82f, 0.18f), new Color(1f, 0.18f, 0.03f), amount),
            "fallback-explosion-" + frame);
    }

    public static Sprite Projectile(bool playerOwned)
    {
        Sprite imported = LoadKenneySprite(playerOwned ? "PNG/Lasers/laserBlue01" : "PNG/Lasers/laserRed01");
        return imported != null
            ? imported
            : Circle(playerOwned ? new Color(0.35f, 0.94f, 1f) : new Color(1f, 0.30f, 0.34f));
    }

    public static Sprite Pickup(PickupType type)
    {
        string path;
        Color fallbackColor;

        switch (type)
        {
            case PickupType.Health:
                path = "PNG/Power-ups/pill_green";
                fallbackColor = new Color(0.20f, 1f, 0.42f);
                break;
            case PickupType.TripleShot:
                path = "PNG/Power-ups/powerupBlue_star";
                fallbackColor = new Color(0.76f, 0.32f, 1f);
                break;
            case PickupType.RapidFire:
                path = "PNG/Power-ups/powerupYellow_bolt";
                fallbackColor = new Color(1f, 0.82f, 0.18f);
                break;
            case PickupType.DamageBoost:
                path = "PNG/Power-ups/powerupRed_bolt";
                fallbackColor = new Color(1f, 0.34f, 0.15f);
                break;
            case PickupType.SpeedBoost:
                path = "PNG/Effects/speed";
                fallbackColor = new Color(0.18f, 0.88f, 1f);
                break;
            case PickupType.Slow:
                path = "PNG/Power-ups/pill_red";
                fallbackColor = new Color(0.58f, 0.30f, 0.88f);
                break;
            case PickupType.SlowFire:
                path = "PNG/Power-ups/powerupRed";
                fallbackColor = new Color(0.90f, 0.22f, 0.31f);
                break;
            default:
                path = "PNG/Power-ups/powerupRed_shield";
                fallbackColor = new Color(0.58f, 0.10f, 0.19f);
                break;
        }

        Sprite imported = LoadKenneySprite(path);
        return imported != null ? imported : Circle(fallbackColor, "pickup-" + type);
    }

    public static Sprite UiSprite(string name)
    {
        return LoadKenneySprite("PNG/UI/" + name);
    }

    public static Texture2D UiTexture(string name)
    {
        Sprite sprite = UiSprite(name);
        if (sprite != null)
            return sprite.texture;

        return Resources.Load<Texture2D>(Root + "PNG/UI/" + name);
    }

    public static Font GameFont(bool thin = false)
    {
        return Resources.Load<Font>(Root + (thin ? "Bonus/kenvector_future_thin" : "Bonus/kenvector_future"));
    }

    public static Sprite Engine()
    {
        return LoadKenneySprite("PNG/Parts/engine3");
    }

    public static Sprite Circle(Color color, string cacheKey = null)
    {
        string key = cacheKey ?? ("circle-" + ColorKey(color));
        if (Cache.TryGetValue(key, out Sprite cached))
            return cached;

        Texture2D texture = new Texture2D(TextureSize, TextureSize, TextureFormat.RGBA32, false);
        texture.name = key;
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        Color32[] pixels = new Color32[TextureSize * TextureSize];

        for (int y = 0; y < TextureSize; y++)
        {
            for (int x = 0; x < TextureSize; x++)
            {
                float nx = (x + 0.5f) / TextureSize - 0.5f;
                float ny = (y + 0.5f) / TextureSize - 0.5f;
                float radius = Mathf.Sqrt(nx * nx + ny * ny);
                if (radius <= 0.46f)
                {
                    Color shaded = radius > 0.37f ? color * 0.72f : Color.Lerp(color, Color.white, 0.12f);
                    pixels[y * TextureSize + x] = shaded;
                }
                else
                {
                    pixels[y * TextureSize + x] = new Color(0f, 0f, 0f, 0f);
                }
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply();
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, TextureSize, TextureSize), new Vector2(0.5f, 0.5f), TextureSize);
        Cache[key] = sprite;
        return sprite;
    }

    private static Sprite Polygon(string key, Vector2[] points, Color color)
    {
        if (Cache.TryGetValue(key, out Sprite cached))
            return cached;

        Texture2D texture = new Texture2D(TextureSize, TextureSize, TextureFormat.RGBA32, false);
        texture.name = key;
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        Color32[] pixels = new Color32[TextureSize * TextureSize];

        for (int y = 0; y < TextureSize; y++)
        {
            for (int x = 0; x < TextureSize; x++)
            {
                Vector2 point = new Vector2((x + 0.5f) / TextureSize - 0.5f, (y + 0.5f) / TextureSize - 0.5f);
                if (PointInPolygon(point, points))
                {
                    float highlight = Mathf.Clamp01(0.15f - Mathf.Abs(point.x) * 0.12f);
                    pixels[y * TextureSize + x] = Color.Lerp(color, Color.white, highlight);
                }
                else
                {
                    pixels[y * TextureSize + x] = new Color(0f, 0f, 0f, 0f);
                }
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply();
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, TextureSize, TextureSize), new Vector2(0.5f, 0.5f), TextureSize);
        Cache[key] = sprite;
        return sprite;
    }

    private static bool PointInPolygon(Vector2 point, Vector2[] polygon)
    {
        bool inside = false;
        for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
        {
            bool crosses = ((polygon[i].y > point.y) != (polygon[j].y > point.y)) &&
                (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) /
                ((polygon[j].y - polygon[i].y) + 0.00001f) + polygon[i].x);
            if (crosses)
                inside = !inside;
        }

        return inside;
    }

    private static string ColorKey(Color color)
    {
        Color32 c = color;
        return c.r + "-" + c.g + "-" + c.b + "-" + c.a;
    }
}
