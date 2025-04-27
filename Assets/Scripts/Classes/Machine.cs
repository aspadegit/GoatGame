using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Godot;
using System;
public class Machine : ISortable
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

	public int Value { get; set;} // buy price

	public int[] Size {get;set;} // x, y
	public Machine()
	{
	   Name = "[UNITIALIZED NAME]";
	   ID = -1;
	   Type = "[UNINITIALIZED TYPE]";
	   Level = 1;
	}

	public Machine(string Name, int ID, string Type, int Level, int Range, int FireRate, Shot ShotType, string Description, Recipe CraftingRecipe, int[] TextureCoords, int[] Size)
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
		this.Size = Size;
		
		// calculate the buy price, TODO: adjust for scaling purposes later
		Value = CraftingRecipe.Value * Level * 2;
	}

	public override string ToString()
	{
		return "MACHINE PRINT\nName: " + Name + "\nLevel: " + Level + "\nDamage: " + ShotType.Damage;
	}

	public string GetStatString()
	{
		return "Type: " + Type + "\nLevel: " + Level + "\nDamage: " + ShotType.Damage + "\nDescription: " + Description;
	}
    public int Compare(Object other, int howToCompare)
    {
        //TODO: (potentially) throw error?
        if(other.GetType() != typeof(Machine))
        {
            return 0;
        }

        Machine toCompare = (Machine)other;

        switch(howToCompare)
        {
            // sort by name
            case 0:
                return Name.CompareTo(toCompare.Name);
            // by amount
            case 1:
                int amtOfThis = GlobalVars.machineInventory[ID];
                int amtOfOther = GlobalVars.machineInventory[toCompare.ID];
                return amtOfOther.CompareTo(amtOfThis); // gives BIGGEST AMOUNT first
            // by type
            case 2:
                return Type.CompareTo(toCompare.Type);
			case 3:
				return toCompare.Level.CompareTo(Level); // BIGGEST level first
            default:
                GD.PrintErr("When attempting to compare Machine " + Name + " to Machine " + toCompare.Name + ", an out of bounds comparison index was found");
                break;
        }

        return 0;
    }

}   
