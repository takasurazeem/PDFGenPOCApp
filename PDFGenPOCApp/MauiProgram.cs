using DrawnUi.Maui.Draw;
using Microsoft.Extensions.Logging;

namespace PDFGenPOCApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("pdms-saleem-quranfont.ttf", "PDMS-Saleem");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.UseDrawnUi();

            return builder.Build();
        }
    }
}
