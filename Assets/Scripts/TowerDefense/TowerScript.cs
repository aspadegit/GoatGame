using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class TowerScript : Node2D
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	public float coneAngle = 90; //degrees

	[Export]
	public float coneLength = 50;

	Timer shotTimer;
	
	Machine machine;

	//enemies get added when they enter shooting collider, get taken off when they exit
	List<Enemy> enemies;
	int targetedIndex = -1; 

	CollisionPolygon2D coneOfAttack;
	ShotScript shotScript;

	public override void _Ready()
	{
		coneOfAttack = GetNode<CollisionPolygon2D>("AttackArea/Cone");
		shotTimer = GetNode<Timer>("ShotTimer");
		shotScript = GetNode("Area2D/ShotAnchor") as ShotScript;

		//ConeMath();
	}

	public void Setup(Machine machine)
	{
		this.machine = machine;
		shotTimer.WaitTime = machine.FireRate;
		enemies = new List<Enemy>();
		shotScript.shotType = machine.ShotType;
		shotScript.Setup();
		shotScript.aoeArea.SetDeferred(Area2D.PropertyName.Scale, new Vector2(machine.ShotType.AoeRange,machine.ShotType.AoeRange));

	}

	//Currently unused; kept around in case we need a cone, but i've switched it to be a circle
		// is actually a capsule polygon for future flexibility
	private void ConeMath()
	{

		/*
			|\   <-- angle from here
			|  \
			|    \
			|______\
		*/

		//calculate the opposite side for half the angle 
		float tangent = MathF.Tan(coneAngle/2 * (MathF.PI/180));
		float adjacent = coneLength;			//length of Y 
		float oppositeSide = tangent*adjacent;	//length of X from centerline

		Vector2 leftPoint = new Vector2(-1*oppositeSide, coneLength);
		Vector2 rightPoint = new Vector2(oppositeSide, coneLength);
		
		Vector2[] points = {Vector2.Zero, leftPoint, rightPoint};
		coneOfAttack.SetDeferred(CollisionPolygon2D.PropertyName.Polygon, points);
	}

	// use this and not process to update the polygon if ever necessary
	public override void _PhysicsProcess(double delta)
	{
		if(enemies.Count > 0 && shotTimer.IsStopped())
		{
			Shoot();
			shotTimer.Start();
		}
	
	}

	private void Shoot()
	{
		List<Enemy> shotEnemies = machine.ShotType.GetShotEnemies(enemies, shotScript.enemiesInAoe, 0);
		shotScript.Shoot();	//shooting animation

		foreach(Enemy e in shotEnemies)
		{
			e.TakeDamage(machine.ShotType.Damage);
		}
	}

	private void PrintEnemies()
	{
		GD.Print("Enemies in " + Name + ": ");
		foreach(Enemy e in enemies)
		{
			GD.Print(e.Name);
		}
	}

	//============================ Signal connections ========================
	private void OnShotTimerEnd()
	{
		if(enemies.Count > 0)
		{
			Shoot();
		}
		else
		{
			shotTimer.Stop();
		}
	}

	//NOTE: since towers only scan on layer 3, enemies should ONLY be on layer 3, so this should always work
		//if there are errors it means that either the enemy's area2D got moved, or the mask/collision numbers changed
	
	private void AreaEntered(Area2D area)
	{
		Enemy newEnemy = area.GetParent<Enemy>();

		//TODO: adjust targetedIndex potentially to not always be 0?

		//first enemy arrives
		if(enemies.Count < 1)
		{
			targetedIndex = 0;
		}
		enemies.Add(newEnemy);
		shotScript.UpdateEnemies(enemies, targetedIndex);

	}

	private void AreaExited(Area2D area)
	{
		Enemy leavingEnemy = area.GetParent<Enemy>();
		enemies.Remove(leavingEnemy);

		//last enemy leaves
		if(enemies.Count < 1)
		{
			targetedIndex = -1;
		}
		shotScript.UpdateEnemies(enemies, targetedIndex);

	}
}
