namespace MauiAndroidTest;

public partial class MainPage : ContentPage
{
	int count = 0;
    int entryCount = 0;

    public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object? sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}
	
	private void OnSubmitClicked(object sender, EventArgs e)
	{
		string userText = TextBox.Text;
		if (string.IsNullOrWhiteSpace(userText))
			return;

		var NewLabel = new Label
		{
			Text = $"{DateTime.Now:HH:mm} - {userText}",
			FontSize = 16,
			Margin = new Thickness(0, 5)
		};

        TextListContainer.Children.Add(NewLabel);

        entryCount++;
        EntryCountLabel.Text = $"Entries: {entryCount}";

        TextBox.Text = string.Empty;
	}

}
