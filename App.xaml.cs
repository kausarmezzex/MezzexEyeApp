using Microsoft.AspNetCore.Components.WebView.Maui;

namespace MezzexEyeApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage(BlazorWebView);
        }
    }
}
