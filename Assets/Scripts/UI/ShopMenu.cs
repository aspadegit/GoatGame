using Godot;
using System;
using System.Collections.Generic;


public partial class ShopMenu : Control
{
	[Export]
	Godot.Collections.Array<string> items;

	[Export]
	Godot.Collections.Array<string> materials;

	[Export]
	Godot.Collections.Array<int> towers;

	[Export]
	PackedScene shopRow;

	[Export]
	VBoxContainer rowVboxContainer;

	[Export]
	Label runningCostLabel;

	Dictionary<string, int> currentAmt; // ShopRow.hash, howManyAreCurrentlySelected
	int runningCost = 0;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentAmt = new Dictionary<string, int>();
		SignalHandler.Instance.Connect(SignalHandler.SignalName.ShowShopMenu, Callable.From(()=> OnShow()), (uint)ConnectFlags.Deferred);
		SignalHandler.Instance.Connect(SignalHandler.SignalName.TextInputChanged, Callable.From((TextEntry t)=> OnRowValueChange(t)), (uint)ConnectFlags.Deferred);

		InstantiateRows();

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnRowValueChange(TextEntry textEntry)
	{
		
		ShopRow row = textEntry.rowParent as ShopRow;
		// make sure the TextEntry's parent is specifically a ShopRow
		if(row != null)
		{
			string textAmt = textEntry.innerText.Text;
			int amt = -1;
			
			// ignore, no length means the user is probably typing it in
			if(textAmt.Length < 1)
				return;

			//parse the ShopRow's text entry into a number & update running cost
			if(Int32.TryParse(textAmt, out amt))
			{
				// remove what was already added to running cost
				if(currentAmt.ContainsKey(row.hash) && currentAmt[row.hash] > 0)
				{
					int currentStoredCost = currentAmt[row.hash] * row.cost;
					runningCost -= currentStoredCost;
				}

				// calc new cost
				int totalCost = amt * row.cost;
				runningCost += totalCost;

				currentAmt[row.hash] = amt; // update dictionary
				UpdateRunningCost();

			}
			// text entry's value isnt an integer
			else
			{
				GD.PrintErr("ShopMenu.cs: Failed to parse TextEntry's value of " + textAmt + " into an Int32");
			}
			
		}
		// text entry isnt a shop row
		else
		{
			GD.PrintErr("ShopMenu.cs: OnRowValueChanged: textEntry's rowParent is null");
		}

	}


	private void UpdateRunningCost()
	{
		if(runningCost == 0)
		{
			runningCostLabel.Text = "000";
		}
		else
		{
			runningCostLabel.Text = runningCost.ToString();

		}
	}	

	private void OnShow()
	{
		Show();
		SignalHandler.Instance.EmitSignal(SignalHandler.SignalName.TogglePlayerMovement, false);

	}

	// close button signals to this
	public void HideAndReset()
	{
		Hide();
		SignalHandler.Instance.EmitSignal(SignalHandler.SignalName.TogglePlayerMovement, true);

	}


	private void InstantiateRows()
	{
		foreach(string name in items)
		{
			//instantiate
			ShopRow row = shopRow.Instantiate<ShopRow>();
			row.Setup(name, "item", GlobalVars.items[name].ID);
			row.Name = name.ToString();
			rowVboxContainer.AddChild(row);
		}

		foreach(string name in materials)
		{
			//instantiate
			ShopRow row = shopRow.Instantiate<ShopRow>();
			row.Setup(name, "material", GlobalVars.materials[name].ID);
			row.Name = name.ToString();
			rowVboxContainer.AddChild(row);
		}
		
		foreach(int id in towers)
		{
			//instantiate
			ShopRow row = shopRow.Instantiate<ShopRow>();
			row.Setup(GlobalVars.machines[id].Name, "machine", id);
			row.Name = GlobalVars.machines[id].Name.ToString();
			rowVboxContainer.AddChild(row);
		}
	}

}
