using System;
using System.IO;

namespace a4label
{
    /// <summary>
    /// Reads and writes application settings to %PROGRAMDATA%\QRlabel\settings.ini
    /// </summary>
    internal static class AppSettings
    {
        public static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "QRlabel",
            "settings.ini");

        public static string Filename { get; set; } = string.Empty;
        public static string Printer { get; set; } = string.Empty;

        /// <summary>Reason the last Save failed, or empty if it succeeded.</summary>
        public static string LastError { get; private set; } = string.Empty;

        /// <summary>Loads settings from the INI file. Missing keys keep their default values.</summary>
        public static void Load()
        {
            if (!File.Exists(SettingsPath))
                return;

            foreach (string line in File.ReadAllLines(SettingsPath))
            {
                int eq = line.IndexOf('=');
                if (eq < 0) continue;

                string key = line.Substring(0, eq).Trim();
                string value = line.Substring(eq + 1).Trim();

                switch (key)
                {
                    case "filename": Filename = value; break;
                    case "printer": Printer = value; break;
                }
            }
        }

        /// <summary>
        /// Saves current settings to the INI file. Returns false, with the reason in
        /// LastError, if the file cannot be written. The usual cause is that it was created
        /// by another Windows user: by default a file under %PROGRAMDATA% can be read by
        /// every user but changed only by the user who created it.
        /// </summary>
        public static bool Save()
        {
            try
            {
                string dir = Path.GetDirectoryName(SettingsPath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllLines(SettingsPath, new[]
                {
                    "filename=" + Filename,
                    "printer="  + Printer
                });
                LastError = string.Empty;
                return true;
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                LastError = e.Message;
                return false;
            }
        }
    }
}