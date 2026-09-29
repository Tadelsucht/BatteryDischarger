using Avalonia;
using BatteryDischarger.Miscellaneous;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace BatteryDischarger
{
    // Owns process startup and applies supported command-line overrides to application configuration.
    public class Program
    {
        // Set by the command line so the window can enter controlled discharge after startup.
        public static bool Autostart = false;

        // These names are a compatibility contract for existing launch scripts and saved shortcuts.
        private const string CMDParameterAutostart = "Autostart";
        private const string CMDParameterLanguage = "Language";
        private const string CMDParameterAccelerateBatteryDischarge = "AccelerateBatteryDischarge";
        private const string CMDParameterTargetBatteryChargeInPercent = "TargetBatteryChargeInPercent";
        private const string CMDParameterPreventUnwantedSystemSleepMode = "PreventUnwantedSystemSleepMode";

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        // Applies saved settings and command-line overrides before opening the desktop lifetime.
        [STAThread]
        [ExcludeFromCodeCoverage]
        public static void Main(string[] args)
        {
            // Set working directory for lib loading
            // https://stackoverflow.com/a/27850458/4172756
            // https://stackoverflow.com/a/6719304/4172756
            try
            {
                string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Directory.SetCurrentDirectory(exeDir);
            }
            catch { /* Ignore */ }

            // Language
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(IniConfiguration.Instance.Language);
            IniConfiguration.Instance.Language = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

            // Parameter
            if (TryGetParameterData(args, CMDParameterLanguage, out string language))
                IniConfiguration.Instance.Language = language;
            if (TryGetParameterData(args, CMDParameterAccelerateBatteryDischarge, out string accelerateBatteryDischarge))
                IniConfiguration.Instance.AccelerateBatteryDischarge = bool.Parse(accelerateBatteryDischarge);
            if (TryGetParameterData(args, CMDParameterPreventUnwantedSystemSleepMode, out string preventUnwantedSystemSleepMode))
                IniConfiguration.Instance.PreventUnwantedSystemSleepMode = bool.Parse(preventUnwantedSystemSleepMode);
            if (TryGetParameterData(args, CMDParameterTargetBatteryChargeInPercent, out string targetBatteryChargeInPercent))
                IniConfiguration.Instance.TargetBatteryChargeInPercent = int.Parse(targetBatteryChargeInPercent);
            if (args.ToList().Contains(CMDParameterAutostart))
                Autostart = true;

            // GUI
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        // Provides the platform-detected builder shared by application startup and Avalonia tooling.
        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();

        // Reads the token immediately after the first matching option; a missing option or value returns false.
        public static bool TryGetParameterData(string[] args, string parameter, out string data)
        {
            data = null;
            if (args.ToList().Contains(parameter))
            {
                var indexOfParameter = args.ToList().IndexOf(parameter);
                if (args.Length > indexOfParameter + 1)
                {
                    data = args[indexOfParameter + 1];
                    return true;
                }
            }
            return false;
        }
    }
}
