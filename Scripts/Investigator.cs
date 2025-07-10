// Investigator.cs
public partial class Investigator
{
	public string Name { get; set; }
	public int Strength { get; set; }
	public int Dexterity { get; set; }
	public int Constitution { get; set; }
	public int Power { get; set; }
	// Add other CoC stats like INT, APP, SIZ, EDU...

	public int CurrentSanity { get; set; }
	public int MaxSanity { get; set; }
	public int CurrentHealth { get; set; }
	public int MaxHealth { get; set; }

	// A constructor is a special method for creating new objects.
	public Investigator(string name, int str, int dex, int pow, int con)
	{
		Name = name;
		Strength = str;
		Dexterity = dex;
		Power = pow;
		Constitution = con;

		// Per CoC rules, max sanity is derived from Power.
		MaxSanity = Power * 5;
		CurrentSanity = MaxSanity;

		// Health is often based on CON and SIZ. We'll simplify for now.
		MaxHealth = (Constitution + 10); // Placeholder
		CurrentHealth = MaxHealth;
	}
}
