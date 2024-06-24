using System.Collections.Generic;
using Godot;

public class Item
{
    public string Name { get; private set; }
    public int ID { get; private set; }
    public string Description { get; private set; }
    public Dictionary<string, int> Stats { get; private set; } // enhancements it provides methinks
    public int Value { get; private set; } // how much its worth when buying
    public int SellValue { get; private set; } // how much its worth when selling

    public Item()
    {
        Name = "[UNINITIALIZED ITEM]";
        ID = -1;
        Description = "[UNINITIALIZED DESCRIPTION]";
        Stats = new Dictionary<string, int>();
        Value = 0;
        SellValue = 0;
    }

    public Item(string name, int id, string description, Dictionary<string, int> stats, int value, int sellValue)
    {
        this.Name = name;
        this.ID = id;
        this.Description = description;
        this.Stats = stats;
        this.Value = value;
        this.SellValue = sellValue;
    }

    public override string ToString()
    {
        string returnStr = "ITEM PRINT\nName: " + Name + "\nID: " + ID + "\nDescription: " + Description + "\nValue: " + Value + "\nSell Value: " + SellValue + "\nStats: ";
        foreach (KeyValuePair<string, int> pair in Stats)
        {
            returnStr += "(";
            returnStr += pair.Key;
            returnStr += ": ";
            returnStr += pair.Value;
            returnStr += ")\n";
        }

        return returnStr;
    }
}
