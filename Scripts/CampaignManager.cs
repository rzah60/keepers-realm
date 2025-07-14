// CampaignManager.cs
using Godot;

public partial class CampaignManager : Node
{
	// This static property will hold the one-and-only instance of our manager.
	public static CampaignManager Instance { get; private set; }

	// Hold the campaign data for the currently running game.
	public Campaign CurrentCampaign { get; set; }

	public override void _Ready()
	{
		// When the CampaignManager node is loaded by Godot, it assigns itself
		// to the static 'Instance' property. This makes the single instance
		// accessible globally from anywhere in your code.
		Instance = this;
	}
}
