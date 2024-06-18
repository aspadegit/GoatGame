using System.Collections.Generic;
using Godot;
public class Recipe
{
    public string Name {get; private set;} 
    public int ID {get; private set;}
    public Dictionary<string, int> RequiredItems {get; private set;} //material ID, amount of that material

    public Recipe()
    {
       Name = "[UNITIALIZED NAME]";
       ID = -1;
    }

    public Recipe(int ID, Dictionary<string, int> RequiredItems)
    {
        Name = "";
        this.ID = ID;
        this.RequiredItems = RequiredItems;
    }
    public Recipe(string Name, int ID, Dictionary<string, int> RequiredItems)
    {
        this.Name = Name;
        this.ID = ID;
        this.RequiredItems = RequiredItems;
    }

    public override string ToString()
    {
        return "RECIPE PRINT\nName: " + Name;
    }

}   