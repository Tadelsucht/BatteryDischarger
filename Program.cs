using Avalonia;
using BatteryDischarger.Miscellaneous;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BatteryDischarger
{
    public class Program
    {
        public static bool Autostart = false;
        private const string CMDParameterAutostart = "Autostart";
        private const string CMDParameterLanguage = "Language";
        private const string CMDParameterAccelerateBatteryDischarge = "AccelerateBatteryDischarge";
        private const string CMDParameterTargetBatteryChargeInPercent = "TargetBatteryChargeInPercent";
        private const string CMDParameterPreventUnwantedSystemSleepMode = "PreventUnwantedSystemSleepMode";

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
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

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();

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