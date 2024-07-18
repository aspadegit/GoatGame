using Godot;
using System;

public partial class GameOver : Control
{
	
	private void _on_retry_button_pressed()
	{
		GetNode<Node>("../..").GetTree().ReloadCurrentScene();
	}
}



