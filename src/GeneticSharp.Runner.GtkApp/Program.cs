using Gtk;

namespace GeneticSharp.Runner.GtkApp
{
    public class MainClass
    {
        public static void Main()
        {
            Application.Init();

            MainWindow win = new MainWindow();
            win.Show();
            Application.Run();
        }
    }
}
