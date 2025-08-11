// MainMenu.cs
using Godot;

public partial class MainMenu : Control
{
	[Export] private Button _newCampaignButton;
	[Export] private Button _loadCampaignButton;

	public override void _Ready()
	{
		_newCampaignButton.Pressed += OnNewCampaignPressed;
		_loadCampaignButton.Pressed += OnLoadCampaignPressed;
	}

	private void OnNewCampaignPressed()
	{
		var dialog = new AcceptDialog { Title = "New Campaign" };
		var label = new Label { Text = "Enter Campaign Name:" };
		var lineEdit = new LineEdit { Name = "CampaignNameInput" };

		dialog.AddChild(label);
		dialog.AddChild(lineEdit);

		dialog.Confirmed += () => 
		{
			var campaignName = dialog.GetNode<LineEdit>("CampaignNameInput").Text;
			if (!string.IsNullOrEmpty(campaignName))
			{
				var newCampaign = new Campaign { CampaignName = campaignName };
				// Store the new campaign in our global manager's instance
				CampaignManager.Instance.CurrentCampaign = newCampaign;
				// Switch to the main tracker scene
				GetTree().ChangeSceneToFile("res://main.tscn");
			}
		};

		AddChild(dialog);
		dialog.PopupCentered();
	}
	
	// Add this method to MainMenu.cs
	private void OnLoadCampaignPressed()
	{
		var dialog = new FileDialog
		{
			Title = "Load Campaign",
			FileMode = FileDialog.FileModeEnum.OpenFile,
			Access = FileDialog.AccessEnum.Filesystem,
		};
		dialog.AddFilter("*.tres", "Campaign File");

		dialog.FileSelected += (string path) =>
		{
			var loadedCampaign = ResourceLoader.Load(path, "Campaign");
			if (loadedCampaign != null)
			{
				// Store the loaded campaign in our global manager's instance
				CampaignManager.Instance.CurrentCampaign = (Campaign)loadedCampaign;
				// Switch to the main tracker scene
				GetTree().ChangeSceneToFile("res://main.tscn");
			}
			else
			{
				GD.PrintErr($"Failed to load campaign file at: {path}");
			}
		};
		AddChild(dialog);
		dialog.PopupCentered();
	}
}
