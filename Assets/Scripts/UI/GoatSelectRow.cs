using Godot;
using System;

public partial class GoatSelectRow : Control
{
	Button selectButton;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		try{
			selectButton = GetNode<Button>("GoatSelectMargin/GoatSelectHBox/SelectMargin/SelectButton");
			selectButton.Pressed += SelectGoat;
		}
		catch(InvalidCastException e)
		{
			GD.PrintErr(e);
		}
		
	}

	private void SelectGoat()
	{
		//bring up job select menu
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
