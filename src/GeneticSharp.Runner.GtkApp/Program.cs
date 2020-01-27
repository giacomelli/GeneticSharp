using Gtk;
using System;

namespace GeneticSharp.Runner.GtkApp
{
    public class MainClass
    {
        public static void Main(string[] args)
        {
            try
            {
                Application.Init();
            }
            catch(DllNotFoundException)
            {
                // If you are here, see this: https://github.com/giacomelli/GeneticSharp/wiki/setup#gtk-app.
                throw;
            }

            MainWindow win = new MainWindow();
            win.Show();
            Application.Run();
        }
    }
}
