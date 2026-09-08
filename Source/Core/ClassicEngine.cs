using UnityEngine;
namespace ClassicLightFX.Core
{
    public class ClassicEngine : MonoBehaviour
    {
        internal static void OpenWindow() { FxModule.OpenStandalone(); }
        internal static void CloseWindow() { FxModule.CloseStandalone(); }
        public static void ToggleWindow() { FxModule.OpenStandalone(true); }
        private void Update() { if (Input.GetKeyDown(KeyCode.F9)) ToggleWindow(); Options.ModOptions.CheckPendingSave(); }
        private void OnDestroy() { Options.ModOptions.SaveImmediate(); FxModule.CloseStandalone(); }
    }
}
