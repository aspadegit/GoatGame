using Godot;
using System;

public partial class CutsceneActor : Node2D
{
	// Called when the node enters the scene tree for the first time.
	AnimatedSprite2D sprite;
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	//flip horizontally
	public void Flip()
	{
		sprite.FlipH = !sprite.FlipH;
	}

	// name of animation, and for how long
	public void PlayAnimation(string name, int length)
	{

	}
}
