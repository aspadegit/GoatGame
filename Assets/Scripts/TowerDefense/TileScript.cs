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

	[Export]
	public Lives livesEnemyCounter;
	const int selectTileSourceNum = 3;
	Vector2I prevSelection = Vector2I.Zero;
	TileMap tileMap;
	Enemy enemy;
	Node2D enemies;
	Path2D pathParent;
	Node2D towerParent;
	TowerPreview towerPreview;
	Godot.Collections.Array<Vector2I> placeableTiles = new Godot.Collections.Array<Vector2I>();
	Dictionary<Vector2I, TowerScript> towers = new Dictionary<Vector2I, TowerScript>();	//tracks what map tiles are used/have towers in them
	Timer enemySpawnTimer;
	int enemyNum = 0;

	Dictionary<int,int> machinesUsed = new Dictionary<int, int>(); // [id of machine, how many used]
	private Machine currentMachine;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TowerDefenseSignals.Instance.Connect(TowerDefenseSignals.SignalName.TowerSelect, Callable.From((int param)=> SetTower(param)), (uint)ConnectFlags.Deferred);


		tileMap = GetNode<TileMap>("TileMap");
		enemySpawnTimer = GetNode<Timer>("EnemySpawnTimer");
		enemies = GetNode<Node2D>("Enemies");
		pathParent = GetNode<Path2D>("EnemyPath");
		towerParent = GetNode<Node2D>("Towers");
		towerPreview = GetNode<TowerPreview>("TowerPreview");
		towerPreview.pointer = pointer;
		
		placeableTiles = tileMap.GetUsedCells(placementLayer);

		Reset();

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
		
		
		//if it is on top of a placeable tile, if the currentMachine exists, and if we have enough of that machine
		if(placeableTiles.Contains(tile) && currentMachine != null && GlobalVars.machineInventory[currentMachine.ID] > 0)
		{
			bool canFit = CheckTowerSize(tile);

			towerPreview.SetColor(canFit);

			if(Input.IsActionJustPressed("left_click") && canFit){
				PlaceTower(tile);
			}

			//towerPreview.SetColor(true);
		}
		else
		{
			towerPreview.SetColor(false);
		}
		
	}
	
	private void PlaceTower(Vector2I curTile)
	{	
	
		//TODO: set tower scene details

		//decrease global inventory
		GlobalVars.machineInventory[currentMachine.ID]--;

		//update that we've used one more of this current machine
		if(machinesUsed.ContainsKey(currentMachine.ID))
		{
			machinesUsed[currentMachine.ID]++;
		}
		//hasn't been added yet, so create the key
		else
		{
			machinesUsed.Add(currentMachine.ID, 1);
		}
		//create the tower
		TowerScript tower = towerScene.Instantiate<TowerScript>();
		towers.Add(curTile, tower);

		//convert the map position to where the tower scene will sit
		Vector2 globalPos = ToGlobal(tileMap.MapToLocal(curTile));
		tower.Position = globalPos;

		//put it in the tree
		towerParent.AddChild(tower);
		tower.Setup(currentMachine);

		if(GlobalVars.machineInventory[currentMachine.ID] <= 0)
			towerPreview.Hide();
		//tileMap.SetCell(towerLayer, curTile, 0, new Vector2I(0, 41), 0);
	
	}


	// returns whether it's good to place (true = good to place)
	private bool CheckTowerSize(Vector2I curTile)
	{
		foreach(KeyValuePair<Vector2I, TowerScript> pair in towers)
		{
			Vector2I tileToTest = pair.Key;
			int[] towerSize = pair.Value.GetSize();

			// if the tower we're holding overlaps one on the map
			bool currentOverlapsCheck = CheckTowerPairOverlap(tileToTest, curTile, towerSize);
			if(currentOverlapsCheck)
				return false;

			// if the one on the map overlaps the tower we're holding
			bool checkOverlapsCurrent = CheckTowerPairOverlap(curTile, tileToTest, currentMachine.Size);
			if(checkOverlapsCurrent)
				return false;
		}


	
		return true;
	}

	// returns whether overlapper overlaps on stationary tower, where stationary tower has size of radius size
	private bool CheckTowerPairOverlap(Vector2I stationary, Vector2I overlapper, int[] size)
	{
		bool inBoundsToRight = overlapper.X >= stationary.X && overlapper.X <= stationary.X + size[0];
		bool inBoundsToLeft = overlapper.X <= stationary.X && overlapper.X >= stationary.X - size[0];

		// note: Y increases as it goes down ( (0,0) is top left )
		bool inBoundsBelow = overlapper.Y >= stationary.Y && overlapper.Y <= stationary.Y + size[1];
		bool inBoundsAbove = overlapper.Y <= stationary.Y && overlapper.Y >= stationary.Y - size[1];

		//must overlap on X AND Y axes
		if((inBoundsToLeft || inBoundsToRight) && (inBoundsBelow || inBoundsAbove))
			return true;

		// no overlaps
		return false;
	}

	public void OnEnemySpawnTimeout()
	{
		PathFollow2D pathFollow = enemyPathFollow.Instantiate<PathFollow2D>();
		pathParent.AddChild(pathFollow);

		Enemy enemy = enemyScene.Instantiate<Enemy>();
		enemyNum++;
		enemy.Setup(pathFollow, enemyNum);
		enemies.AddChild(enemy);
		
		//no more spawning enemies, once we've hit the max
		if(enemyNum >= livesEnemyCounter.numEnemies)
		{
			enemySpawnTimer.Stop();
		}

	}

	private void SetTower(int machineID)
	{
		currentMachine = GlobalVars.machines[machineID];
		towerPreview.SetMachine(currentMachine);
		towerPreview.Show();
	}

	public void OnPause()
	{
		//makes the hover square disappear so it's not awkwardly still there on pause
		tileMap.EraseCell(hoverLayer, prevSelection);
		prevSelection = Vector2I.Zero;
	}

	public void Reset()
	{
		//restore the machines we've used back into the inventory
		foreach(KeyValuePair<int,int> pair in machinesUsed)
		{
			int id = pair.Key;
			GlobalVars.machineInventory[id] += pair.Value;
		}

		machinesUsed.Clear();

		//reset enemy spawn time
		enemySpawnTimer.Start();
		enemyNum = 0;

		//reset enemies
		foreach(Node node in enemies.GetChildren())
		{
			node.QueueFree();
		}

		//tracks which tile to erase when hovering
		prevSelection = Vector2I.Zero;

		//reset # of enemies & lives
		livesEnemyCounter.ResetText();
	}

}
