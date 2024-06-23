using Godot;
using System;
using System.Collections.Generic;

public partial class TowerSelectRow : RowScript
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void Setup(string[] labelTexts, Texture2D[] textures)
	{
		string[] rowButtonStr = {"RowButton", "pressed"};
		buttonActions.Add(rowButtonStr, ClickRow);	//inherited from RowScript

		base.Setup(labelTexts, textures);	
	}

	private void ClickRow()
	{
		GD.Print("clicked " + Name);
	}

	
}
