using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Godot;
public class Machine
{
	public string Name {get; private set;}
	public int ID {get; private set;}
	public string Type {get; private set;}
	public int Level {get; private set;}
	public int Range {get; private set;}
	public int FireRate {get; private set;}
	public Shot ShotType {get; private set;}
	public string Description {get; private set;}
	public Recipe CraftingRecipe {get; private set;}
	public int[] TextureCoords {get; set;}	//x,y,w,h (check the atlas coords for what image a tower should show; currently unused)

	public Machine()
	{
	   Name = "[UNITIALIZED NAME]";
	   ID = -1;
	   Type = "[UNINITIALIZED TYPE]";
	   Level = 1;
	}

	public Machine(string Name, int ID, string Type, int Level, int Range, int FireRate, Shot ShotType, string Description, Recipe CraftingRecipe, int[] TextureCoords)
	{
		this.Name = Name;
		this.ID = ID;
		this.Level = Level;
		this.Type = Type;
		this.Range = Range;
		this.FireRate = FireRate;
		this.ShotType = ShotType;
		this.Description = Description;
		this.CraftingRecipe = CraftingRecipe;
		this.TextureCoords = TextureCoords;

	}

	public override string ToString()
	{
		return "MACHINE PRINT\nName: " + Name + "\nLevel: " + Level + "\nDamage: " + ShotType.Damage;
	}

	public string GetStatString()
	{
		return "Type: " + Type + "\nLevel: " + Level + "\nDamage: " + ShotType.Damage + "\nDescription: " + Description;
	}


}   
