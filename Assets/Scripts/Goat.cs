using Godot;
public class Goat
{
    public string Name {get; set;}
    public int ID {get; private set;}
    public string Class {get; private set;} //TODO: potentially, make this a subclass (Coding term not gaming term)
    public float Stamina {get; private set;}
    public int Level {get; private set;}
    public int Exp { get; private set;}

    public Goat()
    {
        Name = "[UNINITIALIZED GOAT]";
        Class = "[UNINITIALIZED CLASS]";
        ID = -1;
        Stamina = -1;
        Level = -1;
        Exp = -1;
    }
    public Goat(string Name, string Class, float Stamina, int Level, int Exp)
    {
        this.Name = Name;
        this.Class = Class;
        this.Stamina = Stamina;
        this.Level = Level;
        this.Exp = Exp;
        this.ID = 1; //TODO: change
    }
}   