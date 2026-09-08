using System.IO;
using System.Reflection;

namespace ClassicLightFX.Core
{
    internal static class QuickPresets
    {
        internal static bool ApplyVanilla()
        {
            return ClassicLightFXMod.ApplySuiteSection("<classiclightfx><vanillaMode>true</vanillaMode></classiclightfx>");
        }

        internal static bool ApplyOptimized()
        {
            using (var stream = typeof(QuickPresets).Assembly.GetManifestResourceStream("ClassicLightFX.BuiltIns.Optimized.xml"))
            {
                if (stream == null) return false;
                using (var reader = new StreamReader(stream)) return ClassicLightFXMod.ApplySuiteSection(reader.ReadToEnd());
            }
        }
    }
}
