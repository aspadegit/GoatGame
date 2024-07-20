using Godot;
using System;

public partial class TowerUIConnector : Node
{
	
	[Export]
	public Control pauseMenu;
	
	[Export]
	public Node2D towerTest;

	[Signal]
	public delegate void PauseGameEventHandler();

	bool paused = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("escape")){
			PauseMenu();
		}
	}
	
	public void PauseMenu(){
		
		paused = !paused;
		
		// if not paused
		if (paused == false){
			pauseMenu.Hide();
			towerTest.GetTree().Paused = false;
		}
		// if its paused
		else{
			pauseMenu.Show();
			towerTest.GetTree().Paused = true;
			EmitSignal(SignalName.PauseGame);
		}

		
	}
}
