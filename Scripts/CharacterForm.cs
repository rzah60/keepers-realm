// CharacterForm.cs
using System.Collections.Generic;
using Godot;

public partial class CharacterForm : Window
{
	// Define a signal that will be emitted when the user saves.
	// It will carry the completed Npc data.
	[Signal]
	public delegate void CharacterSavedEventHandler(Entity entityData);

	// Export all the input nodes (LineEdit, SpinBox, etc.)
	[Export] private LineEdit _nameInput;
	[Export] private LineEdit _occupationInput;
	[Export] private HBoxContainer _playerHBox;
	[Export] private LineEdit _playerInput;
	[Export] private HBoxContainer _affiliationsHBox;
	[Export] private LineEdit _affiliationsInput;
	[Export] private SpinBox _ageInput;
	[Export] private SpinBox _strInput;
	[Export] private SpinBox _dexInput;
	[Export] private SpinBox _conInput;
	[Export] private SpinBox _appInput;
	[Export] private SpinBox _intInput;
	[Export] private SpinBox _sizInput;
	[Export] private SpinBox _powInput;
	[Export] private SpinBox _eduInput;
	[Export] private SpinBox _mhpInput;
	[Export] private SpinBox _mmpInput;
	[Export] private SpinBox _sanInput;
	[Export] private SpinBox _lckInput;
	[Export] private TextEdit _sklInput;
	[Export] private TextEdit _notesInput;
	// ... Export ALL your other input nodes here ...

	[Export] private Button _saveButton;
	[Export] private Button _cancelButton;

	private int _entityType;

	private Entity _editingEntity; // To hold the character being edited.

	public override void _Ready()
	{
		// Connect the button signals to methods within this script.
		_saveButton.Pressed += OnSaveButtonPressed;
		_cancelButton.Pressed += OnCancelButtonPressed;
		this.CloseRequested += OnCancelButtonPressed; // Also cancel on 'X'
	}

	// This method collects all the data from the form fields.
	private void OnSaveButtonPressed()
	{
		Entity entityToSave;

		// If _editingEntity is NOT null, we are in edit mode.
		// We use the existing entity as our target.
		if (_editingEntity != null)
		{
			entityToSave = _editingEntity;
		}
		// Otherwise, we are in creation mode.
		// We create a new instance based on the selected type.
		else
		{
			entityToSave = _entityType == Constants.INV_ID ? new Investigator() : new Npc();
		}
		
		// --- Update all properties from the form fields ---
		entityToSave.Name = _nameInput.Text;
		entityToSave.Occupation = _occupationInput.Text;
		entityToSave.Age = (int)_ageInput.Value;
		entityToSave.Notes = _notesInput.Text;

		entityToSave.Strength = (int)_strInput.Value;
		entityToSave.Dexterity = (int)_dexInput.Value;
		entityToSave.Constitution = (int)_conInput.Value;
		entityToSave.Appearance = (int)_appInput.Value;
		entityToSave.Size = (int)_sizInput.Value;
		entityToSave.Power = (int)_powInput.Value;
		entityToSave.Intelligence = (int)_intInput.Value;
		entityToSave.Education = (int)_eduInput.Value;

		entityToSave.HitPointsMax = (int)_mhpInput.Value;
		entityToSave.MagicPointsMax = (int)_mmpInput.Value;
		entityToSave.SanityMax = (int)_sanInput.Value;
		entityToSave.LuckMax = (int)_lckInput.Value;
		
		// Set current stats to max by default when creating/editing
		entityToSave.HitPointsCurrent = entityToSave.HitPointsMax;
		entityToSave.MagicPointsCurrent = entityToSave.MagicPointsMax;
		entityToSave.SanityCurrent = entityToSave.SanityMax;
		entityToSave.LuckCurrent = entityToSave.LuckMax;

		entityToSave.Skills = ParseSkills(_sklInput.Text);

		if (entityToSave is Investigator investigator)
		{
			investigator.Player = _playerInput.Text;
		}
		else if (entityToSave is Npc npc)
		{
			npc.Affiliations = _affiliationsInput.Text;
		}

		// Emit the signal with the created OR updated entity
		EmitSignal(SignalName.CharacterSaved, entityToSave);
		
		// Clear the editing entity to reset the form's state
		_editingEntity = null;
		Hide();
	}

