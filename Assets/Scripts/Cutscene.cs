using Godot;
using System;

public partial class Cutscene : Node
{
	public string cutsceneJsonPath;
	public PackedScene location;

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//TODO: set location
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void StartCutscene()
	{
		location.Instantiate();

		// set everything else invisible

		// begin cutscene
	}
}
