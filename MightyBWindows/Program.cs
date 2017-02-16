using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MightyB.ScreenSaver
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.        
        /// /p - Show the screensaver in the screensaver selection dialog box
        /// /c - Show the screensaver configuration dialog box
        /// /s - Show the screensaver full-screen 
        /// </summary>
        [STAThread]
        internal static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new ScreenSaverForm());     

            if (args.Length == 0)
                return;
            
            var firstArgument = args[0].ToLower().Trim();
            var secondArgument = string.Empty;

            // Handle cases where arguments are separated by colon.
            // Examples: /c:1234567 or /P:1234567
            if (firstArgument.Length > 2)
            {
                secondArgument = firstArgument.Substring(3).Trim();
                firstArgument = firstArgument.Substring(0, 2);
            }
            else if (args.Length > 1)
                secondArgument = args[1];

            switch (firstArgument)
            {
                case "/c": // Configuration mode

                    break;
                case "/p": // Preview mode
                    var previewWndHandle = new IntPtr(long.Parse(secondArgument));
                    Application.Run(new ScreenSaverForm(previewWndHandle));
                    break;

                case "/s": // Full-screen mode
                    ShowScreenSaver();
                    Application.Run();
                    break;
                default:
                    Application.Exit();
                    break;
            }

        }

        internal static void ShowScreenSaver()        
        {
#if DEBUG
            var screen = Screen.AllScreens[0];
            var saver = new ScreenSaverForm { Bounds = screen.Bounds, RandomSeed = 1 };
            saver.Show();
#else
            var i = 0;
            foreach (var screensaver in Screen.AllScreens.Select(screen => new ScreenSaverForm { Bounds = screen.Bounds, RandomSeed = ++i }))
            {
                screensaver.Show();
            }
#endif
        }
    }
}
