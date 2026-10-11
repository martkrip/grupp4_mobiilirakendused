namespace MauiAndroidTest;

public partial class ThirdPage : ContentPage
{
	public ThirdPage()
	{
		InitializeComponent();
	}

    private async void OpenThirdOne(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ThirdOnePage));
    }

    private async void OpenThirdTwo(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ThirdTwoPage));
    }

}