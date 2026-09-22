using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace BatteryDischarger.Miscellaneous
{
    public static class StaticHelperCore
    {
        public const string AppDataSubFolder = "BatteryDischarger";
        private static string workingDirectoryPath;

        /// <summary>
        /// Read only
        /// </summary>
        public static string CurrentApplicationDirectory
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public static string ApplicationDirectory
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }
        }

        public static string Version
        { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        /// <summary>
        /// WindowsApplicationDirectory
        /// Read & Write
        /// </summary>
        [ExcludeFromCodeCoverage]
        public static string WorkingDirectoryPath
        {
            get
            {
                if (workingDirectoryPath is null)
                {
                    if (IsDirectoryWritable(CurrentApplicationDirectory))
                    {
                        workingDirectoryPath = CurrentApplicationDirectory;
                    }
                    else
                    {
                        if (IsDirectoryWritable(ApplicationDirectory))
                        {
                            workingDirectoryPath = Path.Combine(ApplicationDirectory, AppDataSubFolder);
                            Directory.CreateDirectory(workingDirectoryPath);
                        }
                        else
                        {
                            throw new IOException(Properties.Resources.DidNotFoundAUseableWorkingDirectoryPath);
                        }
                    }
                }
                return workingDirectoryPath;
            }
        }

        // https://stackoverflow.com/a/6371533/4172756
        public static bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
        {
            try
            {
                using (FileStream fs = File.Create(Path.Combine(dirPath, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose)) { }
                return true;
            }
            catch
            {
                if (throwIfFails)
                    throw;
                else
                    return false;
            }
        }

        [ExcludeFromCodeCoverage]
        public static void TryCatchShowErrorMessageBox(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                _ = GetErrorMessageBox(ex).ShowWindowAsync();
            }
        }

        [ExcludeFromCodeCoverage]
        public static IMsBox<ButtonResult> GetErrorMessageBox(Exception ex)
        {
            return MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard(Properties.Resources.Error, ex.Message, icon: MsBox.Avalonia.Enums.Icon.Error);
        }

        public static void TryCatchIgnore(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                /* Ignore */
            }
        }
    }
}
