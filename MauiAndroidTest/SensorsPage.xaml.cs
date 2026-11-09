namespace MauiAndroidTest;

public partial class SensorsPage : ContentPage
{
	public SensorsPage()
	{
		InitializeComponent();
	}

    private async void OpenCameraPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CameraPage));
    }

    private async void OpenThirdTwo(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ThirdTwoPage));
    }
    
    
    private async void OpenSettingsPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    private async void OnARClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ARPage());
    }


}