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
	const float DEFAULT_DB = -3;

	bool canPlay = true;

	public override void _Ready()
	{
		activeStream = grass;
		SignalHandler.Instance.Connect(SignalHandler.SignalName.AnnounceTilemap, Callable.From((TileMap t)=> SetTileMap(t)), (uint)ConnectFlags.Deferred);
		SetVolume();
		SignalHandler.Instance.Connect(SignalHandler.SignalName.TogglePlayerMovement, Callable.From((bool toggle)=> OnToggleMovement(toggle)), (uint)ConnectFlags.Deferred);

	}

	public void OnToggleMovement(bool toggle)
	{
		//stops footsteps playing when paused
		canPlay = toggle;
	}

	private void SetTileMap(TileMap t)
	{
		tileMap = t;
	}

	// connected to AnimatedSprite2D's "animationStep" signal via the UI
	public void OnAnimationStep()
	{
		if(canPlay)
		{
			DetermineStream();
			PlayStep();
		}
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

		SetVolume();
		Play();
		
	}

	private void SetVolume()
	{
		
		float percentToDecibel = 10 * (float)Math.Log10(GlobalVars.settings.effectsVolume * GlobalVars.settings.masterVolume);
		VolumeDb = DEFAULT_DB + percentToDecibel;	//percentToDecibel is the CHANGE from the default
		
	}
	
}
