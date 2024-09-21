#if WINDOWS
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI;              // Namespace for AppWindow
using Microsoft.UI.Windowing;    // For AppWindow and managing window settings
using WinRT.Interop; 
using System.Runtime.InteropServices;
#endif

using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.AspNetCore.Components;

namespace MezzexEyeApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage(BlazorWebView blazorWebView)
        {
            InitializeComponent();

            // Programmatically navigate to login
            var navManager = (blazorWebView.Handler.PlatformView as IServiceProvider).GetService<NavigationManager>();
            navManager.NavigateTo("/login");

#if WINDOWS
            var window = App.Current.Windows[0].Handler.PlatformView as Microsoft.UI.Xaml.Window;

            if (window != null)
            {
                // Set the window to fullscreen for the login page
                var appWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(WinRT.Interop.WindowNative.GetWindowHandle(window)));
                appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);

                // Block system keys like Alt, Ctrl, Esc, Win
                BlockSystemKeys(window);
            }
#endif
        }

#if WINDOWS
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private void BlockSystemKeys(Microsoft.UI.Xaml.Window window)
        {
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);

            // Block keys: Ctrl (0x11), Alt (0x12), Esc (0x1B), Left Windows Key (0x5B)
            RegisterHotKey(windowHandle, 1, 1, 0x11); // Ctrl
            RegisterHotKey(windowHandle, 2, 1, 0x12); // Alt
            RegisterHotKey(windowHandle, 3, 1, 0x1B); // Esc
            RegisterHotKey(windowHandle, 4, 1, 0x5B); // Left Windows Key
        }

        public void UnblockSystemKeys()
        {
            var window = App.Current.Windows[0].Handler.PlatformView as Microsoft.UI.Xaml.Window;
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);

            // Unblock keys
            UnregisterHotKey(windowHandle, 1);
            UnregisterHotKey(windowHandle, 2);
            UnregisterHotKey(windowHandle, 3);
            UnregisterHotKey(windowHandle, 4);
        }

        public void EnableWindowFeatures()
        {
            var window = App.Current.Windows[0].Handler.PlatformView as Microsoft.UI.Xaml.Window;

            if (window != null)
            {
                // Restore window features after login
                var appWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(WinRT.Interop.WindowNative.GetWindowHandle(window)));
                appWindow.SetPresenter(AppWindowPresenterKind.Overlapped); // Restore normal window

                // Unblock system keys
                UnblockSystemKeys();
            }
        }
#endif
    }
}
