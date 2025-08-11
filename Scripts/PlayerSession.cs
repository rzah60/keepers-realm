// In PlayerSession.cs
using Godot;

public partial class PlayerSession : RefCounted
{
    public long PeerId { get; set; }
    public Investigator ChosenInvestigator { get; set; }
    // We can add more player-specific state here later
}