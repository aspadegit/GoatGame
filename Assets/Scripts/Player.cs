using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class Player : Area2D
{
	[Signal]
	public delegate void HitEventHandler();

	[Export]
	public int Speed {get; set;} = 100;

	public Vector2 ScreenSize;
	private Vector2 velocity;
	private Vector2 facing = new Vector2(0,0);
	private RayCast2D raycast = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		raycast = GetNode<RayCast2D>("RayCast2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		velocity = Vector2.Zero; // The player's movement vector.

		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
			facing = new Vector2(25,0);
		}
		
		if (Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
			facing = new Vector2(-25, 0);
		}

		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
			facing = new Vector2(0, 25);
		}

		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
			facing = new Vector2(0, -25);
		}



		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			animatedSprite2D.Play();
		}
		else
		{
			animatedSprite2D.Stop();
		}

		if(Input.IsActionPressed("sprint"))
		{
			Speed = 200;
			animatedSprite2D.SpeedScale = 2;
		}
		else
		{
			Speed = 100;
			animatedSprite2D.SpeedScale = 1;
		}

		
		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);

		if (velocity.X != 0)
		{
			animatedSprite2D.Animation = "walk";
			animatedSprite2D.FlipH = velocity.X < 0;
		}
		

	}

	public override void _Draw()
	{
		DrawLine(new Vector2(0,0), facing, Colors.Green, 1.0f);

	}
	public override void _PhysicsProcess(double delta)
	{
		// use global coordinates, not local to node

		raycast.TargetPosition = facing;
		GodotObject collider = raycast.GetCollider();

		//are we pressing space, the collider exists, and it can be interacted with?
		if(Input.IsActionJustPressed("interact") && collider != null && collider.HasMethod("EmitInteract"))
		{
			collider.CallDeferred("EmitInteract");
		}
		QueueRedraw();
	}

	private void OnBodyEntered(Node2D body)
	{
		Hide(); // Player disappears after being hit.
		EmitSignal(SignalName.Hit);
		// Must be deferred as we can't change physics properties on a physics callback.
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}
}
