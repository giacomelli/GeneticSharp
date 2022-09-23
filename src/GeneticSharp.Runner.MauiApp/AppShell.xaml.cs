namespace GeneticSharp.Runner.MauiApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(PropertyEditorPage), typeof(PropertyEditorPage));
        }

    }
}