// Campaign.cs
using Godot;

[GlobalClass]
public partial class Campaign : Resource
{
	[Export] public string CampaignName { get; set; }
	[Export] public Godot.Collections.Array<Npc> Npcs { get; set; } = new Godot.Collections.Array<Npc>();
	[Export] public Godot.Collections.Array<Investigator> Investigators { get; set; } = new Godot.Collections.Array<Investigator>();
	// Add lists for Investigators, Monsters, etc. here later.
}
