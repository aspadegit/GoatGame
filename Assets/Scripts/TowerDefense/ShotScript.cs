using Godot;
using System;
using System.Collections.Generic;

public partial class ShotScript : Node2D
{
	[Export]
	public Area2D aoeArea;

	public Shot shotType;

	//TODO: handle AOE targetting
	AnimatedSprite2D sprite;
	Animation draw;
	Animation shoot;
	Enemy targetedEnemy = null;
	public List<Enemy> enemies; // enemies in red circle; the range of where to hit
	public List<Enemy> enemiesInAoe;	//enemies in smaller purple circle; all enemies that can be hit at once

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("ShotSprite");
		enemiesInAoe = new List<Enemy>();
		FullReset();
	}

	public void Shoot()
	{
		sprite.Animation = "shoot";
		if(!sprite.IsPlaying())
			sprite.Play();

		aoeArea.GetNode<CpuParticles2D>("ExplosionParticles").Emitting = true;
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

	private void MoveAoeCollider(Vector2 enemyPos)
	{
		aoeArea.SetDeferred("global_position", enemyPos);
	}

	private void OnEnemyInShotCollider(Area2D enemy)
	{
		enemiesInAoe.Add(enemy.GetParent<Enemy>());
	}

	private void OnEnemyLeavingShotCollider(Area2D enemy)
	{
		enemiesInAoe.Remove(enemy.GetParent<Enemy>());
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
		aoeArea.SetDeferred("position", new Vector2(0,32));

	}

	public override void _PhysicsProcess(double delta)
	{
		//constantly rotate to target the enemy
		if(targetedEnemy != null)
		{
			RotateSprite(targetedEnemy.Position, delta);
			MoveAoeCollider(targetedEnemy.GlobalPosition);
		}
		//no enemy to target, reset if not resetted
		else if(Rotation != 0)
		{
			ResetRotation(delta);
		}
	}
}
