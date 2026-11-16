using System;

namespace MauiAndroidTest;

public partial class ARPage : ContentPage
{
    private string selectedObject = "Chair";

#if ANDROID
    private ARCoreMauiView? arView;
#endif

    public ARPage()
    {
        InitializeComponent();

#if ANDROID
        arView = new ARCoreMauiView();

        arView.NativeARView = null;

        ARContainer.Content = arView;
#endif
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

#if ANDROID
        arView?.NativeARView?.ResumeAR();
#endif
    }

    protected override void OnDisappearing()
    {
#if ANDROID
        arView?.NativeARView?.PauseAR();
#endif

        base.OnDisappearing();
    }

    private void ChairClicked(object sender, EventArgs e)
    {
        selectedObject = "Chair";
        SetSelectedObject();
    }

    private void TableClicked(object sender, EventArgs e)
    {
        selectedObject = "Table";
        SetSelectedObject();
    }

    private void LampClicked(object sender, EventArgs e)
    {
        selectedObject = "Lamp";
        SetSelectedObject();
    }

    private void PlaceClicked(object sender, EventArgs e)
    {
#if ANDROID
        arView?.NativeARView?.PlaceSelectedObject(selectedObject);
#else
        StatusLabel.Text = "ARCore on Android only.";
#endif
    }

    private void RemoveClicked(object sender, EventArgs e)
    {
#if ANDROID
        arView?.NativeARView?.RemoveSelectedObject();
#endif
    }

    private void SetSelectedObject()
    {
        StatusLabel.Text = $"Valitud: {selectedObject}";

#if ANDROID
        arView?.NativeARView?.SetSelectedObject(selectedObject);
#endif
    }
}