// Npc.cs
using Godot;

[GlobalClass]
public partial class Npc : Entity
{
	[Export] public string Affiliations { get; set; }
	public Npc() : base(Constants.NPC_ID) { }
}
