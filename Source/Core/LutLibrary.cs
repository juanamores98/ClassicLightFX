using System;
using System.Reflection;
using UnityEngine;
using ColossalFramework;

namespace ClassicLightFX.Core
{
    /// <summary>
    /// Generates the classic-look color tables procedurally and reads the
    /// active map environment. No external texture is embedded or derived.
    /// </summary>
    internal static class LutLibrary
    {
        internal static Texture3DWrapper Synthesize(string name)
        {
            var volume = ClassicLutSynth.Generate(name);
            var wrapper = ScriptableObject.CreateInstance<Texture3DWrapper>();
            wrapper.name = name;
            wrapper.texture = volume;
            return wrapper;
        }

        internal static string GetEnvironment()
        {
            var simulationManager = Singleton<SimulationManager>.instance;
            var metaData = simulationManager != null ? simulationManager.m_metaData : null;
            return metaData != null ? metaData.m_environment : null;
        }
    }
}
