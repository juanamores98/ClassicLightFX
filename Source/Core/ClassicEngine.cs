using UnityEngine;
using ClassicLightFX.UI;

namespace ClassicLightFX.Core
{
    /// <summary>
    /// Scene host for the in-game quick panel (F9).
    /// </summary>
    public class ClassicEngine : MonoBehaviour
    {
        private static bool _open;

        private ClassicWindow _window;
        private int _windowId;

        internal static void OpenWindow()
        {
            _open = true;
        }

        internal static void CloseWindow()
        {
            _open = false;
            Options.ModOptions.SaveImmediate();
        }

        private void OnDestroy()
        {
            Options.ModOptions.SaveImmediate();
        }

        private void Start()
        {
            _windowId = GetInstanceID();
            _window = new ClassicWindow();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                _open = !_open;
                if (!_open)
                {
                    Options.ModOptions.SaveImmediate();
                }
            }

            Options.ModOptions.CheckPendingSave();
        }

        private void OnGUI()
        {
            if (_open)
            {
                _window.Draw(_windowId);
            }
        }
    }
}

