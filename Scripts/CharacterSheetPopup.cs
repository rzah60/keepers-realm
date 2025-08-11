// CharacterSheetPopup.cs
using Godot;

public partial class CharacterSheetPopup : Window
{
	[Signal] public delegate void EditCharacterRequestedEventHandler(Entity character);

	[Export] private Label _nameLabel, _occupationLabel, _playerLabel, _affiliationsLabel;
	[Export] private HBoxContainer _playerHBox, _affiliationsHBox;
	[Export] private Label _ageLabel, _strLabel, _dexLabel, _intLabel, _conLabel, _appLabel, _powLabel, _sizLabel, _eduLabel;
	[Export] private ProgressBar _hpBar;
	[Export] private Label _hpValueLabel;
	[Export] private ProgressBar _sanityBar;
	[Export] private Label _sanityValueLabel;
	[Export] private ProgressBar _mpBar;
	[Export] private Label _mpValueLabel;
	[Export] private ProgressBar _luckBar;
	[Export] private Label _luckValueLabel;
	[Export] private ItemList _skillsList;
	[Export] private ItemList _weaponsList;
	[Export] private Label _notesLabel;
	[Export] private Button _editButton;

	private Entity _character;

	public override void _Ready()
	{
		this.CloseRequested += () => Hide();
		_editButton.Pressed += OnEditButtonPressed;
	}

	public void DisplayCharacter(Entity data)
	{
		_character = data; // Store the character for the edit button to use

		HideEntitySpecificFields();
		this.Title = data.Name;
		_nameLabel.Text = $"Name: {data.Name}";
		_occupationLabel.Text = $"Occupation: {data.Occupation}";
		_ageLabel.Text = $"Age: {data.Age}";
		_notesLabel.Text = data.Notes;

		switch (data.EntityId)
		{
			case Constants.INV_ID:
				_playerHBox.Show();
				_playerLabel.Text = $"Player: {((Investigator)data).Player}";
				break;
			case Constants.NPC_ID:
				_affiliationsHBox.Show();
				_affiliationsLabel.Text = $"Affiliations: {((Npc)data).Affiliations}";
				break;
		}

		_strLabel.Text = $"STR: {data.Strength}";
		_dexLabel.Text = $"DEX: {data.Dexterity}";
		_intLabel.Text = $"INT: {data.Intelligence}";
		_conLabel.Text = $"CON: {data.Constitution}";
		_appLabel.Text = $"APP: {data.Appearance}";
		_powLabel.Text = $"POW: {data.Power}";
		_sizLabel.Text = $"SIZ: {data.Size}";
		_eduLabel.Text = $"EDU: {data.Education}";

		UpdateBar(_hpBar, _hpValueLabel, data.HitPointsCurrent, data.HitPointsMax);
		UpdateBar(_sanityBar, _sanityValueLabel, data.SanityCurrent, data.SanityMax);
		UpdateBar(_mpBar, _mpValueLabel, data.MagicPointsCurrent, data.MagicPointsMax);
		UpdateBar(_luckBar, _luckValueLabel, data.LuckCurrent, data.LuckMax);

		_skillsList.Clear();
		if (data.Skills != null)
		{
			foreach (var skill in data.Skills)
			{
				_skillsList.AddItem($"{skill.Key}: {skill.Value}%");
			}
		}

		_weaponsList.Clear();
		if (data.Weapons != null)
		{
			foreach (var weapon in data.Weapons)
			{
				_weaponsList.AddItem($"{weapon.Name}: {weapon.SkillValue}%");
			}
		}

		PopupCentered();
	}

	private void OnEditButtonPressed()
	{
		EmitSignal(SignalName.EditCharacterRequested, _character);
		Hide();
	}

	private void HideEntitySpecificFields()
	{
		_playerHBox.Hide();
		_affiliationsHBox.Hide();
	}

	private void UpdateBar(ProgressBar bar, Label label, int current, int max)
	{
		bar.MaxValue = max;
		bar.Value = current;
		label.Text = $"{current}/{max}";
	}
}
