using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MezzexEyeApp.Components.Pages
{
    public partial class Login : IDisposable
    {
        private string Username { get; set; }
        private string Password { get; set; }
        private string LoginMessage { get; set; } // Message for login success/failure
        private string LoginMessageCssClass { get; set; } // CSS class for alert styling
        private List<string> UsernamesCache = new();
        private List<string> suggestions = new();

        protected override async Task OnInitializedAsync()
        {
            await PreloadUsernames();
        }

        private async Task PreloadUsernames()
        {
            try
            {
                UsernamesCache = await Http.GetFromJsonAsync<List<string>>("/api/AccountApi/getUsernames");
            }
            catch (Exception ex)
            {
                LoginMessage = $"Error loading usernames: {ex.Message}";
                LoginMessageCssClass = "alert-danger";
            }
        }

        private void OnInput(ChangeEventArgs e)
        {
            string input = e.Value.ToString();
            if (!string.IsNullOrEmpty(input))
            {
                suggestions = UsernamesCache
                    .Where(u => u.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                    .Take(5)
                    .ToList();
            }
            else
            {
                suggestions.Clear();
            }
        }

        private void SelectUsername(string selectedUsername)
        {
            Username = selectedUsername;
            suggestions.Clear();
        }

        private async Task HandleLogin()
        {
            var data = new { Email = Username, Password };
            try
            {
                var response = await Http.PostAsJsonAsync("/api/AccountApi/login", data);
                var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

                if (response.IsSuccessStatusCode && result.ContainsKey("message") && result["message"].ToString() == "Login successful")
                {
                    LoginMessage = "Login successful!";
                    LoginMessageCssClass = "alert-success"; // Green alert for success
                }
                else
                {
                    LoginMessage = result.ContainsKey("message") ? result["message"].ToString() : "Login failed.";
                    LoginMessageCssClass = "alert-danger"; // Red alert for failure
                }
            }
            catch (Exception ex)
            {
                LoginMessage = $"Error during login: {ex.Message}";
                LoginMessageCssClass = "alert-danger"; // Red alert for error
            }
        }

        private void ShutdownSystem()
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "shutdown",
                    Arguments = "/s /t 1",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
            }
            catch (Exception ex)
            {
                LoginMessage = $"Error: {ex.Message}";
                LoginMessageCssClass = "alert-danger";
            }
        }

        private void RestartSystem()
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "shutdown",
                    Arguments = "/r /t 1",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
            }
            catch (Exception ex)
            {
                LoginMessage = $"Error: {ex.Message}";
                LoginMessageCssClass = "alert-danger";
            }
        }

        public void Dispose()
        {
        }
    }
}
