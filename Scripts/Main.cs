// Main.cs
using Godot;
using System.Collections.Generic;
using System.IO; // Needed for using Lists

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
	[Export] private Button _deleteInvestigatorButton;
	[Export] private Button _deleteNpcButton;
	[Export] private ConfirmationDialog _deleteConfirmDialog;
	[Export] private Button _hostSessionButton;
	[Export] private Label _sessionInfoLabel;

	private CharacterSheetPopup _characterPopup;
	private Campaign _currentCampaign;
	private CharacterForm _characterForm;
	private bool _isDirty = false;
	private Entity _entityToDelete;
	private NetworkManager _networkManager;

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

		// Get the singleton instance
	_networkManager = GetNode<NetworkManager>("/root/NetworkManager");

		_addInvestigatorButton.Pressed += OnAddInvestigatorButtonPressed;
		_characterForm.CharacterSaved += OnCharacterSaved;
		_addNpcButton.Pressed += OnAddNpcButtonPressed;
		_saveCampaignButton.Pressed += SaveCampaign;
		_returnToMenuButton.Pressed += OnReturnToMenuPressed;
		_characterPopup.VisibilityChanged += OnPopupVisibilityChanged;
		_investigatorItemList.ItemSelected += OnInvestigatorItemSelected;
		_npcItemList.ItemSelected += OnNpcItemSelected;
		_returnToMenuConfirmDialog.Confirmed += OnReturnToMenuConfirmed;
		_characterPopup.EditCharacterRequested += OnEditCharacterRequested;
		_deleteInvestigatorButton.Pressed += OnDeleteInvestigatorButtonPressed;
		_deleteNpcButton.Pressed += OnDeleteNpcButtonPressed;
		_deleteConfirmDialog.Confirmed += OnDeleteConfirmed;
		_hostSessionButton.Pressed += OnHostSessionButtonPressed;

		// --- Create a sample campaign for testing ---
		_currentCampaign = CampaignManager.Instance.CurrentCampaign;
		UpdateCampaignNameLabel();

		// We replace the direct call to PopulateLists() with this
		InitializeSceneData();

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
	private void OnDeleteInvestigatorButtonPressed()
	{
		var selectedItems = _investigatorItemList.GetSelectedItems();
		if (selectedItems.Length == 0) return; // Do nothing if nothing is selected

		int index = selectedItems[0];
		_entityToDelete = _currentCampaign.Investigators[index];
		_deleteConfirmDialog.PopupCentered();
	}

	private void OnDeleteNpcButtonPressed()
	{
		var selectedItems = _npcItemList.GetSelectedItems();
		if (selectedItems.Length == 0) return;

		int index = selectedItems[0];
		_entityToDelete = _currentCampaign.Npcs[index];
		_deleteConfirmDialog.PopupCentered();
	}

	private void OnHostSessionButtonPressed()
	{
		_networkManager.CreateServer();
		_hostSessionButton.Disabled = true;
		_hostSessionButton.Text = "Hosting...";

		string hostIp = "127.0.0.1";

		// Get the code from the NetworkManager and update the label text
		string sessionCode = _networkManager.SessionCode;
		_sessionInfoLabel.Text = $"URL: http://{hostIp}:8080  |  Code: {sessionCode}";
	}
	private void OnEditCharacterRequested(Entity character)
	{
		_characterForm.PopupToEdit(character);
	}

	private void OnCharacterSaved(Entity entityData)
	{
		// This logic handles both adding a new entity and updating an existing one.
		if (entityData is Investigator investigator)
		{
			if (!_currentCampaign.Investigators.Contains(investigator))
			{
				_currentCampaign.Investigators.Add(investigator);
			}
		}
		else if (entityData is Npc npc)
		{
			if (!_currentCampaign.Npcs.Contains(npc))
			{
				_currentCampaign.Npcs.Add(npc);
			}
		}

		// --- ADD THIS SECTION ---
		// If the saved character is an Investigator, push the update over the network.
		// We use the static Instance of NetworkManager to call the method.
		if (entityData is Investigator savedInvestigator)
		{
			NetworkManager.Instance.UpdatePlayerCharacter(savedInvestigator);
		}
		// --- END OF NEW SECTION ---

		RefreshNpcList();
		RefreshInvestigatorList();
		_isDirty = true;
		UpdateCampaignNameLabel();
	}
	private void OnDeleteConfirmed()
	{
		if (_entityToDelete == null) return;

		if (_entityToDelete is Investigator investigator)
		{
			_currentCampaign.Investigators.Remove(investigator);
		}
		else if (_entityToDelete is Npc npc)
		{
			_currentCampaign.Npcs.Remove(npc);
		}

		// Mark campaign as dirty and refresh the UI
		_isDirty = true;
		UpdateCampaignNameLabel();
		PopulateLists();

		// Clear the entity to delete to reset the state
		_entityToDelete = null;
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

	private void PopulateLists()
	{
		_investigatorItemList.Clear();
		_npcItemList.Clear();

		if (_currentCampaign.Investigators != null)
		{
			foreach (var investigator in _currentCampaign.Investigators)
			{
				_investigatorItemList.AddItem(investigator.Name);
			}
		}

		if (_currentCampaign.Npcs != null)
		{
			foreach (var npc in _currentCampaign.Npcs)
			{
				_npcItemList.AddItem(npc.Name);
			}
		}
	}

	private async void InitializeSceneData()
{
	// This command pauses execution of this method for one frame.
	// It ensures that the scene change is 100% complete before we proceed.
	await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

	// Now that we are certain everything is settled, we populate the lists.
	PopulateLists();
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
