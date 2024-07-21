using Godot;
using System;

public partial class WinLevel : Control
{
	[Signal]
	public delegate void GameOverSignalEventHandler();

	private void _on_retry_button_pressed()
	{
		EmitSignal(SignalName.GameOverSignal);
		GetNode<Node>("../..").GetTree().Paused = false;
		GetNode<Node>("../..").GetTree().ReloadCurrentScene();
	}
}



