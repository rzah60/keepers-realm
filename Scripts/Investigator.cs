// Investigator.cs
using Godot;

[GlobalClass]
public partial class Investigator : Entity
{
	[Export] public string Player { get; set; }
	public Investigator() : base(Constants.INV_ID) {}
}