	private Godot.Collections.Dictionary<string, int> ParseSkills(string textEditStr)
	{
		string[] skillValueStrs = textEditStr.Split(',');
		Godot.Collections.Dictionary<string, int> skillValuePairs = new Godot.Collections.Dictionary<string, int>();

		foreach (string pair in skillValueStrs)
		{
			string[] skillVal = pair.Split(':');
			skillValuePairs.Add(skillVal[0].TrimEnd().TrimStart(), skillVal[1].Trim().ToInt());
		}

		return skillValuePairs;
	}


	private void OnCancelButtonPressed()
	{
		_playerHBox.Visible = false;
		_affiliationsHBox.Visible = false;
		Hide(); // Just close the form without saving.
	}

	// A public method to open the form for creating a new character.
	public void CreateNewCharacter(int entityId)
	{
		_entityType = entityId;
		this.Title = "Create New Character";
		_editingEntity = null; // Ensure we are not in edit mode.
							   // Clear all form fields here...
		_nameInput.Text = "";
		_occupationInput.Text = "";
		_ageInput.Value = 10;
		_strInput.Value = 10;
		_dexInput.Value = 10;
		_conInput.Value = 10;
		_appInput.Value = 10;
		_sizInput.Value = 10;
		_powInput.Value = 10;
		_intInput.Value = 10;
		_eduInput.Value = 10;
		_mhpInput.Value = 10;
		_mmpInput.Value = 10;
		_sanInput.Value = 10;
		_lckInput.Value = 10;

		switch (_entityType)
		{
			case Constants.INV_ID:
				_playerHBox.Show();
				_playerInput.Text = "";
				break;
			case Constants.NPC_ID:
				_affiliationsHBox.Show();
				_affiliationsInput.Text = "";
				break;
		}

		this.PopupCentered();
	}
	
	public void PopupToEdit(Entity character)
	{
		_editingEntity = character; // Store the character we are editing
		this.Title = $"Edit {character.Name}";
		
		// --- Populate General Fields ---
		_nameInput.Text = character.Name;
		_occupationInput.Text = character.Occupation;
		_ageInput.Value = character.Age;
		_notesInput.Text = character.Notes;

		// --- Populate Characteristics ---
		_strInput.Value = character.Strength;
		_dexInput.Value = character.Dexterity;
		_conInput.Value = character.Constitution;
		_appInput.Value = character.Appearance;
		_sizInput.Value = character.Size;
		_powInput.Value = character.Power;
		_intInput.Value = character.Intelligence;
		_eduInput.Value = character.Education;

		// --- Populate Stats ---
		_mhpInput.Value = character.HitPointsMax;
		_mmpInput.Value = character.MagicPointsMax;
		_sanInput.Value = character.SanityMax;
		_lckInput.Value = character.LuckMax;

		// --- Populate Entity-Specific Fields ---
		if (character is Investigator investigator)
		{
			_playerHBox.Show();
			_playerInput.Text = investigator.Player;
			_entityType = Constants.INV_ID;
		}
		else if (character is Npc npc)
		{
			_affiliationsHBox.Show();
			_affiliationsInput.Text = npc.Affiliations;
			_entityType = Constants.NPC_ID;
		}

		// --- Populate Skills ---
		_sklInput.Text = ""; // Clear existing text
		if (character.Skills != null && character.Skills.Count > 0)
		{
			var skillStrings = new System.Collections.Generic.List<string>();
			foreach (var skill in character.Skills)
			{
				skillStrings.Add($"{skill.Key}: {skill.Value}");
			}
			_sklInput.Text = string.Join(", ", skillStrings);
		}
		
		PopupCentered();
	}
}
