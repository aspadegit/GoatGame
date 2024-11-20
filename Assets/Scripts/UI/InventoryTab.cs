using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class InventoryTab : MarginContainer
{
	[Export]
	VBoxContainer scrollVBox;

	[Export]
	PackedScene inventoryRow;

	[Export]
	string type;

	[Export]
	Label infoLabel;

	[Export]
	OptionButton dropdown;

	// USED FOR SORTING
	Dictionary<int, ISortable> sortableRows = new Dictionary<int, ISortable>(); // tab-assigned ID (separate from GlobalVars ID), the object it is
	List<int> currentIDOrder = new List<int>(); // current ID order (what gets sorted)

	//TODO: sort button

	public override void _Ready()
	{
		SignalHandler.Instance.Connect(SignalHandler.SignalName.OnInventoryHover, Callable.From((InventoryRow row)=> OnRowHover(row)), (uint)ConnectFlags.Deferred);

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
			case "item":
				InstantiateItems();
				break;
		}

		infoLabel.Text = "";
		

	}

	public void OnSortOption(int index)
	{
		Godot.Collections.Array<Node> children = scrollVBox.GetChildren();

		currentIDOrder.Sort((a,b) => sortableRows[a].Compare(sortableRows[b], index));

		for(int i = 0; i < children.Count; i++)
		{
			scrollVBox.MoveChild(scrollVBox.FindChild(currentIDOrder[i].ToString(), false, false), i);
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
		bool sortableRowsFillable = sortableRows.Count > 0? false : true;
		//spawn in new entries
		int i = 0;
		foreach(KeyValuePair<string, int> entry in GlobalVars.materialsObtained)
		{
			//instantiate
			InventoryRow row = inventoryRow.Instantiate<InventoryRow>();
			row.Setup(entry.Key, "material", GlobalVars.materials[entry.Key].ID);
			row.Name = i.ToString();//entry.Key.ToString();
			scrollVBox.AddChild(row);
			
			//avoids adding twice
			if(sortableRowsFillable)
			{
				sortableRows.Add(i, GlobalVars.materials[entry.Key]);
				currentIDOrder.Add(i);
			}
			
			i++;
		}
	}
	private void InstantiateMachines()
	{
		bool sortableRowsFillable = sortableRows.Count > 0? false : true;
		int i = 0;

		//spawn in new entries
		foreach(KeyValuePair<int, int> entry in GlobalVars.machineInventory)
		{
			//instantiate
			InventoryRow row = inventoryRow.Instantiate<InventoryRow>();
			row.Setup(GlobalVars.machines[entry.Key].Name, "machine", entry.Key);
			row.Name = i.ToString();//entry.Key.ToString();
			scrollVBox.AddChild(row);

			//avoids adding twice
			if(sortableRowsFillable)
			{
				sortableRows.Add(i, GlobalVars.machines[entry.Key]);
				currentIDOrder.Add(i);
			}
			i++;
		}
	}
	private void InstantiateItems()
	{
		//spawn in new entries
		foreach(KeyValuePair<string, int> entry in GlobalVars.itemInventory)
		{
			//instantiate
			InventoryRow row = inventoryRow.Instantiate<InventoryRow>();
			row.Setup(entry.Key, "item", GlobalVars.items[entry.Key].ID);
			row.Name = entry.Key.ToString();
			scrollVBox.AddChild(row);
		}
	}

	private void OnRowHover(InventoryRow row)
	{
		//only adjust the tab whose type matches
		if(type == row.type)
		{
			switch(row.type)
			{
				case "goat":
					infoLabel.Text = GlobalVars.goats[row.ID].ToString();
					break;
				case "machine":
					infoLabel.Text = GlobalVars.machines[row.ID].ToString();
					break;
				case "material":
					infoLabel.Text = GlobalVars.materials[row.name].ToString();
					break;
				case "item":
					infoLabel.Text = GlobalVars.items[row.name].ToString();
					break;
			}
		}
	}

}
