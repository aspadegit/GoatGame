using Godot;
using System;

public partial class GameOver : Control
{

	[Signal]
	public delegate void GameOverSignalEventHandler();

	private void _on_retry_button_pressed()
	{
		EmitSignal(SignalName.GameOverSignal);
		GetNode<Node>("../..").GetTree().Paused = false;
		GetNode<Node>("../..").GetTree().ReloadCurrentScene();
	}
	
	private void _on_menu_button_pressed()
	{
		GetNode<Node>("../..").GetTree().ChangeSceneToFile("res://Scenes/TestScene.tscn");
	}
}






