using Godot;
using System;
using System.Collections.Generic;
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
	public PackedScene enemyScene;
	[Export]
	public PackedScene enemyPathFollow;
	[Export]
	public PackedScene towerScene;

	[Export]
	public Pointer pointer;


	const int selectTileSourceNum = 3;
	Vector2I prevSelection = Vector2I.Zero;
	TileMap tileMap;
	Enemy enemy;
	Node2D enemies;
	Path2D pathParent;
	Node2D towerParent;
	Godot.Collections.Array<Vector2I> placeableTiles = new Godot.Collections.Array<Vector2I>();
	Dictionary<Vector2I, TowerScript> towers = new Dictionary<Vector2I, TowerScript>();	//tracks what map tiles are used/have towers in them
	Timer enemySpawnTimer;
	int enemyNum = 0;


	private Machine currentMachine;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SignalHandler.Instance.Connect(SignalHandler.SignalName.TowerSelect, Callable.From((int param)=> SetTower(param)), (uint)ConnectFlags.Deferred);

		tileMap = GetNode<TileMap>("TileMap");
		enemySpawnTimer = GetNode<Timer>("EnemySpawnTimer");
		enemies = GetNode<Node2D>("Enemies");
		pathParent = GetNode<Path2D>("EnemyPath");
		towerParent = GetNode<Node2D>("Towers");

		placeableTiles = tileMap.GetUsedCells(placementLayer);
		enemySpawnTimer.Start();
		enemyNum = 0;

	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//enemy.SetPosition();
		//adjust the hover tile
		Vector2I tile = tileMap.LocalToMap(ToLocal(pointer.positionWithCamera));
		tileMap.EraseCell(hoverLayer, prevSelection);
		tileMap.SetCell(hoverLayer, tile, selectTileSourceNum, Vector2I.Zero, 0);
		prevSelection = tile;

		if(Input.IsActionJustPressed("left_click"))
			PlaceTower(tile);
	}

	private void PlaceTower(Vector2I curTile)
	{	
		//only set the tile if it is on top of a placeable tile, if the currentMachine exists, and if we have enough of that machine
		if(placeableTiles.Contains(curTile) && currentMachine != null && GlobalVars.machineInventory[currentMachine.ID] > 0)
		{
			//TODO: set tower scene details
			GD.Print("placed " + currentMachine.Name);

			//decrease global inventory
			GlobalVars.machineInventory[currentMachine.ID]--;

			//create the tower
			TowerScript tower = towerScene.Instantiate<TowerScript>();
			towers.Add(curTile, tower);

			//convert the map position to where the tower scene will sit
			Vector2 globalPos = ToGlobal(tileMap.MapToLocal(curTile));
			tower.Position = globalPos;

			//put it in the tree
			towerParent.AddChild(tower);
			
			//tileMap.SetCell(towerLayer, curTile, 0, new Vector2I(0, 41), 0);
		}
	}

	public void OnEnemySpawnTimeout()
	{
		PathFollow2D pathFollow = enemyPathFollow.Instantiate<PathFollow2D>();
		pathParent.AddChild(pathFollow);

		Enemy enemy = enemyScene.Instantiate<Enemy>();
		enemyNum++;
		enemy.Setup(pathFollow, enemyNum);
		enemies.AddChild(enemy);
		

	}

	private void SetTower(int machineID)
	{
		currentMachine = GlobalVars.machines[machineID];

	}

}
