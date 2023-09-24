namespace prjGuIA;
using PanCardView;
using CommunityToolkit;
using CommunityToolkit.Maui;
using prjGuIA.Views;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.UseCardsView()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Nexa-ExtraLight.ttf", "NexaLight");
                fonts.AddFont("Nexa-Heavy.ttf", "NexaHeavy");
            });
		return builder.Build();
	}
}
