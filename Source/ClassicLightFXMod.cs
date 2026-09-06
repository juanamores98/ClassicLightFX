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
        private const string Version = "2.0.0";

        public string Name
        {
            get { return "ClassicLightFX v2"; }
        }

        public string Description
        {
            get { return "Restores the pre-After Dark lighting: LUTs, sun color, strength and position, classic fog. v" + Version; }
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
            if (element == null)
            {
                return false;
            }

            try
            {
                var opt = ModOptions.Instance;
                foreach (System.Xml.XmlNode node in element.ChildNodes)
                {
                    if (node.NodeType != System.Xml.XmlNodeType.Element) continue;
                    string name = node.Name.ToLowerInvariant();
                    string val = node.InnerText != null ? node.InnerText.Trim() : string.Empty;
                    bool b;

                    if (name == "swapluts" && bool.TryParse(val, out b)) opt.SwapLuts = b;
                    else if (name == "suncolor" && bool.TryParse(val, out b)) opt.SunColor = b;
                    else if (name == "sunstrength" && bool.TryParse(val, out b)) opt.SunStrength = b;
                    else if (name == "suncoords" && bool.TryParse(val, out b)) opt.SunCoords = b;
                    else if (name == "classicfogmode" && bool.TryParse(val, out b)) opt.ClassicFogMode = b;
                    else if (name == "classicfogtint" && bool.TryParse(val, out b)) opt.ClassicFogTint = b;
                    else if (name == "applyonload" && bool.TryParse(val, out b)) opt.ApplyOnLoad = b;
                }

                ClassicLook.Apply(ClassicFeature.StockTables, opt.SwapLuts);
                ClassicLook.Apply(ClassicFeature.SunGradient, opt.SunColor);
                ClassicLook.Apply(ClassicFeature.SunPower, opt.SunStrength);
                ClassicLook.Apply(ClassicFeature.SunPosition, opt.SunCoords);
                ClassicLook.Apply(ClassicFeature.FogEffect, opt.ClassicFogMode);
                ClassicLook.Apply(ClassicFeature.FogTint, opt.ClassicFogTint);

                ModOptions.SaveImmediate();
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
                "    <applyOnLoad>{6}</applyOnLoad>\n" +
                "  </classiclightfx>",
                opt.SwapLuts.ToString().ToLowerInvariant(),
                opt.SunColor.ToString().ToLowerInvariant(),
                opt.SunStrength.ToString().ToLowerInvariant(),
                opt.SunCoords.ToString().ToLowerInvariant(),
                opt.ClassicFogMode.ToString().ToLowerInvariant(),
                opt.ClassicFogTint.ToString().ToLowerInvariant(),
                opt.ApplyOnLoad.ToString().ToLowerInvariant());
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
