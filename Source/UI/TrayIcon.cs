using UnityEngine;

namespace ClassicLightFX.UI
{
    /// <summary>
    /// Procedurally drawn tray icon: day and night halves around a sun disc.
    /// </summary>
    internal static class TrayIcon
    {
        internal static Texture2D Make()
        {
            const int size = 48;
            var tex = new Texture2D(size, size, TextureFormat.ARGB32, false)
            {
                name = "ClassicLightFX.tray"
            };

            var center = new Vector2((size - 1) / 2f, (size - 1) / 2f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    var p = new Vector2(x, y);
                    float distance = Vector2.Distance(p, center);
                    Color c = new Color(0f, 0f, 0f, 0f);

                    if (distance < 22f)
                    {
                        bool day = (x + (size - 1 - y)) >= size; // diagonal split
                        c = day ? new Color32(84, 110, 160, 255) : new Color32(22, 26, 44, 255);
                    }

                    if (distance < 10f)
                    {
                        c = new Color(1f, 0.9f, 0.55f, 1f); // shared sun disc
                    }
                    else if (distance < 12f)
                    {
                        c = Color.Lerp(c, new Color(1f, 0.9f, 0.55f, 1f), 0.5f);
                    }

                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply(false, true);
            return tex;
        }
    }
}
