using Godot;
using System;
using System.Linq;

public partial class InventoryRow : RowScript
{
	[Export]
	Texture2D temp; //TODO: DELETE ME

	public string type;
	public string name;
	public int ID;

	public void Setup(string name, string type, int ID)
	{
		this.type = type;
		this.ID = ID;
		this.name = name;
		string amount = "";
		int amt = -1;

		Texture2D[] tex = new Texture2D[]{temp};
		
		//not ideal lol
		switch(type)
		{
			case "goat":
				amount = "";
				break;

			//should never result in -1, -1 means that it's showing something that you don't have in your inventory...
			case "material":
				amt = GlobalVars.materialsObtained.ContainsKey(name) ? GlobalVars.materialsObtained[name] : -1; 
				tex = new Texture2D[]{GlobalVars.materials[name].Texture};
				break;
			case "machine":
				amt = GlobalVars.machineInventory.ContainsKey(ID) ? GlobalVars.machineInventory[ID] : -1; 
				break;
		}

		ButtonSetup();

		//it's not a goat, set the count variable properly
		if(amt != -1)
		{
			//just makes sure it shows up in x00 format
			amount = (amt < 10)? ("0" + amt) : amt.ToString();
			amount = "x" + amount;
		}

		base.Setup(new string[]{name,amount}, tex);	

	}

	public override void Setup(string[] labelTexts, Texture2D[] textures)
	{
		ButtonSetup();
		base.Setup(labelTexts, textures);	
	}

	private void ButtonSetup()
	{
		string[] rowButtonStr = {"RowButton", "pressed"};
		buttonActions.Add(rowButtonStr, ClickRow);	//buttonActions inherited from RowScript
		
		rowButtonStr = new string[]{"RowButton", "focus_entered"};
		buttonActions.Add(rowButtonStr, HoverRow);

		rowButtonStr = new string[]{"RowButton", "mouse_entered"};
		buttonActions.Add(rowButtonStr, HoverRow);

	}

	private void ClickRow()
	{
		//TODO: items can be used when clicked
		GD.Print("clicked");
	}

	private void HoverRow()
	{
		//emit this script to the inventory menu when hovering
		SignalHandler.Instance.EmitSignal(SignalHandler.SignalName.OnInventoryHover, this);

	}
}
