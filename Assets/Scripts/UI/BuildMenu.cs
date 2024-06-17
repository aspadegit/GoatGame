using Godot;
using System;
using System.Collections.Generic;

//TODO: sorting goat menu
public partial class BuildMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	Button cancelButton;
	TextureButton confirmButton;
	PackedScene machineRow = GD.Load<PackedScene>("res://Scenes/UI/RowEntries/BuildMachineRow.tscn");
	VBoxContainer machineContainer;
	bool hasChanged = true;
	public override void _Ready()
	{
		SignalHandler.Instance.Connect(SignalHandler.SignalName.ShowBuildMenu, Callable.From(()=> OnShowBuildMenu()), (uint)ConnectFlags.Deferred);

		//set up buttons
		cancelButton = GetNode<Button>("MenuMargin/MainVBox/ButtonHBox/CancelButton");
		confirmButton = GetNode<TextureButton>("MenuMargin/MainVBox/ButtonHBox/ConfirmButton");
		cancelButton.Pressed += HideAndReset;
		confirmButton.Pressed += Confirm;

		machineContainer = GetNode<VBoxContainer>("MenuMargin/MainVBox/MainHBox/ScrollContainer/OptionsVBox");


	}

	private void OnShowBuildMenu()
	{
		Show();

		//TODO: hasChanged always true
		if(hasChanged)
			InstantiateChildren();
		cancelButton.CallDeferred("grab_focus");
	}

	private void HideAndReset()
	{
		Hide();
		//TODO: track changes. revert the changes on cancel
		//TODO: add "save and close" but not confirm button
	}

	private void Confirm()
	{
		//TODO: make the machines
		Hide();
	}

	private void InstantiateChildren()
	{
		//remove all of the old machine instances (TODO: not always do this)
		foreach(Node child in machineContainer.GetChildren())
		{
			machineContainer.RemoveChild(child);
			child.QueueFree();
		}

		//spawn in new goat buttons
		foreach(KeyValuePair<int, Machine> machineEntry in GlobalVars.machines)
		{
			//get the type of machine via the global vars' dictionary & its ID
			Machine curMachine = machineEntry.Value;

			//instantiating...
			Node row = machineRow.Instantiate();
			SetRowInformation(curMachine, row);
			machineContainer.AddChild(row);
		}
	}

	private void SetRowInformation(Machine machine, Node row)
	{
		//TODO: add images
		Label nameLabel = row.GetNode<Label>("RowMargin/RowHBox/MachineName");
		Label amountInStock = row.GetNode<Label>("RowMargin/RowHBox/AmountVBox/StockLabel");
		
		//0 if it doesn't exist in inventory already
		int amtInStock = GlobalVars.machineInventory.ContainsKey(machine.ID) ? GlobalVars.machineInventory[machine.ID] : 0;

		nameLabel.Text = machine.Name;
		amountInStock.Text = "(" + amtInStock + " in stock)";
		row.Name = machine.ID.ToString();

	}

}
