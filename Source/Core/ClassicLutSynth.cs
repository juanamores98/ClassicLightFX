using UnityEngine;

namespace ClassicLightFX.Core
{
    /// <summary>
    /// Procedurally computes the classic-look color tables. These tables are
    /// original approximations evaluated in code over an identity cube;
    /// nothing is extracted, converted or derived from third-party assets.
    /// </summary>
    internal static class ClassicLutSynth
    {
        private const int Size = 32;

        internal static Texture3D Generate(string builtinLutName)
        {
            float exposure;
            float warmth;
            float contrast;
            float lift;
            float saturation;
            ClassicResponse(builtinLutName, out exposure, out warmth, out contrast, out lift, out saturation);

            var tex = new Texture3D(Size, Size, Size, TextureFormat.RGBA32, false);
            tex.name = "ClassicLightFX." + builtinLutName;

            var pixels = new Color[Size * Size * Size];
            int index = 0;
            for (int b = 0; b < Size; b++)
            {
                for (int g = 0; g < Size; g++)
                {
                    for (int r = 0; r < Size; r++)
                    {
                        var c = new Color(r / (Size - 1f), g / (Size - 1f), b / (Size - 1f), 1f);

                        c.r *= exposure * (1f + 0.12f * warmth);
                        c.g *= exposure;
                        c.b *= exposure * (1f - 0.12f * warmth);

                        float luma = c.r * 0.299f + c.g * 0.587f + c.b * 0.114f;
                        c.r = Mathf.Lerp(luma, c.r, saturation);
                        c.g = Mathf.Lerp(luma, c.g, saturation);
                        c.b = Mathf.Lerp(luma, c.b, saturation);

                        c.r = ClassicCurve(c.r, contrast, lift);
                        c.g = ClassicCurve(c.g, contrast, lift);
                        c.b = ClassicCurve(c.b, contrast, lift);

                        pixels[index++] = new Color(
                            Mathf.Clamp01(c.r),
                            Mathf.Clamp01(c.g),
                            Mathf.Clamp01(c.b),
                            1f);
                    }
                }
            }

            tex.SetPixels(pixels);
            tex.Apply(false, false);
            return tex;
        }

        private static float ClassicCurve(float v, float contrast, float lift)
        {
            v = Mathf.Clamp01(v);
            v = v + lift * (1f - v);
            float expanded = Mathf.Clamp01(0.5f + (v - 0.5f) * (1f + contrast));
            if (expanded > 0.85f)
            {
                float over = (expanded - 0.85f) / 0.15f;
                expanded = 0.85f + 0.15f * (1f - (1f - over) * (1f - over));
            }

            return expanded;
        }

        private static void ClassicResponse(string builtinLutName, out float exposure, out float warmth, out float contrast, out float lift, out float saturation)
        {
            switch (builtinLutName)
            {
                case "LUTSunny":
                    exposure = 1.08f; warmth = 0.30f; contrast = 0.18f; lift = 0.04f; saturation = 1.10f;
                    break;

                case "LUTTropical":
                    exposure = 1.06f; warmth = 0.35f; contrast = 0.10f; lift = 0.03f; saturation = 1.18f;
                    break;

                case "LUTNorth":
                    exposure = 1.04f; warmth = 0.05f; contrast = 0.16f; lift = 0.05f; saturation = 1.02f;
                    break;

                case "LUTWinter":
                    exposure = 1.05f; warmth = -0.18f; contrast = 0.08f; lift = 0.10f; saturation = 0.88f;
                    break;

                case "LUTeurope":
                default:
                    exposure = 1.05f; warmth = 0.18f; contrast = 0.15f; lift = 0.04f; saturation = 1.06f;
                    break;
            }
        }
    }
}
