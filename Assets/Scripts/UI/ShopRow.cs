using Godot;
using System;
using System.Linq;

public partial class ShopRow : RowScript
{
	[Export]
	Texture2D temp; //TODO: DELETE ME

	[Export]
	public TextEntry textEntry;

	public string type;
	public string name;
	public int ID;
    private int currentAmt;
	public int cost;

	public string hash;

	public void Setup(string name, string type, int ID)
	{
		this.type = type;
		this.ID = ID;
		this.name = name;
		string amount = "";
		int amt = -1;
		cost = -1;

		hash = name+ID;

		Texture2D[] tex = new Texture2D[]{temp};
		
		//not ideal lol
		switch(type)
		{
			
			//0 means that the player has none of that item, and can buy it
			case "material":
				amt = GlobalVars.materialsObtained.ContainsKey(name) ? GlobalVars.materialsObtained[name] : 0; 
				tex = new Texture2D[]{GlobalVars.materials[name].Texture};
				cost = GlobalVars.materials[name].Value;
				break;
			case "machine":
				amt = GlobalVars.machineInventory.ContainsKey(ID) ? GlobalVars.machineInventory[ID] : 0; 
				cost = GlobalVars.machines[ID].Value;
				break;
			case "item":
				amt = GlobalVars.itemInventory.ContainsKey(name) ? GlobalVars.itemInventory[name] : 0; 
				cost = GlobalVars.items[name].Value;

				break;
		}

		ButtonSetup();

		//set the count variable properly
		if(amt != -1)
		{
			//just makes sure it shows up in x00 format
			amount = (amt < 10)? ("0" + amt) : amt.ToString();
			amount = "(owned: x" + amount + ")";
		}

		base.Setup(new string[]{name,amount, cost.ToString()}, tex);	

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
		//TODO: focus on text entry / controller inputs
		GD.Print("clicked");
	}

	private void LeftArrow()
	{
		
	}

	private void HoverRow()
	{
		//emit this script to the inventory menu when hovering
		//SignalHandler.Instance.EmitSignal(SignalHandler.SignalName.OnInventoryHover, this);

	}



}
