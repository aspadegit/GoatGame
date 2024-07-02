using Godot;
using System;
using System.Collections.Generic;

public partial class TowerSelectRow : RowScript
{
	public Machine machine;
	[Export]
	Texture2D temp; //TODO: DELETE ME

	public override void Setup<T>(T genericMachine)
	{
		machine = genericMachine as Machine;
		ButtonSetup();

		//name, damage, amount
		string[] text = {machine.Name, machine.ShotType.Damage.ToString(), GlobalVars.machineInventory[machine.ID].ToString()};
		base.Setup(text, new Texture2D[]{temp}); //TODO: make this come from somewhere
		
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
	}

	private void ClickRow()
	{
		TowerDefenseSignals.Instance.EmitSignal(TowerDefenseSignals.SignalName.TowerSelect, machine.ID);
	}

	
}
