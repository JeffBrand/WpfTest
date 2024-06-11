﻿using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using TestWpf.Services;
using Application = System.Windows.Application;
using WinForms = System.Windows.Forms;

namespace TestWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly InterceptKeys.LowLevelKeyboardProc _proc = KeyboardHookCallback;
        private static IntPtr _keyboardHookID = IntPtr.Zero;

        public App()
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Application.Current.Exit += new ExitEventHandler(Current_Exit);
            try
            {
                _keyboardHookID = InterceptKeys.SetHook(_proc);
            }
            catch
            {
                DetachKeyboardHook();
            }
        }


        private void Application_Startup(object sender, StartupEventArgs e)
        {
          


        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            DetachKeyboardHook();
        }

        private static void DetachKeyboardHook()
        {
            if (_keyboardHookID != IntPtr.Zero)
                InterceptKeys.UnhookWindowsHookEx(_keyboardHookID);
        }

        public static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                bool alt = (WinForms.Control.ModifierKeys & Keys.Alt) != 0;
                bool control = (WinForms.Control.ModifierKeys & Keys.Control) != 0;

                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;

                if (alt && key == Keys.F4)
                {
                    Application.Current.Shutdown();
                    return (IntPtr)1; // Handled.
                }

                if (!AllowKeyboardInput(alt, control, key))
                {
                    return (IntPtr)1; // Handled.
                }
            }

            return InterceptKeys.CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);
        }

        /// <summary>Determines whether the specified keyboard input should be allowed to be processed by the system.</summary>
        /// <remarks>Helps block unwanted keys and key combinations that could exit the app, make system changes, etc.</remarks>
        public static bool AllowKeyboardInput(bool alt, bool control, Keys key)
        {
            // Disallow various special keys.
            if (key <= Keys.Back || key == Keys.None ||
                key == Keys.Menu || key == Keys.Pause ||
                key == Keys.Help)
            {
                return false;
            }

            // Disallow ranges of special keys.
            // Currently leaves volume controls enabled; consider if this makes sense.
            // Disables non-existing Keys up to 65534, to err on the side of caution for future keyboard expansion.
            if ((key >= Keys.LWin && key <= Keys.Sleep) ||
                (key >= Keys.KanaMode && key <= Keys.HanjaMode) ||
                (key >= Keys.IMEConvert && key <= Keys.IMEModeChange) ||
                (key >= Keys.BrowserBack && key <= Keys.BrowserHome) ||
                (key >= Keys.MediaNextTrack && key <= Keys.LaunchApplication2) ||
                (key >= Keys.ProcessKey && key <= (Keys)65534))
            {
                return false;
            }

            // Disallow specific key combinations. (These component keys would be OK on their own.)
            if ((alt && key == Keys.Tab) ||
                (alt && key == Keys.Space) ||
                (control && key == Keys.Escape))
            {
                return false;
            }

            // Allow anything else (like letters, numbers, spacebar, braces, and so on).
            return true;
        }

    }

}
