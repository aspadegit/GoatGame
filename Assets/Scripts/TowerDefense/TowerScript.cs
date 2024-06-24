using Godot;
using System;

public partial class TowerScript : Node2D
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	public float coneAngle = 90; //degrees

	[Export]
	public float coneLength = 50;

	CollisionPolygon2D coneOfAttack;
	public override void _Ready()
	{
		coneOfAttack = GetNode<CollisionPolygon2D>("AttackArea/Cone");
		//ConeMath();
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
        base._PhysicsProcess(delta);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
