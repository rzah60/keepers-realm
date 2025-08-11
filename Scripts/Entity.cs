// Entity.cs
using Godot;
using System.Collections.Generic;

[GlobalClass]
public abstract partial class Entity : Resource
{
	// Personal Details from the character sheet
	[Export] public int EntityId { get; set; }
	[Export] public string Name { get; set; } // [cite: 3]
	[Export] public string Occupation { get; set; } // [cite: 6]
	[Export] public int Age { get; set; } // [cite: 7]
	[Export] public string Sex { get; set; } // [cite: 8]
	[Export] public string Notes { get; set; }

	// Core Characteristics
	[Export] public int Strength { get; set; } // 
	[Export] public int Dexterity { get; set; } // 
	[Export] public int Intelligence { get; set; } //  
	[Export] public int Constitution { get; set; } // 
	[Export] public int Appearance { get; set; } // 
	[Export] public int Power { get; set; } // 
	[Export] public int Size { get; set; } // 
	[Export] public int Education { get; set; } // 

	// Derived Characteristics & Combat Stats
	[Export] public int Idea { get; set; } // 
	[Export] public int Know { get; set; } // 
	[Export] public string MoveRate { get; set; } // 
	[Export] public string DamageBonus { get; set; } // [cite: 137]
	[Export] public int Build { get; set; } // [cite: 138]

	// Sanity, Magic, and Hit Points
	[Export] public int SanityCurrent { get; set; } // [cite: 9]
	[Export] public int SanityMax { get; set; } // [cite: 9]
	[Export] public int MagicPointsCurrent { get; set; } // [cite: 10]
	[Export] public int MagicPointsMax { get; set; } // [cite: 10]
	[Export] public int HitPointsCurrent { get; set; } // [cite: 22]
	[Export] public int HitPointsMax { get; set; } // [cite: 22]

	// Luck
	[Export] public int LuckCurrent { get; set; } // [cite: 35]
	[Export] public int LuckMax { get; set; } // [cite: 35]

	// Skills
	[Export] public Godot.Collections.Dictionary<string, int> Skills { get; set; } = new Godot.Collections.Dictionary<string, int>(); // 

	// A struct to hold weapon data
	public partial class WeaponStats : Resource
	{
		[Export] public string Name; // [cite: 131]
		[Export] public int SkillValue; // [cite: 132]
		[Export] public string Damage; // [cite: 133]
		[Export] public string Range; // [cite: 134]
		[Export] public int Attacks; // [cite: 134]
		[Export] public int Ammo; // [cite: 135]
		[Export] public int Malfunction; // [cite: 136]
	}
	[Export] public Godot.Collections.Array<WeaponStats> Weapons { get; set; } = new Godot.Collections.Array<WeaponStats>();
	
	public Entity() {}
	
	public Entity(int entityData)
	{
		EntityId = entityData;
	}
}
