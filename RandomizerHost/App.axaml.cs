using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RandomizerHost.ViewModels;
using RandomizerHost.Views;
using Avalonia.Themes.Default;
using System.Net.Http;
using System.Threading.Tasks;

namespace RandomizerHost
{
    public class App : Application
    {
    
    public async Task<string> CheckForUpdatesAsync(string updateUrl)
    {
      using var client = new HttpClient();
      var response = await client.GetStringAsync(updateUrl);
      return response; // Example: JSON with version info
    }

    public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
