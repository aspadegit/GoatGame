using Godot;
public class Material
{
    public string Name {get; private set;}
    public int ID {get; private set;}
    public string Type {get; private set;}
    
    public Material()
    {
       Name = "[UNITIALIZED NAME]";
       ID = -1;
    }

    public Material(string Name, int ID, string Type)
    {
        this.Name = Name;
        this.ID = ID;
        this.Type = Type;
    }

    public override string ToString()
    {
        return "MATERIAL PRINT\nName: " + Name + "\nType: " + Type;
    }

}   