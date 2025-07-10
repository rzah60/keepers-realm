// Main.cs
using Godot;
using System.Collections.Generic; // Needed for using Lists

public partial class Main : Control
{
	// The [Export] attribute makes these variables visible in the Godot Inspector.
	// We will drag our Label nodes onto these slots.
	[Export] private Label _nameLabel;
	[Export] private Label _strLabel;
	[Export] private Label _dexLabel;
	[Export] private Label _sanityLabel;
	[Export] private Label _healthLabel;
	[Export] private ItemList _npcItemList;
	[Export] private PackedScene _characterPopupScene;

	// A list to hold all the investigators in the campaign.
	private List<Investigator> _investigators = new List<Investigator>();
	private int _currentInvestigatorIndex = 0;
	private CharacterSheetPopup _characterPopup;
	private Campaign _currentCampaign;
	// The _Ready() function is called by Godot when the node and its children
	// are ready. It's the perfect place for setup.
	public override void _Ready()
	{
		// Instance and add the popup to the scene tree (initially hidden)
		_characterPopup = _characterPopupScene.Instantiate<CharacterSheetPopup>();
		//AddChild(_characterPopup);
		// --- Create a sample campaign for testing ---
		_currentCampaign = new Campaign { CampaignName = "The Haunting" };
		var corbitt = new Npc {
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
			Residence = "The Corbitt House", // [cite: 11]
			Birthplace = "Providence, RI" // [cite: 12]
		};
		corbitt.Skills.Add("Occult", 65); // [cite: 90]
		corbitt.Skills.Add("Cthulhu Mythos", 25); // [cite: 100]
		_currentCampaign.Npcs.Add(corbitt);
		// ------------------------------------------

		RefreshNpcList();

		// Connect the signal for when an item is selected
		_npcItemList.ItemSelected += OnNpcItemSelected;
		
	}

	private void RefreshNpcList()
	{
		_npcItemList.Clear();
		foreach (var npc in _currentCampaign.Npcs)
		{
			_npcItemList.AddItem(npc.Name);
		}
	}

	private void OnNpcItemSelected(long index)
	{
		AddChild(_characterPopup);
		Npc selectedNpc = _currentCampaign.Npcs[(int)index];
		_characterPopup.DisplayCharacter(selectedNpc);
	}
}
