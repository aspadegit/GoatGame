using Godot;
using System;

public partial class TileMapScript : TileMap
{
	public override void _Ready()
	{
		
		CallDeferred("EmitSelf"); // waits to emit on the first frame of the game 
		
	}

	private void EmitSelf()
	{
		SignalHandler.Instance.EmitSignal(SignalHandler.SignalName.AnnounceTilemap, this);

	}
	
}
