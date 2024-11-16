using Godot;
using System;
using System.Text.Json.Nodes;

// could probably become a part of NPC scenes, later on
public partial class CutsceneActor : Node2D
{
	// Called when the node enters the scene tree for the first time.
	AnimatedSprite2D sprite;
	public string actorName;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void PlayAction(string func, JsonArray param, float length)
	{
		GD.Print("Will run function " + func + " with " + param.Count + " parameters for " + length + " seconds.");
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
