using Godot;
using System;

public partial class Enemy : Node2D
{
	private PathFollow2D pathFollow;
	private AnimatedSprite2D animation;
	private Vector2 prevPosition;
	bool shouldStart = false;

	public override void _Ready()
	{
		animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public void Setup(PathFollow2D enemyPathFollow, int enemyNum)
	{
		//set up variables
		pathFollow = enemyPathFollow;
		pathFollow.ProgressRatio = 0;
		Position = pathFollow.Position;
		prevPosition = Position; 

		//set names
		Name = Name + "_" + enemyNum;
		pathFollow.Name = pathFollow.Name + "_" + enemyNum;

		shouldStart = true;
	}

	public override void _Process(double delta)
	{
		if(shouldStart)
		{
			prevPosition = Position;	//set old position

			//set new position
			pathFollow.ProgressRatio += 0.05f * (float)delta;
			Position = pathFollow.Position;
			AdjustAnimation();
		}
	}

	//changes the animation based on the instantaneous velocity 
	private void AdjustAnimation()
	{
		Vector2 velocity = Position - prevPosition;

		if(velocity.Length() > 0)
			animation.Play();
		else
			animation.Stop();

		//left/right
		if(Math.Abs(velocity.X) > Math.Abs(velocity.Y))
		{
			animation.Animation = "walk_side";
			//right
			if(velocity.X > 0)
			{
				animation.FlipH = false;
			}
			//left
			else
			{
				animation.FlipH = true;
			}

		}
		//up/down
		else
		{
			animation.FlipH = false;
			//down
			if(velocity.Y > 0)
			{
				animation.Animation = "walk_down";
			}
			//up
			else
			{
				animation.Animation = "walk_up";

			}
		}
	}

	//deletes itself and its path
	private void Destroy()
	{
		pathFollow.QueueFree();
		QueueFree();

	}

	//destroy when no longer visible
	private void OnVisibleOnScreenNotifier2DScreenExited()
	{
		Destroy();
	}
}
