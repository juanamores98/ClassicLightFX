using System;
using System.Reflection;
using UnityEngine;
using ColossalFramework;

namespace ClassicLightFX.Core
{
    /// <summary>
    /// Loads the classic LUT textures embedded in the assembly and reads the
    /// active map environment.
    /// </summary>
    internal static class LutLibrary
    {
        internal static Texture3DWrapper Load(string resourceName, string name)
        {
            var flat = LoadTextureFromAssembly(resourceName);
            flat.name = name;

            var wrapper = ScriptableObject.CreateInstance<Texture3DWrapper>();
            wrapper.name = name;
            wrapper.texture = Texture3DWrapper.Convert(flat);
            return wrapper;
        }

        private static Texture2D LoadTextureFromAssembly(string path)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(path))
            {
                if (stream == null)
                {
                    throw new ArgumentException("Embedded resource not found: " + path);
                }

                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);

                var texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                texture.LoadImage(buffer);
                // The wrapper conversion below reads the pixels back, so the
                // texture must stay readable (makeNoLongerReadable: false).
                texture.Apply(false, false);
                return texture;
            }
        }

        internal static string GetEnvironment()
        {
            var simulationManager = Singleton<SimulationManager>.instance;
            var metaData = simulationManager != null ? simulationManager.m_metaData : null;
            return metaData != null ? metaData.m_environment : null;
        }
    }
}
