using BatteryDischarger.PlatformSpecificActions;
using IniParser;
using IniParser.Model;
using System;
using System.Globalization;
using System.IO;

namespace BatteryDischarger.Miscellaneous
{
    // Loads UI preferences from an INI file and persists each property update immediately.
    public class IniConfiguration
    {
        // Lazily shared application configuration; path-based instances remain available to isolated callers.
        protected static IniConfiguration instance;
        // Fixed at construction so later writes always target the same configuration file.
        private readonly string configurationFilePath;
        // Holds parsed values, including defaults supplied by getters when entries are absent or malformed.
        private IniData data;
        // Serializes writes through the parser instance used for this configuration file.
        private FileIniDataParser parser;

        // Uses the application's selected writable directory for the normal singleton instance.
        protected IniConfiguration() : this(Path.Combine(StaticHelperCore.WorkingDirectoryPath, "Configuration.ini"))
        {
        }

        // Loads the requested file when present and starts with an empty INI model otherwise.
        public IniConfiguration(string configurationFilePath)
        {
            this.configurationFilePath = configurationFilePath ?? throw new ArgumentNullException(nameof(configurationFilePath));
            parser = new FileIniDataParser();
            if (File.Exists(configurationFilePath))
            {
                data = parser.ReadFile(configurationFilePath);
            }
            else
            {
                data = new IniData();
            }
        }

        // Creates one process-wide configuration object on first access.
        public static IniConfiguration Instance
        {
            get
            {
                if (instance is null) instance = new IniConfiguration();
                return instance;
            }
        }

        // Persists the UI language code; missing or unreadable values fall back to the current culture.
        public string Language
        {
            get
            {
                try
                {
                    return data["UI"]["Language"] ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                }
                catch
                {
                    return CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                }
            }
            set
            {
                data["UI"]["Language"] = value;
                Save();
            }
        }

        // Stores a battery percentage target; missing or malformed values use the established 30 percent default.
        public int TargetBatteryChargeInPercent
        {
            get
            {
                try
                {
                    if (int.TryParse(data["UI"]["TargetBatteryChargeInPercent"], out int result))
                    {
                        return result;
                    }
                    else
                    {
                        return 30;
                    }
                }
                catch
                {
                    return 30;
                }
            }
            set
            {
                data["UI"]["TargetBatteryChargeInPercent"] = value.ToString();
                Save();
            }
        }

        // Stores whether optional CPU-load workers are enabled; legacy or missing values default to true.
        public bool AccelerateBatteryDischarge
        {
            get
            {
                try
                {
                    return bool.Parse(data["UI"]["AccelerateBatteryDischarge"]);
                }
                catch
                {
                    return true;
                }
            }
            set
            {
                data["UI"]["AccelerateBatteryDischarge"] = value.ToString();
                Save();
            }
        }

        // Stores the requested sleep-prevention preference; missing or malformed values default to true.
        public bool PreventUnwantedSystemSleepMode
        {
            get
            {
                try
                {
                    return bool.Parse(data["UI"]["PreventUnwantedSystemSleepMode"]);
                }
                catch
                {
                    return true;
                }
            }
            set
            {
                data["UI"]["PreventUnwantedSystemSleepMode"] = value.ToString();
                Save();
            }
        }

        // Stores the enum name for compatibility with existing INI files and defaults to shutdown on parse failure.
        public EndActionEnum EndAction
        {
            get
            {
                try
                {
                    return Enum.Parse<EndActionEnum>(data["UI"]["EndAction"]);
                }
                catch
                {
                    return EndActionEnum.Shutdown;
                }
            }
            set
            {
                data["UI"]["EndAction"] = value.ToString();
                Save();
            }
        }

        // Serializes file writes and reports persistence failures to the console without changing the in-memory value.
        protected void Save()
        {
            lock (parser)
            {
                try
                {
                    parser.WriteFile(configurationFilePath, data);
                    GC.Collect();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
