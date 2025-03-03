using Godot;
using Godot.Collections;
using System;

public partial class FootstepPlayer : AudioStreamPlayer2D
{

	[Export]
	AudioStreamOggVorbis[] grass;

	[Export]
	AudioStreamOggVorbis[] snow;

	[Export]
	AudioStreamOggVorbis[] carpet;

	private string curTerrain;
	
	private AudioStreamOggVorbis[] activeStream;

	private TileMap tileMap;
	private float additionalPitchModulator = 1.0f;

	const int GROUND_LAYER = 0;

	public override void _Ready()
	{
		activeStream = grass;
		SignalHandler.Instance.Connect(SignalHandler.SignalName.AnnounceTilemap, Callable.From((TileMap t)=> SetTileMap(t)), (uint)ConnectFlags.Deferred);

	}

	private void SetTileMap(TileMap t)
	{
		tileMap = t;
	}

	// connected to AnimatedSprite2D's "animationStep" signal via the UI
	public void OnAnimationStep()
	{
		DetermineStream();
		PlayStep();
	}


	private void DetermineStream()
	{
		// get the tile that the player is on
	 	Vector2I mapPos = tileMap.LocalToMap(GlobalPosition);
		TileData tileData = tileMap.GetCellTileData(GROUND_LAYER, mapPos);

		// get the terrain name of that tile
		string terrainName = tileMap.TileSet.GetTerrainName(tileData.TerrainSet, tileData.Terrain);

		// the terrain hasn't changed, ignore the rest of this
		if(terrainName == curTerrain)
			return;

		curTerrain = terrainName;
		terrainName = terrainName.ToLower();
		additionalPitchModulator = 1.0f;

		switch(terrainName)
		{
			case "grass":
				activeStream = grass;
				break;
			case "path":
				activeStream = grass;
				additionalPitchModulator = 0.25f;
				break;
			default:
				activeStream = grass;
				break;
		}

	}

	private void PlayStep()
	{

		int index = (int)(GD.Randi() % activeStream.Length);
		float randPitch = (float)GD.RandRange(0.5, 1.5);

		Stream = activeStream[index];
		PitchScale = randPitch * additionalPitchModulator;
		Play();
		
	}
	
}
