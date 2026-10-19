using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates simple placeholder sprites at runtime, so the prototype works without imported art assets.
/// </summary>
public static class SpriteFactory
{
    private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();
    private const int TextureSize = 64;

    public static Sprite PlayerShip()
    {
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

    public static Sprite EnemyShip()
    {
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

    public static Sprite Pickup(PickupType type)
    {
        Color color;
        switch (type)
        {
            case PickupType.Health: color = new Color(0.20f, 1f, 0.42f); break;
            case PickupType.TripleShot: color = new Color(0.76f, 0.32f, 1f); break;
            case PickupType.RapidFire: color = new Color(1f, 0.82f, 0.18f); break;
            case PickupType.DamageBoost: color = new Color(1f, 0.34f, 0.15f); break;
            case PickupType.SpeedBoost: color = new Color(0.18f, 0.88f, 1f); break;
            case PickupType.Slow: color = new Color(0.58f, 0.30f, 0.88f); break;
            case PickupType.SlowFire: color = new Color(0.90f, 0.22f, 0.31f); break;
            default: color = new Color(0.58f, 0.10f, 0.19f); break;
        }

        return Circle(color, "pickup-" + type);
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
