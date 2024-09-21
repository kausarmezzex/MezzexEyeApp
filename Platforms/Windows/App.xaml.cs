using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.UI.Xaml;

namespace MezzexEyeApp.WinUI
{
    public partial class App : MauiWinUIApplication
    {
        public App()
        {
            InitializeComponent();
        }

        // Implement the abstract method to return your MauiApp
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
