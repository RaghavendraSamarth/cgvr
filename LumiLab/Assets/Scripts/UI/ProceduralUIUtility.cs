using System.Collections.Generic;
using UnityEngine;

namespace LumiLab.UI
{
    /// <summary>
    /// Generates sleek procedural 9-slice rounded sprites and textures for Unity UI.
    /// Eliminates harsh rectangular edges and gives the entire interface a modern,
    /// high-end React/Figma designer aesthetic with smooth rounded corners and clean borders.
    /// </summary>
    public static class ProceduralUIUtility
    {
        private static Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();

        public static Sprite GetRoundedSprite(int radius = 12, Color? fillColor = null, Color? borderColor = null, int borderWidth = 1)
        {
            Color fill = fillColor ?? Color.white;
            Color border = borderColor ?? Color.clear;
            string key = $"round_{radius}_{fill.GetHashCode()}_{border.GetHashCode()}_{borderWidth}";

            if (_spriteCache.TryGetValue(key, out Sprite cached) && cached != null)
            {
                return cached;
            }

            int size = radius * 2 + 10;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color[] pixels = new Color[size * size];
            float r = radius;
            float innerR = Mathf.Max(0, r - borderWidth);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    // Compute distance to nearest corner center
                    float cx = x < size / 2f ? r : size - 1 - r;
                    float cy = y < size / 2f ? r : size - 1 - r;

                    float dx = Mathf.Max(0, (x < size / 2f ? r - x : x - (size - 1 - r)));
                    float dy = Mathf.Max(0, (y < size / 2f ? r - y : y - (size - 1 - r)));
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    if (dist > r)
                    {
                        // Antialiased outer edge
                        float alpha = Mathf.Clamp01(1f - (dist - r));
                        pixels[y * size + x] = new Color(border.r, border.g, border.b, border.a * alpha);
                    }
                    else if (borderWidth > 0 && dist >= innerR)
                    {
                        pixels[y * size + x] = border;
                    }
                    else
                    {
                        pixels[y * size + x] = fill;
                    }
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();

            // 9-slice borders: left, bottom, right, top
            Vector4 sliceBorder = new Vector4(radius, radius, radius, radius);
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, sliceBorder);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetPillSprite(int height = 36)
        {
            return GetRoundedSprite(height / 2, Color.white, new Color(0.2f, 0.22f, 0.26f, 0.8f), 1);
        }
    }
}

