namespace MauiAndroidTest;

public partial class SettingsPage : ContentPage
{
    private const string NameKey = "user_name";
    private const string DarkKey = "dark_mode";
    private const string FontKey = "font_size";

    private string NotePath => Path.Combine(FileSystem.AppDataDirectory, "note.txt");

    private bool _loading;

    public SettingsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _loading = true;

        NameEntry.Text = Preferences.Default.Get(NameKey, string.Empty);
        DarkSwitch.IsToggled = Preferences.Default.Get(DarkKey, false);
        FontSlider.Value = Preferences.Default.Get(FontKey, 16.0);
        ApplyTheme(DarkSwitch.IsToggled);
        ApplyFont(FontSlider.Value);

        if (File.Exists(NotePath))
            NoteEditor.Text = await File.ReadAllTextAsync(NotePath);

        var photoPath = GetLatestPhotoPath();
        if (photoPath != null)
            PhotoImage.Source = ImageSource.FromFile(photoPath);

        _loading = false;
    }
    
    private void OnSaveSettingsClicked(object? sender, EventArgs e)
    {
        Preferences.Default.Set(NameKey, NameEntry.Text ?? string.Empty);
        Preferences.Default.Set(DarkKey, DarkSwitch.IsToggled);
        Preferences.Default.Set(FontKey, FontSlider.Value);
        StatusLabel.Text = "Sätted salvestatud.";
    }

    private void OnDarkToggled(object? sender, ToggledEventArgs e)
    {
        if (_loading) return;
        ApplyTheme(e.Value);
    }

    private void OnFontChanged(object? sender, ValueChangedEventArgs e)
    {
        if (_loading) return;
        ApplyFont(e.NewValue);
    }

    private static void ApplyTheme(bool dark)
    {
        if (Application.Current != null)
            Application.Current.UserAppTheme = dark ? AppTheme.Dark : AppTheme.Light;
    }

    private void ApplyFont(double size)
    {
        var rounded = Math.Round(size);
        FontLabel.Text = $"Teksti suurus: {rounded}";
        NoteEditor.FontSize = rounded;
    }
    
    private async void OnSaveNoteClicked(object? sender, EventArgs e)
    {
        try
        {
            await File.WriteAllTextAsync(NotePath, NoteEditor.Text ?? string.Empty);
            StatusLabel.Text = $"Fail salvestatud: {NotePath}";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Viga: {ex.Message}";
        }
    }

    private async void OnShareNoteClicked(object? sender, EventArgs e)
    {
        if (!File.Exists(NotePath))
        {
            StatusLabel.Text = "Salvesta märge kõigepealt.";
            return;
        }

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Märkmed",
            File = new ShareFile(NotePath)
        });
    }
    
    private async void OnTakePhotoClicked(object? sender, EventArgs e)
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                StatusLabel.Text = "Kaamera pole sellel seadmel toetatud.";
                return;
            }

            var status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                StatusLabel.Text = "Kaamera luba puudub.";
                return;
            }

            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo == null) return; // kasutaja tühistas

            var path = Path.Combine(FileSystem.AppDataDirectory,
                $"photo_{DateTime.Now:yyyyMMdd_HHmmss}.jpg");

            using var source = await photo.OpenReadAsync();
            using var target = File.Create(path);
            await source.CopyToAsync(target);

            PhotoImage.Source = ImageSource.FromFile(path);
            StatusLabel.Text = $"Foto salvestatud: {path}";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Viga: {ex.Message}";
        }
    }

    private async void OnSharePhotoClicked(object? sender, EventArgs e)
    {
        var path = GetLatestPhotoPath();
        if (path == null)
        {
            StatusLabel.Text = "Tee kõigepealt foto.";
            return;
        }

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Foto",
            File = new ShareFile(path)
        });
    }

    private static string? GetLatestPhotoPath()
    {
        return Directory.GetFiles(FileSystem.AppDataDirectory, "photo_*.jpg")
            .OrderByDescending(f => f)
            .FirstOrDefault();
    }
}
