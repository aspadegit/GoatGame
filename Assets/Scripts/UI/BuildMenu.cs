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
	Dictionary<int, int> machinesToMake;
	Dictionary<string, int> totalMaterialCost;
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
		{
			machinesToMake = new Dictionary<int, int>();
			totalMaterialCost = new Dictionary<string, int>();
			InstantiateChildren();
		}
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
		
		//when pressed, increase or decrease the amount of the machine youre making
		Action increaseAmt = new Action(() => {ChangeMachineAmount(machine, 1, row);});
		Action decreaseAmt = new Action(() => {ChangeMachineAmount(machine, -1, row);});

		upButton.Pressed += increaseAmt;
		downButton.Pressed += decreaseAmt;
	}

	private void ChangeMachineAmount(Machine machine, int amount, Node row)
	{
		//TODO: disable up button when not enough

		//check for materials
		if(amount > 0 && !CheckRecipeMaterialCost(machine.CraftingRecipe))
			return;

		//decreasing, but either we haven't added the machine yet (functionally 0) or it has been set to 0 
			//aka, you can't decrease below 0, so return here
		if(amount < 0 && (!machinesToMake.ContainsKey(machine.ID) || machinesToMake[machine.ID] == 0))
			return;

		AcquireReleaseResources(machine, amount);

		//add it if it doesnt exist
		if(!machinesToMake.ContainsKey(machine.ID))
			machinesToMake.Add(machine.ID, 0);
		
		//we're making more/less of that machine
		machinesToMake[machine.ID] += amount;

		//set it back to 0 if it goes below (shouldn't happen, but for safety)
		if(machinesToMake[machine.ID] < 0)
			machinesToMake[machine.ID] = 0;

		//update visuals
		UpdateMachineAmount(machine, row);
	}

	private void UpdateMachineAmount(Machine machine, Node row)
	{
		Label currentlyMaking = row.GetNode<Label>("RowMargin/RowHBox/AmountVBox/AmountToMake");
		currentlyMaking.Text = "x" + machinesToMake[machine.ID];
		UpdateMaterialsInformation(machine);
	}

	private void AcquireReleaseResources(Machine machine, int amount)
	{
		foreach(KeyValuePair<string,int> item in machine.CraftingRecipe.RequiredItems)
		{
			string materialName = item.Key;
			int cost = item.Value;

			//add the key if it doesn't exist
			if(!totalMaterialCost.ContainsKey(materialName))
				totalMaterialCost.Add(materialName, 0);

			//acquire resources
			if(amount > 0)
				totalMaterialCost[materialName] += cost;
			//release resources
			else
				totalMaterialCost[materialName] -= cost;

			//clamp to 0 if below, shouldn't happen but you never know...
			if(totalMaterialCost[materialName] < 0)
				totalMaterialCost[materialName] = 0;
		}

	}
	private bool CheckRecipeMaterialCost(Recipe recipe)
	{
		//loop through recipe's requirements
		foreach(KeyValuePair<string, int> item in recipe.RequiredItems)
		{
			//one of the recipe's requirements fails
			if(!CheckMaterialCost(item.Key, item.Value))
				return false;
		}
		return true;
	}
	private bool CheckMaterialCost(string materialName, int cost)
	{
		// we don't have the material at all
		if(!GlobalVars.materialsObtained.ContainsKey(materialName))
			return false;

		//get the amount we're currently using in the build menu
		int currentlyUsing = 0;
		if(totalMaterialCost.ContainsKey(materialName))
			currentlyUsing = totalMaterialCost[materialName];

		//we have enough leftover materials to make it
		if(GlobalVars.materialsObtained[materialName] - currentlyUsing >= cost)
			return true;

		return false;
	}

	private void ShowMachineInformation(Machine machine)
	{
		//new information to update
		if(currentInfoShowing == null || machine != currentInfoShowing)
		{
			statsLabel.Text = machine.GetStatString();
			UpdateMaterialsInformation(machine);

		}
	}

	private void UpdateMaterialsInformation(Machine machine)
	{
		string matStr = "";
		// produces "Name: currentAmount/neededAmount\n"
		foreach(KeyValuePair<string,int> materialPair in machine.CraftingRecipe.RequiredItems)
		{
			Material mat = GlobalVars.materials[materialPair.Key];
			matStr += mat.Name;
			matStr += ": ";

			//0 if key doesn't exist, otherwise the amount we have
			int globalAmt = 0;
			if(GlobalVars.materialsObtained.ContainsKey(mat.Name))
				globalAmt = GlobalVars.materialsObtained[mat.Name];
			
			int localAmt = 0;
			if(totalMaterialCost.ContainsKey(mat.Name))
				localAmt = totalMaterialCost[mat.Name];
			
			matStr += globalAmt - localAmt;
			matStr += "/";
			matStr += materialPair.Value;
			matStr += "\n";
		}

		materialsLabel.Text = matStr;
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
