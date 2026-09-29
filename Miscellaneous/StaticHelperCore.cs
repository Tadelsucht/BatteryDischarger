using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace BatteryDischarger.Miscellaneous
{
    // Shared path, filesystem-probe, and error-dialog helpers used by the desktop application.
    public static class StaticHelperCore
    {
        // Application-specific folder name used when the executable directory is not writable.
        public const string AppDataSubFolder = "BatteryDischarger";
        // Caches the selected writable location for this process after its first lookup.
        private static string workingDirectoryPath;

        // Returns the directory containing the launched application assembly.
        public static string CurrentApplicationDirectory
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        // Returns the current user's ApplicationData directory without an app-specific suffix.
        public static string ApplicationDirectory
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }
        }

        // Returns the version embedded in the executing assembly metadata.
        public static string Version
        { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        // Chooses a writable configuration directory, preferring the app directory before the app-specific data folder.
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
        // Probes write access by creating a uniquely named file that is deleted when the stream closes.
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

        // Runs a UI action and reports its exception through the standard asynchronous error dialog.
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

        // Builds the localized standard error dialog for a caught exception.
        [ExcludeFromCodeCoverage]
        public static IMsBox<ButtonResult> GetErrorMessageBox(Exception ex)
        {
            return MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard(Properties.Resources.Error, ex.Message, icon: MsBox.Avalonia.Enums.Icon.Error);
        }

        // Suppresses all action failures; call only where losing the error is acceptable to the caller.
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
