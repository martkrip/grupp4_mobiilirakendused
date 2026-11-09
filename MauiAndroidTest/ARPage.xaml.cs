
namespace MauiAndroidTest;

public partial class ARPage : ContentPage
{
    private string selectedObject = "Chair";

    public ARPage()
    {
        InitializeComponent();
    }

    private void ChairClicked(object sender, EventArgs e)
    {
        selectedObject = "Chair";
        StatusLabel.Text = "Selected: Chair";
    }

    private void TableClicked(object sender, EventArgs e)
    {
        selectedObject = "Table";
        StatusLabel.Text = "Selected: Table";
    }

    private void LampClicked(object sender, EventArgs e)
    {
        selectedObject = "Lamp";
        StatusLabel.Text = "Selected: Lamp";
    }

    private void PlaceClicked(object sender, EventArgs e)
    {
        StatusLabel.Text = $"Selected object: {selectedObject}";
    }

    private void RemoveClicked(object sender, EventArgs e)
    {
        StatusLabel.Text = "Object removed";
    }
}
