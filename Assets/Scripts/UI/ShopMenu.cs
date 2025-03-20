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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SignalHandler.Instance.Connect(SignalHandler.SignalName.ShowShopMenu, Callable.From(()=> OnShow()), (uint)ConnectFlags.Deferred);

		InstantiateRows();

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
