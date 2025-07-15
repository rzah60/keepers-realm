// CharacterSheetPopup.cs
using Godot;

public partial class CharacterSheetPopup : Window
{
	[Signal] public delegate void EditCharacterRequestedEventHandler(Entity character);
	// Export all the UI nodes you just created
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


	public override void _Ready()
	{
		this.CloseRequested += () => Hide(); // Hide popup on 'X' click
	}

	public void DisplayCharacter(Entity data)
	{
		HideEntitySpecificFields();
		this.Title = data.Name;
		_nameLabel.Text = $"Name: {data.Name}";
		_occupationLabel.Text = $"Occupation: {data.Occupation}";

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
				_weaponsList.AddItem($"{weapon.Name} ({weapon.SkillValue}%) - Dmg: {weapon.Damage}");
			}
		}

		_notesLabel.Text = $"Notes: {data.Notes}";

		this.PopupCentered();
	}
	private void HideEntitySpecificFields()
	{
		_playerHBox.Hide();
		_affiliationsHBox.Hide();
	}

	private void UpdateBar(ProgressBar bar, Label text, int current, int max)
	{
		bar.MaxValue = max;
		bar.Value = current;
		text.Text = $"{current} / {max}";
	}
}
