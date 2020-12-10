using System;
using System.Windows.Forms;

namespace Hashing
{
    static class Program
    {
        public static Init init;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            init = new Init();
            Application.Run(init);
        }
    }
}
