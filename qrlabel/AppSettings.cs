using System;
using System.IO;

namespace a4label
{
    /// <summary>
    /// Reads and writes application settings to %PROGRAMDATA%\QRlabel\settings.ini
    /// </summary>
    internal static class AppSettings
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "QRlabel",
            "settings.ini");

        public static string Filename { get; set; } = string.Empty;
        public static string Printer { get; set; } = string.Empty;

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

        /// <summary>Saves current settings to the INI file.</summary>
        public static void Save()
        {
            string dir = Path.GetDirectoryName(SettingsPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllLines(SettingsPath, new[]
            {
                "filename=" + Filename,
                "printer="  + Printer
            });
        }
    }
}