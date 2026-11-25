using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Hosting;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace a4label
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// An optional argument names the layout file to open, e.g. from a desktop shortcut:
        ///     QRLabel.exe NV4.txt
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(StartupLayoutFile(args)));
        }

        /// <summary>
        /// The layout file named when the program was started, or null if none was.
        /// </summary>
        internal static string StartupLayoutFile(string[] args)
        {
            if (args != null && args.Length > 0 && args[0].Trim() != "")
                return args[0].Trim();

            // Installed with ClickOnce the program is started through an .appref-ms shortcut,
            // and an argument given to that shortcut arrives as activation data instead.
            ActivationArguments activation = AppDomain.CurrentDomain.SetupInformation.ActivationArguments;
            if (activation == null || activation.ActivationData == null || activation.ActivationData.Length == 0)
                return null;
            return LayoutFileFromActivation(activation.ActivationData[0]);
        }

        /// <summary>
        /// Picks a layout file name out of ClickOnce activation data, which may instead be the
        /// address the program was installed or started from.
        /// </summary>
        internal static string LayoutFileFromActivation(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return null;
            data = data.Trim();

            Uri uri;
            if (Uri.TryCreate(data, UriKind.Absolute, out uri))
            {
                if (!uri.IsFile)
                    return null;            // a web address, not a file
                data = uri.LocalPath;
            }

            // Started from its own deployment manifest rather than with a layout file
            if (data.EndsWith(".application", StringComparison.OrdinalIgnoreCase))
                return null;
            return data;
        }
    }
}
