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

	[Export]
	public int pointerSpeed = 300;

	const int selectTileSourceNum = 3;
	Vector2I prevSelection = Vector2I.Zero;
	TileMap tileMap;
	Sprite2D pointer;
	Camera2D camera;
	private Vector2 ScreenSize;
	private Vector2 velocity;
	private Vector2 pointerSize;

	bool usingMouse = true;

	Godot.Collections.Array<Vector2I> placeableTiles = new Godot.Collections.Array<Vector2I>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		tileMap = GetNode<TileMap>("TileMap");
		pointer = GetNode<Sprite2D>("Pointer");
		camera = GetNode<Camera2D>("Camera2D");
		DisplayServer.MouseSetMode(DisplayServer.MouseMode.Hidden);	//hide the mouse
		ScreenSize = GetViewportRect().Size;

		AtlasTexture pointerTex = (AtlasTexture)pointer.Texture;
		pointerSize =  pointerTex.Region.Size;

		placeableTiles = tileMap.GetUsedCells(placementLayer);

	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//move the pointer around
		SetCursorPosition(delta, GetGlobalMousePosition());

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

	//if we detect mouse movement in the game then we're back to using the mouse
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			usingMouse = true;
		}

	}
	private void SetCursorPosition(double delta, Vector2 mousePos)
	{
		velocity = Vector2.Zero; // The player's movement vector.

		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
		}
		
		if (Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
		}

		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
		}

		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
		}

		//use the movement options to move the cursor
		if (velocity.Length() > 0)
		{
			usingMouse = false;
			velocity = velocity.Normalized() * pointerSpeed;
			pointer.Position += velocity * (float)delta;
		}
		//use the mouse to move the cursor (no movement from the arrow keys)
		else if(usingMouse)
		{
			pointer.Position = mousePos;
		}

		//clamp the movement
		float endScreenX = (ScreenSize.X/camera.Zoom.X)-(pointerSize.X/camera.Zoom.X);
		float endScreenY = (ScreenSize.Y/camera.Zoom.Y)-(pointerSize.Y/camera.Zoom.Y);

		pointer.Position = new Vector2(
			x: Mathf.Clamp(pointer.Position.X, 0, endScreenX),
			y: Mathf.Clamp(pointer.Position.Y, 0, endScreenY)
		);
	}

}
