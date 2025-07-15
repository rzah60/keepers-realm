// Entity.cs
using Godot;
using System.Collections.Generic;

[GlobalClass]
public abstract partial class Entity : Resource
{
	// Personal Details from the character sheet
	public int EntityId { get; set; }
	public string Name { get; set; } // [cite: 3]
	public string Occupation { get; set; } // [cite: 6]
	public int Age { get; set; } // [cite: 7]
	public string Sex { get; set; } // [cite: 8]
	public string Notes { get; set; }

	// Core Characteristics
	public int Strength { get; set; } // 
	public int Dexterity { get; set; } // 
	public int Intelligence { get; set; } //  
	public int Constitution { get; set; } // 
	public int Appearance { get; set; } // 
	public int Power { get; set; } // 
	public int Size { get; set; } // 
	public int Education { get; set; } // 

	// Derived Characteristics & Combat Stats
	public int Idea { get; set; } // 
	public int Know { get; set; } // 
	public string MoveRate { get; set; } // 
	public string DamageBonus { get; set; } // [cite: 137]
	public int Build { get; set; } // [cite: 138]

	// Sanity, Magic, and Hit Points
	public int SanityCurrent { get; set; } // [cite: 9]
	public int SanityMax { get; set; } // [cite: 9]
	public int MagicPointsCurrent { get; set; } // [cite: 10]
	public int MagicPointsMax { get; set; } // [cite: 10]
	public int HitPointsCurrent { get; set; } // [cite: 22]
	public int HitPointsMax { get; set; } // [cite: 22]

	// Luck
	public int LuckCurrent { get; set; } // [cite: 35]
	public int LuckMax { get; set; } // [cite: 35]

	// Skills
	public Dictionary<string, int> Skills { get; set; } = new Dictionary<string, int>(); // 

	// A struct to hold weapon data
	public struct WeaponStats
	{
		public string Name; // [cite: 131]
		public int SkillValue; // [cite: 132]
		public string Damage; // [cite: 133]
		public string Range; // [cite: 134]
		public int Attacks; // [cite: 134]
		public int Ammo; // [cite: 135]
		public int Malfunction; // [cite: 136]
	}
	public List<WeaponStats> Weapons { get; set; } = new List<WeaponStats>();
	
	public Entity() {}
	
	public Entity(int entityData)
	{
		EntityId = entityData;
	}
}
