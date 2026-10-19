namespace MauiAndroidTest;
using MauiAndroidTest.Services;
using Microsoft.Maui.Media;
public partial class CameraPage : ContentPage
{
	public CameraPage()
	{
		InitializeComponent();
	}

    private async void OpenCameraClicked(object sender, EventArgs e)
    {
        bool hasPermission =
            await PermissionService.EnsurePermissionAsync<Permissions.Camera>();

        if (!hasPermission)
            return;

        if (!MediaPicker.Default.IsCaptureSupported)
            return;

        FileResult? photo =
            await MediaPicker.Default.CapturePhotoAsync();

        if (photo == null)
            return;

        string localPath = Path.Combine(
            FileSystem.CacheDirectory,
            photo.FileName);

        await using (Stream sourceStream = await photo.OpenReadAsync())
        await using (FileStream localFileStream = File.OpenWrite(localPath))
        {
            await sourceStream.CopyToAsync(localFileStream);
        }

        CapturedImage.Source = ImageSource.FromFile(localPath);
    }
}
