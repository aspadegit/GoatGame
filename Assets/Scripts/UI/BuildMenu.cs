using Godot;
using System;
using System.Collections.Generic;

//TODO: sorting goat menu
public partial class BuildMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	Button cancelButton;
	Label statsLabel;
	Label materialsLabel;
	TextureButton confirmButton;
	PackedScene machineRow = GD.Load<PackedScene>("res://Scenes/UI/RowEntries/BuildMachineRow.tscn");
	VBoxContainer machineContainer;
	Machine currentInfoShowing = null;
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

		statsLabel = GetNode<Label>("MenuMargin/MainVBox/MainHBox/StatsMargin/StatsVBox/StatsLabel");
		materialsLabel = GetNode<Label>("MenuMargin/MainVBox/MainHBox/MaterialsMargin/MaterialsVBox/MaterialsLabel");

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
		
		//there were no machines instantiated
		if(machineContainer.GetChildren().Count < 1)
		{
			statsLabel.Hide();
			materialsLabel.Hide();
		}
		//machines were instantiated, set the data to automatically be the first machine in the list
		else
		{
			SetMachineInfoToFirst();
		}
	}

	private void SetRowInformation(Machine machine, Node row)
	{
		//TODO: add images
		Label nameLabel = row.GetNode<Label>("RowMargin/RowHBox/MachineName");
		Label amountInStock = row.GetNode<Label>("RowMargin/RowHBox/AmountVBox/StockLabel");
		TextureButton upButton = row.GetNode<TextureButton>("RowMargin/RowHBox/UpButton");
		TextureButton downButton = row.GetNode<TextureButton>("RowMargin/RowHBox/DownButton");
		
		//0 if it doesn't exist in inventory already
		int amtInStock = GlobalVars.machineInventory.ContainsKey(machine.ID) ? GlobalVars.machineInventory[machine.ID] : 0;

		nameLabel.Text = machine.Name;
		amountInStock.Text = "(" + amtInStock + " in stock)";
		row.Name = machine.ID.ToString();

		Action showInfo =  new Action(() => {ShowMachineInformation(machine);});

		//mousing over row updates information
		row.Connect("mouse_entered", Callable.From(showInfo));

		//buttons also show row information (for controller support later)
		upButton.FocusEntered += showInfo;
		upButton.MouseEntered += showInfo;
		downButton.FocusEntered += showInfo;
		downButton.MouseEntered += showInfo;
		
		//TODO: BUTTONS WHEN .PRESSED

	}

	private void ShowMachineInformation(Machine machine)
	{
		//new information to update
		if(currentInfoShowing == null || machine != currentInfoShowing)
		{
			statsLabel.Text = machine.GetStatString();

			string matStr = "";
			// produces "Name: currentAmount/neededAmount\n"
			foreach(KeyValuePair<string,int> materialPair in machine.CraftingRecipe.RequiredItems)
			{
				Material mat = GlobalVars.materials[materialPair.Key];
				matStr += mat.Name;
				matStr += ": ";

				//0 if key doesn't exist, otherwise the amount we have
				int currentAmt = GlobalVars.materialsObtained.ContainsKey(mat.Name) ? GlobalVars.materialsObtained[mat.Name] : 0;
				matStr += currentAmt;
				matStr += "/";
				matStr += materialPair.Value;
				matStr += "\n";
			}

			materialsLabel.Text = matStr;

		}
	}

	private void SetMachineInfoToFirst()
	{
		statsLabel.Show();
		materialsLabel.Show();

		int ID;
		if(Int32.TryParse(machineContainer.GetChild(0).Name, out ID))
		{
			Machine machine = GlobalVars.machines[ID];
			ShowMachineInformation(machine);
		}
		else
		{
			GD.PrintErr("ID " + ID + " doesn't exist / couldn't be parsed as an int for machines in the Build Menu.");
		}
	}

}
