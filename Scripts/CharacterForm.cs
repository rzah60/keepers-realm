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
	[Export] private TextEdit _ntsInput;
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
		// If we are not editing an existing NPC, create a new one.
		if (_editingEntity == null)
		{
			switch (_entityType)
			{
				case Constants.INV_ID:
					_editingEntity = new Investigator();
					((Investigator)_editingEntity).Player = _playerInput.Text;
					_playerHBox.Hide();
					break;
				case Constants.NPC_ID:
					_editingEntity = new Npc();
					((Npc)_editingEntity).Affiliations = _affiliationsInput.Text;
					_affiliationsHBox.Hide();
					break;
			}
		}

		// Populate the Npc object with data from the input fields.
		_editingEntity.Name = _nameInput.Text;
		_editingEntity.Occupation = _occupationInput.Text;
		_editingEntity.Age = (int)_ageInput.Value;
		_editingEntity.Strength = (int)_strInput.Value;
		_editingEntity.Dexterity = (int)_dexInput.Value;
		_editingEntity.Constitution = (int)_conInput.Value;
		_editingEntity.Appearance = (int)_appInput.Value;
		_editingEntity.Size = (int)_sizInput.Value;
		_editingEntity.Power = (int)_powInput.Value;
		_editingEntity.Intelligence = (int)_intInput.Value;
		_editingEntity.Education = (int)_eduInput.Value;
		_editingEntity.HitPointsMax = (int)_mhpInput.Value;
		_editingEntity.MagicPointsMax = (int)_mmpInput.Value;
		_editingEntity.SanityMax = (int)_sanInput.Value;
		_editingEntity.LuckMax = (int)_lckInput.Value;
		_editingEntity.Skills = ParseSkills(_sklInput.Text);
		_editingEntity.Notes = _ntsInput.Text;
		// ... Get the value from ALL your other input nodes ...

		// Emit the signal, sending the completed NPC object.
		EmitSignal(SignalName.CharacterSaved, _editingEntity);
		Hide(); // Close the form
	}

	private Dictionary<string, int> ParseSkills(string textEditStr)
	{
		string[] skillValueStrs = textEditStr.Split(',');
		Dictionary<string, int> skillValuePairs = new Dictionary<string, int>();

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
}
