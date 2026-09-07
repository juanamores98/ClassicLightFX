using UnityEngine;

namespace ClassicLightFX.Core
{
    /// <summary>
    /// Los dos ajustes de un clic: el del juego sin tocar, y la receta del usuario.
    /// </summary>
    /// <remarks>
    /// <b>Que son.</b> <c>Vanilla</c> deja este mod sin imponer nada: el juego tal cual.
    /// <c>Optimized</c> es la receta calibrada del usuario, la misma que llevaba el preset
    /// «Default» de Render It!+, derivada en su dia de como tenia configurados los mods
    /// clasicos que esta suite sustituye.
    ///
    /// <b>Por que pasan por ApplySuiteSection.</b> Es el mismo camino que recorre un perfil de
    /// suite guardado, con sus validaciones y sus efectos inmediatos. Un atajo que escribiera
    /// los campos por su cuenta se desincronizaria del resto en cuanto alguien anadiera un
    /// ajuste nuevo.
    /// </remarks>
    internal static class QuickPresets
    {
        private const string VanillaXml =
            "<classiclightfx>" +
            "<swapLuts>false</swapLuts>" +
            "<sunColor>false</sunColor>" +
            "<sunStrength>false</sunStrength>" +
            "<sunCoords>false</sunCoords>" +
            "<classicFogMode>false</classicFogMode>" +
            "<classicFogTint>false</classicFogTint>" +
            "</classiclightfx>";

        private const string OptimizedXml =
            "<classiclightfx>" +
            "<swapLuts>false</swapLuts>" +
            "<sunColor>false</sunColor>" +
            "<sunStrength>false</sunStrength>" +
            "<sunCoords>false</sunCoords>" +
            "<classicFogMode>false</classicFogMode>" +
            "<classicFogTint>false</classicFogTint>" +
            "<applyOnLoad>true</applyOnLoad>" +
            "</classiclightfx>";

        internal static bool ApplyVanilla()
        {
            return Apply(VanillaXml, "Vanilla");
        }

        internal static bool ApplyOptimized()
        {
            return Apply(OptimizedXml, "Optimized");
        }

        private static bool Apply(string xml, string name)
        {
            bool ok = ClassicLightFX.ClassicLightFXMod.ApplySuiteSection(xml);
            Debug.Log("[ClassicLightFX] preset " + name + (ok ? " aplicado" : " RECHAZADO"));
            return ok;
        }
    }
}
