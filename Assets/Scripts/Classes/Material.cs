using System;
using Godot;
public class Material : ISortable
{
    public string Name {get; set;}
    public int ID {get; set;}
    public string Description {get; set;}
    public string Type {get; set;}
    public Texture2D Texture {get;set;}
    
    public Material()
    {
       Name = "[UNITIALIZED NAME]";
       ID = -1;
    }

    public Material(string Name, int ID, string Description, string Type)
    {
        this.Name = Name;
        this.ID = ID;
        this.Description = Description;
        this.Type = Type;
    }
    public Material(string Name, int ID, string Description, string Type, Texture2D Texture)
    {
        this.Name = Name;
        this.ID = ID;
        this.Description = Description;
        this.Type = Type;
        this.Texture = Texture;
    }

    public override string ToString()
    {
        return "MATERIAL PRINT\nName: " + Name + "\nType: " + Type;
    }

    public int Compare(Object other, int howToCompare)
    {
        //TODO: (potentially) throw error?
        if(other.GetType() != typeof(Material))
        {
            return 0;
        }

        Material toCompare = (Material)other;

        switch(howToCompare)
        {
            // sort by name
            case 0:
                return Name.CompareTo(toCompare.Name);
            // by amount
            case 1:
                int amtOfThis = GlobalVars.materialsObtained[Name];
                int amtOfOther = GlobalVars.materialsObtained[toCompare.Name];
                return amtOfOther.CompareTo(amtOfThis); // gives BIGGEST AMOUNT first
            // by type
            case 2:
                return Type.CompareTo(toCompare.Type);
            default:
                GD.PrintErr("When attempting to compare Material " + Name + " to Material " + toCompare.Name + ", an out of bounds comparison index was found");
                break;
        }

        return 0;
    }

}   