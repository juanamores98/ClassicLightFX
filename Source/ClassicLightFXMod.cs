using ICities;
using ClassicLightFX.Core;
using ClassicLightFX.Options;

namespace ClassicLightFX
{
    /// <summary>
    /// ClassicLightFX v2 entry point: restores the pre-After Dark look with
    /// its own option model and lifecycle.
    /// </summary>
    public class ClassicLightFXMod : IUserMod
    {
        public static string ActiveClaims
        {
            get
            {
                if (!ClassicLook.Active || ModOptions.Instance.VanillaMode) return string.Empty;
                return (ClassicLook.Owns(ClassicFeature.SunGradient) ? "lightColor," : "")
                    + (ClassicLook.Owns(ClassicFeature.SunPower) ? "sunIntensity,exposure," : "")
                    + (ClassicLook.Owns(ClassicFeature.SunPosition) ? "sunPosition" : "");
            }
        }

        private const string Version = "2.0.0";

        public string Name
        {
            get { return "ClassicLightFX v2"; }
        }

        public string Description
        {
            get { return "Reversible classic-style lighting: original LUT approximations, sun controls and classic fog. v" + Version; }
        }

        public void OnEnabled()
        {
            ModOptions.Load();
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            OptionsPanel.Build(helper);
        }

        /// <summary>
        /// Suite profile coordinator entry point: applies a <classiclightfx> XML section.
        /// </summary>
        public static bool ApplySuiteSection(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return false;
            }

            try
            {
                var doc = new System.Xml.XmlDocument();
                doc.LoadXml(xml);
                return ApplySuiteSection(doc.DocumentElement);
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogException(e);
                return false;
            }
        }

        public static bool ApplySuiteSection(System.Xml.XmlElement element)
        {
            if (element == null || !element.Name.Equals("classiclightfx", System.StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            try
            {
                var opt = new ModOptions.OptionsDocument { VanillaMode = ModOptions.Instance.VanillaMode };
                foreach (System.Xml.XmlNode node in element.ChildNodes)
                {
                    if (node.NodeType != System.Xml.XmlNodeType.Element) continue;
                    string name = node.Name.ToLowerInvariant();
                    string val = node.InnerText != null ? node.InnerText.Trim() : string.Empty;


                    if (name == "swapluts") opt.SwapLuts = bool.Parse(val);
                    else if (name == "suncolor") opt.SunColor = bool.Parse(val);
                    else if (name == "sunstrength") opt.SunStrength = bool.Parse(val);
                    else if (name == "suncoords") opt.SunCoords = bool.Parse(val);
                    else if (name == "classicfogmode") opt.ClassicFogMode = bool.Parse(val);
                    else if (name == "classicfogtint") opt.ClassicFogTint = bool.Parse(val);
                    else if (name == "classicfogwithcycle") opt.ClassicFogWithCycle = bool.Parse(val);
                    else if (name == "vanillamode") opt.VanillaMode = bool.Parse(val);
                    else if (name == "applyonload") opt.ApplyOnLoad = bool.Parse(val);
                }

                opt.Apply();
                ClassicLook.ApplyFromOptions();
                ModOptions.Save();
                return true;
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogException(e);
                return false;
            }
        }

        public static string ExportSuiteSection()
        {
            var opt = ModOptions.Instance;
            return string.Format(
                "  <classiclightfx>\n" +
                "    <swapLuts>{0}</swapLuts>\n" +
                "    <sunColor>{1}</sunColor>\n" +
                "    <sunStrength>{2}</sunStrength>\n" +
                "    <sunCoords>{3}</sunCoords>\n" +
                "    <classicFogMode>{4}</classicFogMode>\n" +
                "    <classicFogTint>{5}</classicFogTint>\n" +
                "    <classicFogWithCycle>{7}</classicFogWithCycle>\n" +
                "    <applyOnLoad>{6}</applyOnLoad>\n" +
                "    <vanillaMode>{8}</vanillaMode>\n" +
                "  </classiclightfx>",
                opt.SwapLuts.ToString().ToLowerInvariant(),
                opt.SunColor.ToString().ToLowerInvariant(),
                opt.SunStrength.ToString().ToLowerInvariant(),
                opt.SunCoords.ToString().ToLowerInvariant(),
                opt.ClassicFogMode.ToString().ToLowerInvariant(),
                opt.ClassicFogTint.ToString().ToLowerInvariant(),
                opt.ApplyOnLoad.ToString().ToLowerInvariant(),
                opt.ClassicFogWithCycle.ToString().ToLowerInvariant(),
                opt.VanillaMode.ToString().ToLowerInvariant());
        }
    }


    public class LoadingExtension : LoadingExtensionBase
    {
        private const string HostObjectName = "ClassicLightFX2";

        private UnityEngine.GameObject _host;

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            ModOptions.Load();
            ClassicLook.Attach();
            if (ModOptions.Instance.ApplyOnLoad)
            {
                ClassicLook.ApplyFromOptions();
            }

            DestroyHosts();
            _host = new UnityEngine.GameObject(HostObjectName);
            _host.AddComponent<Core.ClassicEngine>();

            UI.UuiButton.Register(
                "ClassicLightFX v2",
                "Classic lighting quick switches (F9)",
                UI.TrayIcon.Make(),
                show => Core.ClassicEngine.OpenWindow());
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            UI.UuiButton.Unregister();
            DestroyHosts();
            ClassicLook.Detach();
        }

        public void OnDisabled()
        {
            ClassicLook.Detach();
            DestroyHosts();
        }

        private static void DestroyHosts()
        {
            while (true)
            {
                var leftover = UnityEngine.GameObject.Find(HostObjectName);
                if (!leftover)
                {
                    break;
                }

                UnityEngine.Object.DestroyImmediate(leftover);
            }
        }
    }
}
