using LedCSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;

namespace WaveEchoRGB
{
    public class RGBHandler
    {
        private CancellationTokenSource cancellation;
        private const int ANIMATION_INTERVAL = 10;

        private long _elapsedTimes = 0;
        private GlobalKeyboardHook _hook;

        public event Action<string> StatusChanged;

        public readonly Dictionary<int, PressedKey> _keyboardNames;
        private int lastKey;

        public RGBHandler()
        {
            _keyboardNames = KeyboardInfo.WinToHid.ToDictionary(p => p.Key, p => new PressedKey(p.Value));
            _hook = new GlobalKeyboardHook();
        }

        public void Initialize()
        {
            if (LogitechGSDK.LogiLedInitWithName("Wave Echo RGB"))
            {   //if no error...
                Thread.Sleep(250); //wait for LGS to wake up
                _hook.KeyboardPressed += OnKeyboardPressed;
                _elapsedTimes = 0;
                LogitechGSDK.LogiLedSaveCurrentLighting();
                LogitechGSDK.LogiLedSetLighting(10, 10, 10);
                int[] version = LGSversion();
                if (version[0] == -1)
                {
                    StatusChanged?.Invoke("Successfully connected to LGS but version unknown.");
                }
                else
                {
                    StatusChanged?.Invoke($"Successfully connected to LGS version {version[0]}.{version[1]}.{version[2]}.");
                }
            }
            else
            {
                StatusChanged?.Invoke("Failed to connect to LGS.");
            }
        }

        private async void OnKeyboardPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown || e.KeyboardState == GlobalKeyboardHook.KeyboardState.SysKeyDown)
            {
                lastKey = e.KeyboardData.Flags * 256 + e.KeyboardData.HardwareScanCode;
                var key = _keyboardNames[lastKey];
                key.IsPressed = true;
                var keyName = key.Name;
                StatusChanged?.Invoke($"HSC + Flags {e.KeyboardData.Flags} : {e.KeyboardData.HardwareScanCode} => [0x{keyName:X}({(int)keyName}) : {keyName}].");

                await Task.Run(() =>
                {
                    var (or, og, ob) = GetRGB(_elapsedTimes + 100);
                    var (nr, ng, nb) = GetRGB(_elapsedTimes - 100);

                    for (int i = 0; i < 100; i++)
                    {
                        var p = i / 100.0;
                        var r = or + p * (nr - or);
                        var g = og + p * (ng - og);
                        var b = ob + p * (nb - ob);
                        LogitechGSDK.LogiLedSetLightingForKeyWithKeyName(key.Name, (int)r, (int)g, (int)b);

                        Thread.Sleep(10);
                    }
                }).ConfigureAwait(false);

                key.IsPressed = false;
            }
        }

        public void Start()
        {
            cancellation = new CancellationTokenSource();
            Task.Run(() => UpdateLoop(cancellation.Token));
            StatusChanged?.Invoke("Started.");
        }

        public void Pause()
        {
            cancellation.Cancel();
            Thread.Sleep(1000);
            StatusChanged?.Invoke("Stopped.");
        }

        internal int[] LGSversion()
        {
            int major = 0;
            int minor = 0;
            int build = 0;
            bool error = LogitechGSDK.LogiLedGetSdkVersion(ref major, ref minor, ref build);
            //in testing, this call always returned false.  Therefore ignore the result
            int[] result = { major, minor, build };
            return result;
        }

        private void UpdateLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var (r, g, b) = GetRGB(_elapsedTimes);

                foreach (var key in _keyboardNames.Values)
                {
                    if (!key.IsPressed) LogitechGSDK.LogiLedSetLightingForKeyWithKeyName(key.Name, r - 40, g - 40, b - 40);
                }

                _elapsedTimes++;

                Thread.Sleep(10);
            }
        }

        private (int R, int G, int B) GetRGB(long i,
            int center = 128, int width = 127,
            float freq1 = 0.005f, float freq2 = 0.005f, float freq3 = 0.005f,
            int phase1 = 0, int phase2 = 2, int phase3 = 4)
        {

            const float toPercent = 100.0f / 255.0f;

            var red = Math.Sin(freq1 * i + phase1) * width + center;
            var green = Math.Sin(freq2 * i + phase2) * width + center;
            var blue = Math.Sin(freq3 * i + phase3) * width + center;

            red *= toPercent;
            green *= toPercent;
            blue *= toPercent;

            return ((int)red, (int)green, (int)blue);
        }

        public void Stop()
        {
            Pause();
            _hook.KeyboardPressed -= OnKeyboardPressed;
            LogitechGSDK.LogiLedRestoreLighting();
            LogitechGSDK.LogiLedShutdown();
        }
    }
}
