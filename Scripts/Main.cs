// Main.cs
using Godot;
using System.Collections.Generic; // Needed for using Lists

public partial class Main : Control
{
	// The [Export] attribute makes these variables visible in the Godot Inspector.
	// We will drag our Label nodes onto these slots.
	[Export] private ItemList _investigatorItemList;
	[Export] private ItemList _npcItemList;
	[Export] private PackedScene _characterPopupScene;
	[Export] private PackedScene _characterFormScene;
	[Export] private Button _addInvestigatorButton;
	[Export] private Button _addNpcButton;
	[Export] private Label _campaignNameLabel;
	[Export] private Button _saveCampaignButton;
	[Export] private Button _returnToMenuButton;
	[Export] private ConfirmationDialog _returnToMenuConfirmDialog;

	private CharacterSheetPopup _characterPopup;
	private Campaign _currentCampaign;
	private CharacterForm _characterForm;
	private bool _isDirty = false;
	// The _Ready() function is called by Godot when the node and its children
	// are ready. It's the perfect place for setup.
	public override void _Ready()
	{
		// Instance and add the popup to the scene tree (initially hidden)
		_characterPopup = _characterPopupScene.Instantiate<CharacterSheetPopup>();
		AddChild(_characterPopup);
		_characterPopup.Hide();

		// Instance the form and add it to the scene.
		_characterForm = _characterFormScene.Instantiate<CharacterForm>();
		AddChild(_characterForm);
		_characterForm.Hide();

		_addInvestigatorButton.Pressed += OnAddInvestigatorButtonPressed;
		_characterForm.CharacterSaved += OnCharacterFormSaved;
		_addNpcButton.Pressed += OnAddNpcButtonPressed;
		_saveCampaignButton.Pressed += SaveCampaign;
		_returnToMenuButton.Pressed += OnReturnToMenuPressed;
		_characterPopup.VisibilityChanged += OnPopupVisibilityChanged;
		_investigatorItemList.ItemSelected += OnInvestigatorItemSelected;
		_npcItemList.ItemSelected += OnNpcItemSelected;
		_returnToMenuConfirmDialog.Confirmed += OnReturnToMenuConfirmed;


		// --- Create a sample campaign for testing ---
		_currentCampaign = CampaignManager.Instance.CurrentCampaign;

		// It's good practice to handle the case where the scene is run directly
		if (_currentCampaign == null)
		{
			GD.PrintErr("No campaign loaded! Returning to main menu.");
			GetTree().ChangeSceneToFile("res://main_menu.tscn");
			return; // Stop processing _Ready()
		}

		var corbitt = new Npc
		{
			Name = "Walter Corbitt", // [cite: 3]
			Occupation = "Landlord", // [cite: 6]
			Strength = 12, // 
			Dexterity = 10, // 
			Constitution = 11, // 
			Power = 15, // 
			HitPointsMax = 11, // [cite: 22]
			HitPointsCurrent = 11, // [cite: 22]
			SanityMax = 75, // [cite: 9]
			SanityCurrent = 0, // [cite: 9]
			Notes = "He's a Guy", // [cite: 11]
		};
		corbitt.Skills.Add("Occult", 65); // [cite: 90]
		corbitt.Skills.Add("Cthulhu Mythos", 25); // [cite: 100]
		_currentCampaign.Npcs.Add(corbitt);
		// ------------------------------------------

		// Refresh UI with the loaded campaign's data
		UpdateCampaignNameLabel();
		RefreshNpcList();
		RefreshInvestigatorList();


	}

	private void RefreshInvestigatorList()
	{
		_investigatorItemList.Clear();
		foreach (var investigator in _currentCampaign.Investigators)
		{
			_investigatorItemList.AddItem(investigator.Name);
		}
	}

	private void RefreshNpcList()
	{
		_npcItemList.Clear();
		foreach (var npc in _currentCampaign.Npcs)
		{
			_npcItemList.AddItem(npc.Name);
		}
	}

	private void OnInvestigatorItemSelected(long index)
	{

		Investigator selectedInvestigator = _currentCampaign.Investigators[(int)index];
		_characterPopup.DisplayCharacter(selectedInvestigator);
	}
	private void OnNpcItemSelected(long index)
	{
		Npc selectedNpc = _currentCampaign.Npcs[(int)index];
		_characterPopup.DisplayCharacter(selectedNpc);
	}
	private void OnAddInvestigatorButtonPressed()
	{
		_characterForm.CreateNewCharacter(Constants.INV_ID);
	}
	private void OnAddNpcButtonPressed()
	{
		_characterForm.CreateNewCharacter(Constants.NPC_ID);
	}

	private void OnCharacterFormSaved(Entity entityData)
	{
		_isDirty = true; // Mark that we have unsaved changes
		UpdateCampaignNameLabel(); // Update the UI to show the unsaved state
		switch (entityData.EntityId)
		{
			case Constants.INV_ID:
				_currentCampaign.Investigators.Add((Investigator)entityData);
				RefreshInvestigatorList();
				break;
			case Constants.NPC_ID:
				_currentCampaign.Npcs.Add((Npc)entityData);
				RefreshNpcList();
				break;
		}
	}

	private void UpdateCampaignNameLabel()
	{
		var campaignName = CampaignManager.Instance.CurrentCampaign.CampaignName;
		if (_isDirty)
		{
			_campaignNameLabel.Text = $"Campaign: {campaignName}*";
		}
		else
		{
			_campaignNameLabel.Text = $"Campaign: {campaignName}";
		}
	}

	private void OnPopupVisibilityChanged()
	{
		// This method runs when the popup is shown AND when it is hidden.
		// We only care about when it becomes hidden.
		if (!_characterPopup.Visible)
		{
			// Deselect all items in the list, resetting it for the next click.
			_npcItemList.DeselectAll();
		}
	}
	private void SaveCampaign()
	{
		// We'll hardcode the path for now.
		// Later, this will come from a save/load dialog.

		var path = $"res://{_currentCampaign.CampaignName.Replace(" ", "_")}.tres";

		Error err = ResourceSaver.Save(_currentCampaign, path);
		if (err != Error.Ok)
		{
			GD.PrintErr($"Failed to save campaign to {path}.");
		}
		else
		{
			GD.Print($"Campaign saved to {path}");

			_isDirty = false; // The campaign is now saved and clean
			UpdateCampaignNameLabel(); // Update the label to remove the asterisk
		}

	}
	private void OnReturnToMenuPressed()
	{
		if (_isDirty)
		{
			// If there are unsaved changes, show the confirmation dialog
			_returnToMenuConfirmDialog.PopupCentered();
		}
		else
		{
			// Otherwise, just go back to the menu without any fuss
			GetTree().ChangeSceneToFile(Constants.MainMenuScene);
		}
	}

	private void OnReturnToMenuConfirmed()
	{
		// This method is called when the "Return to Menu" button on the dialog is clicked.
		GetTree().ChangeSceneToFile(Constants.MainMenuScene);
	}

	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
		{
			if (_isDirty)
			{
				_returnToMenuConfirmDialog.PopupCentered();
			}
			else
			{
				GetTree().Quit();
			}
		}
	}
}
