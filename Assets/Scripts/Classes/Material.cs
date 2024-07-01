using Godot;
public class Material
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

}   