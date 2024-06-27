using Godot;
using System;
using System.Collections.Generic;

public partial class ShotScript : Node2D
{
	//TODO: handle AOE targetting
	AnimatedSprite2D sprite;
	Animation draw;
	Animation shoot;
	Enemy targetedEnemy = null;

	public List<Enemy> enemies;
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("ShotSprite");
		FullReset();
	}

	public void Shoot()
	{
		sprite.Animation = "shoot";
		if(!sprite.IsPlaying())
			sprite.Play();

	}

	public void OnAnimationFinish()
	{
		//if we just finished shooting, switch to preparing to shoot
		if(sprite.Animation == "shoot")
		{
			sprite.Animation = "draw";
			sprite.Play();
		}
	}

	public void UpdateEnemies(List<Enemy> newEnemies, int targetIndex)
	{
		enemies = newEnemies;
		//enemies is now empty
		if(enemies.Count < 1)
		{
			ResetNoRotate();
		}
		//there's a new targeted enemy
		else if(enemies[targetIndex] != targetedEnemy)
		{
			TargetEnemy(enemies[targetIndex]);
		}
	}

	//called the first time the current tracked enemy is targeted
	private void TargetEnemy(Enemy e)
	{
		targetedEnemy = e;
		sprite.Animation = "draw";
		if(!sprite.IsPlaying())
			sprite.Play();
	}
	private void RotateSprite(Vector2 other, double delta)
	{
		float angleRad = GlobalPosition.AngleToPoint(other);
		float angleDeg = Mathf.RadToDeg(angleRad);
		//godot considers sprites that are right facing to be the standard direction, so adjust for that with -90 degrees
		angleDeg = angleDeg - 90;

		Rotation = (float)Mathf.LerpAngle(Rotation, Mathf.DegToRad(angleDeg), delta*10);
	}

	private void FullReset()
	{
		ResetNoRotate();
		Rotation = 0;
	}

	//smoothly rotates back to starting
	private void ResetRotation(double delta)
	{
		Rotation = (float)Mathf.LerpAngle(Rotation, 0, delta*7);
	}	

	private void ResetNoRotate()
	{
		sprite.Stop();
		sprite.Animation = "draw";
		sprite.Frame = 0;
		targetedEnemy = null;
	}

	public override void _PhysicsProcess(double delta)
	{
		//constantly rotate to target the enemy
		if(targetedEnemy != null)
		{
			RotateSprite(targetedEnemy.Position, delta);
		}
		//no enemy to target, reset if not resetted
		else if(Rotation != 0)
		{
			ResetRotation(delta);
		}
	}
}
