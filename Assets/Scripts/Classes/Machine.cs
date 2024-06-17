using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Godot;
public class Machine
{
    public string Name {get; private set;}
    public int ID {get; private set;}
    public string Type {get; private set;}
    public int Level {get; private set;}
    public int Damage {get; private set;}
    public string Description {get; private set;}
    public Recipe CraftingRecipe {get; private set;}
    
    public Machine()
    {
       Name = "[UNITIALIZED NAME]";
       ID = -1;
       Type = "[UNINITIALIZED TYPE]";
       Level = 1;
       Damage = -1;
    }

    public Machine(string Name, int ID, string Type, int Level, int Damage, string Description, Recipe CraftingRecipe)
    {
        this.Name = Name;
        this.ID = ID;
        this.Level = Level;
        this.Type = Type;
        this.Damage = Damage;
        this.Description = Description;
        this.CraftingRecipe = CraftingRecipe;
    }

    public override string ToString()
    {
        return "MACHINE PRINT\nName: " + Name + "\nLevel: " + Level + "\nDamage: " + Damage;
    }

}   