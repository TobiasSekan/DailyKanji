using SharpDX.DirectInput;
using System.Diagnostics;
using System.Linq;

namespace DailyKanjiLogic.Helper
{
    /// <summary>
    /// Helper class to easier use Direct Input API (via Sharpen DX)
    /// </summary>
    public static class DirectInputHelper
    {
        /// <summary>
        /// Return the first found gamepad
        /// </summary>
        /// <returns>A gamepad or <c>null</c> when no gamepad was found</returns>
        public static Joystick? GetFirstGamepad()
        {
            var directInput = new DirectInput();

            var inputDevice = directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly).FirstOrDefault();
            if(inputDevice == null)
            {
                Debug.WriteLine("No gamepad found");
                return null;
            }

            var gamepad = new Joystick(directInput, inputDevice.InstanceGuid);

            Debug.WriteLine($"Found gamepad - with {gamepad.Capabilities.ButtonCount} buttons");

            gamepad.Acquire();

            return gamepad;
        }
    }
}
