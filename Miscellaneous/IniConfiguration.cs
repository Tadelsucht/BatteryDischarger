using BatteryDischarger.PlatformSpecificActions;
using IniParser;
using IniParser.Model;
using System;
using System.Globalization;
using System.IO;

namespace BatteryDischarger.Miscellaneous
{
    public class IniConfiguration
    {
        protected static IniConfiguration instance;
        private string configurationFilePath = Path.Combine(StaticHelperCore.WorkingDirectoryPath, "Configuration.ini");
        private IniData data;
        private FileIniDataParser parser;

        protected IniConfiguration()
        {
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

        public static IniConfiguration Instance
        {
            get
            {
                if (instance is null) instance = new IniConfiguration();
                return instance;
            }
        }

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