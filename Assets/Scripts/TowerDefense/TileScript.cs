using Godot;
using System;

public partial class TileScript : TileMap
{
	[Export]
	public int placementLayer = 1;

	[Export]
	public int selectionLayer = 2;
	int gridSize = 16;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		GD.Print(GetUsedCells(placementLayer));
	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
