namespace MauiAndroidTest;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        //Navbar navigatsioon
        Routing.RegisterRoute(nameof(ThirdOnePage), typeof(ThirdOnePage));
        Routing.RegisterRoute(nameof(ThirdTwoPage), typeof(ThirdTwoPage));
    }

    protected override async void OnNavigated(ShellNavigatedEventArgs args) //Kui navigeerida 3.1 ja alumiselt navbarilt liigutakse uuesti lehtede vahel, tuleks tagasi esimese taseme leht mitte teise taseme leht (3.1 v 3.2 asemel 3)
    {
        base.OnNavigated(args);

        if (args.Source == ShellNavigationSource.ShellSectionChanged)
        {
            await CurrentItem.CurrentItem.Navigation.PopToRootAsync(false);
        }
    }
}
