using Godot;
using System;
using System.Collections.Generic;

public partial class ShotScript : Node2D
{
	//TODO: will probably handle animation
	AnimatedSprite2D sprite;
	Animation draw;
	Animation shoot;
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("ShotSprite");
		Reset();
	}

	public void Shoot()
	{
		sprite.Animation = "shoot";
		sprite.Play();
		
	}

	public void TargetEnemy(Enemy e)
	{
		RotateSprite(e.Position);
		sprite.Animation = "draw";
		sprite.Play();
	}
	private void RotateSprite(Vector2 other)
	{
		float angleRad = Position.AngleToPoint(other);
		Rotation = angleRad;
	}

	private void Reset()
	{
		Rotation = 0;
		sprite.Animation = "draw";
		sprite.Stop();
	}

	public override void _Process(double delta)
	{
	}
}
