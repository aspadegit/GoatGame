using Godot;
using System;
using System.Collections.Generic;
public partial class TowerSelectMenu : Control
{
	[Export]
	VBoxContainer scrollVBox;

	[Export]
	PackedScene selectRow;

	[Export]
	Texture2D tempTex; //TODO: DELETE ME

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InstantiateChildren();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private void InstantiateChildren()
	{
		
		//remove all of the old goat instances (TODO: not always do this)
		foreach(Node child in scrollVBox.GetChildren())
		{
			scrollVBox.RemoveChild(child);
			child.QueueFree();
		}

		//spawn in new goat buttons
		foreach(KeyValuePair<int, int> machineEntry in GlobalVars.machineInventory)
		{
			//Get the machine
			Machine machine = GlobalVars.machines[machineEntry.Key];

			//instantiate
			TowerSelectRow row = selectRow.Instantiate<TowerSelectRow>();
			row.Setup(new string[]{machine.Name, machine.Damage.ToString(), machineEntry.Value.ToString()}, new Texture2D[]{tempTex});
			row.Name = machineEntry.Key.ToString();
			scrollVBox.AddChild(row);
		}
	}
}
