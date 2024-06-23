using Godot;
using System;
using System.Collections.Generic;

public partial class TowerSelectRow : RowScript
{
	[Export]
	Texture2D tempSelector;  //TODO: DELETE ME

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Setup(new string[]{"test"}, new Texture2D[]{tempSelector}); //TODO: DELETE ME
	}

	public override void Setup(string[] labelTexts, Texture2D[] textures)
	{
		string[] rowButtonStr = {"RowButton", "pressed"};
		buttonActions.Add(rowButtonStr, ClickRow);

		base.Setup(labelTexts, textures);	
	}

	private void ClickRow()
	{
		GD.Print("clicked " + Name);
	}

	
}
