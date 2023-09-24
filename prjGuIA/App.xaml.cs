using prjGuIA.Views;
namespace prjGuIA;

public partial class App : Application
{
    public App()
	{
		InitializeComponent();
        //MainPage = new NavigationPage(new PerfilInicial());
        var nav = new NavigationPage(new PerfilInicial());
        nav.BarBackground = Color.FromHex("#101216");
        MainPage = nav;
    }
}
