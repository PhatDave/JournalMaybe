using System;
using System.Windows.Forms;

namespace JournalMaybe {
    static class Program {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            //int[] test = new int[10];
            //test[12] = 13;
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
