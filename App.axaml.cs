using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace BatteryDischarger
{
    // Connects Avalonia's loaded application resources to the desktop window lifetime.
    public partial class App : Application
    {
        // Loads the application-level Avalonia resources declared in App.axaml.
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        // Creates the main window only for the desktop lifetime supported by this application.
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
