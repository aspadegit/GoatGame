using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class InventoryTab : MarginContainer
{
	[Export]
	VBoxContainer scrollVBox;

	[Export]
	PackedScene inventoryRow;

	[Export]
	string type;

	//TODO: sort button

	public override void _Ready()
	{
		InstantiateChildren();
	}

	public void InstantiateChildren()
	{
		//remove all of the old machine instances (TODO: not always do this)
		foreach(Node child in scrollVBox.GetChildren())
		{
			scrollVBox.RemoveChild(child);
			child.QueueFree();
		}
		
		//not the best
		switch(type)
		{
			case "goat":
				InstantiateGoats();
				break;
			case "machine":
				InstantiateMachines();
				break;
			case "material":
				InstantiateMaterials();
				break;
		}

	}

	//a failure of my architecture... <int,Goat>, <string,int>, <int,int>...they all have to be separate
	private void InstantiateGoats()
	{
		//spawn in new entries
		foreach(KeyValuePair<int, Goat> entry in GlobalVars.goats)
		{
			//instantiate
			InventoryRow row = inventoryRow.Instantiate<InventoryRow>();
			row.Setup(entry.Value.Name, "goat", entry.Key);
			row.Name = entry.Key.ToString();
			scrollVBox.AddChild(row);
		}
	}

	private void InstantiateMaterials()
	{
			//spawn in new entries
		foreach(KeyValuePair<string, int> entry in GlobalVars.materialsObtained)
		{
			//instantiate
			InventoryRow row = inventoryRow.Instantiate<InventoryRow>();
			row.Setup(entry.Key, "material", GlobalVars.materials[entry.Key].ID);
			row.Name = entry.Key.ToString();
			scrollVBox.AddChild(row);
		}
	}
	private void InstantiateMachines()
	{
		//spawn in new entries
		foreach(KeyValuePair<int, int> entry in GlobalVars.machineInventory)
		{
			//instantiate
			InventoryRow row = inventoryRow.Instantiate<InventoryRow>();
			row.Setup(GlobalVars.machines[entry.Key].Name, "machine", entry.Key);
			row.Name = entry.Key.ToString();
			scrollVBox.AddChild(row);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
