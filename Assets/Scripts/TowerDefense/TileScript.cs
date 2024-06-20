using Godot;
using System;
using System.IO;

public partial class TileScript : Node2D
{
	[Export]
	public int placementLayer = 1;
	[Export]
	public int towerLayer = 2;
	[Export]
	public int hoverLayer = 3;

	const int selectTileSourceNum = 3;
	Vector2I prevSelection = Vector2I.Zero;
	TileMap tileMap;
	Sprite2D pointer;

	Godot.Collections.Array<Vector2I> placeableTiles = new Godot.Collections.Array<Vector2I>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		tileMap = GetNode<TileMap>("TileMap");
		pointer = GetNode<Sprite2D>("Pointer");
		DisplayServer.MouseSetMode(DisplayServer.MouseMode.Hidden);	//hide the mouse

		placeableTiles = tileMap.GetUsedCells(placementLayer);

	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		//adjust the hover tile
		Vector2I tile = tileMap.LocalToMap(pointer.Position);
		tileMap.EraseCell(hoverLayer, prevSelection);
		tileMap.SetCell(hoverLayer, tile, selectTileSourceNum, Vector2I.Zero, 0);
		prevSelection = tile;

		if(Input.IsActionJustPressed("left_click"))
			PlaceTower(tile);
	}

	private void PlaceTower(Vector2I curTile)
	{	
		//TODO: instantiate scene of tower instead
		//only set the tile if it is on top of a placeable tile
		if(placeableTiles.Contains(curTile))
			tileMap.SetCell(towerLayer, curTile, 0, new Vector2I(0, 41), 0);

	}

	

}
