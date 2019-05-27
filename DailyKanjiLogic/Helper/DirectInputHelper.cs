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
        public static Joystick? GetFirstGamePad()
        {
            using(var directInput = new DirectInput())
            {
                var inputDevice = directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly).FirstOrDefault();
                if(inputDevice is null)
                {
                    Debug.WriteLine("No game-pad found");
                    return null;
                }

                using(var gamePad = new Joystick(directInput, inputDevice.InstanceGuid))
                {
                    Debug.WriteLine($"Found game-pad - with {gamePad.Capabilities.ButtonCount} buttons");

                    gamePad.Acquire();

                    return gamePad;
                }
            }
        }
    }
}
