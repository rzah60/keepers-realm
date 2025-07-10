// CharacterSheetPopup.cs
using Godot;

public partial class CharacterSheetPopup : Window
{
	// Export all the UI nodes you just created
	[Export] private Label _nameLabel, _occupationLabel;
	[Export] private Label _strLabel, _dexLabel, _intLabel, _conLabel, _appLabel, _powLabel, _sizLabel, _eduLabel;
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
	[Export] private Label _residenceLabel;
	[Export] private Label _birthplaceLabel;


	public override void _Ready()
	{
		this.CloseRequested += () => Hide(); // Hide popup on 'X' click
	}

	public void DisplayCharacter(Npc data)
	{
		this.Title = data.Name;
		_nameLabel.Text = $"Name: {data.Name}";
		_occupationLabel.Text = $"Occupation: {data.Occupation}";

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
			foreach(var weapon in data.Weapons)
			{
				_weaponsList.AddItem($"{weapon.Name} ({weapon.SkillValue}%) - Dmg: {weapon.Damage}");
			}
		}

		_residenceLabel.Text = $"Residence: {data.Residence}";
		_birthplaceLabel.Text = $"Birthplace: {data.Birthplace}";

		this.PopupCentered();
	}

	private void UpdateBar(ProgressBar bar, Label text, int current, int max)
	{
		bar.MaxValue = max;
		bar.Value = current;
		text.Text = $"{current} / {max}";
	}
}
