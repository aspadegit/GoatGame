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
	MarginContainer menu;

	[Signal]
	public delegate void TowerSelectedEventHandler();
	bool menuShowing = false;

	public override void _Ready()
	{
		TowerDefenseSignals.Instance.Connect(TowerDefenseSignals.SignalName.TowerSelect, Callable.From((int param)=> HideMainMenu()), (uint)ConnectFlags.Deferred);

	}

	private void InstantiateChildren()
	{
		
		//remove all of the old instances (TODO: not always do this)
		foreach(Node child in scrollVBox.GetChildren())
		{
			scrollVBox.RemoveChild(child);
			child.QueueFree();
		}

		//spawn in new entries
		foreach(KeyValuePair<int, int> machineEntry in GlobalVars.machineInventory)
		{
			//Get the machine
			Machine machine = GlobalVars.machines[machineEntry.Key];

			//instantiate
			TowerSelectRow row = selectRow.Instantiate<TowerSelectRow>();
			row.Setup(machine);
			row.Name = machineEntry.Key.ToString();
			scrollVBox.AddChild(row);
		}
	}

	private void ToggleMenu()
	{
		if(!menuShowing)
			ShowMainMenu();
		else
			HideMainMenu();
	}

	//TODO: (potentially), make it so that instantiating children happens once. and that numbers for
		// the reduced counts of placing machines on the tiles are updated via signal
	private void ShowMainMenu()
	{
		menuShowing = true;
		InstantiateChildren();
		menu.Show();
	}
	private void HideMainMenu()
	{
		menuShowing = false;
		menu.Hide();
	}

}
