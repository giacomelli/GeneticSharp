using GeneticSharp.Runner.MauiApp.ViewModels;

namespace GeneticSharp.Runner.MauiApp
{
    public static class MauiProgram
    {
        public static Microsoft.Maui.Hosting.MauiApp CreateMauiApp()
        {
            var builder = Microsoft.Maui.Hosting.MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services
                .AddSingleton<MainPage>()
                .AddSingleton<MainViewModel>()
                .AddTransient<PropertyEditorPage>()
                .AddTransient<PropertyEditorViewModel>();
            return builder.Build();
        }
    }
}