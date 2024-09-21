using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MezzexEyeApp.Platforms.Windows
{
    public class CustomWindow
    {
        private IntPtr _hwnd;
        private IntPtr _hookId = IntPtr.Zero;
        private User32.LowLevelKeyboardProc _proc;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowLong(IntPtr hwnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public CustomWindow(IntPtr hwnd)
        {
            _hwnd = hwnd;
            _proc = HookCallback;

            // Make the window fullscreen
            MakeWindowFullscreen(_hwnd);

            // Set the keyboard hook to block keys
            _hookId = SetHook(_proc);
        }

        private void MakeWindowFullscreen(IntPtr hwnd)
        {
            const int GWL_STYLE = -16; // Window style constant
            const int WS_POPUP = unchecked((int)0x80000000); // Popup window style for fullscreen
            const int SW_MAXIMIZE = 3;  // Command to maximize window

            // Remove the window's title bar and borders
            int style = GetWindowLong(hwnd, GWL_STYLE);
            SetWindowLong(hwnd, GWL_STYLE, style | WS_POPUP);

            // Maximize the window to cover the entire screen
            ShowWindow(hwnd, SW_MAXIMIZE);
        }

        // Hook callback to block specific keys (Alt, Esc, Ctrl, Win)
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)User32.WindowMessage.WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                // Block Alt, Esc, Ctrl, and Windows keys
                if (vkCode == (int)User32.VirtualKey.VK_ESCAPE ||
                    vkCode == (int)User32.VirtualKey.VK_LWIN ||
                    vkCode == (int)User32.VirtualKey.VK_RWIN ||
                    vkCode == (int)User32.VirtualKey.VK_CONTROL ||
                    vkCode == (int)User32.VirtualKey.VK_MENU)
                {
                    return (IntPtr)1; // Block the key press
                }
            }
            return User32.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private IntPtr SetHook(User32.LowLevelKeyboardProc proc)
        {
            using var curProcess = Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;

            // Use Kernel32 to get the module handle
            return User32.SetWindowsHookEx(
                User32.WindowsHookType.WH_KEYBOARD_LL,
                proc,
                Kernel32.GetModuleHandle(curModule.ModuleName), // Use Kernel32.GetModuleHandle
                0);
        }

        public void RemoveHook()
        {
            if (_hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookId); // Call the imported function here
                _hookId = IntPtr.Zero;
            }
        }
    }

    internal class User32
    {
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public enum VirtualKey
        {
            VK_ESCAPE = 0x1B,
            VK_LWIN = 0x5B,
            VK_RWIN = 0x5C,
            VK_CONTROL = 0x11,
            VK_MENU = 0x12 // Alt key
        }

        public enum WindowMessage
        {
            WM_KEYDOWN = 0x0100
        }

        public enum WindowsHookType
        {
            WH_KEYBOARD_LL = 13
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(WindowsHookType idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
    }

    internal class Kernel32
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
