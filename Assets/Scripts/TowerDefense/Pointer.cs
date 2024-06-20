using Godot;
using System;

public partial class Pointer : Sprite2D
{
	Vector2 pointerSize;
	private Vector2 velocity;
	private Vector2 ScreenSize;
	bool usingMouse = true;
	[Export]
	public int pointerSpeed = 300;

	[Export]
	Camera2D camera {get; set;}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AtlasTexture pointerTex = (AtlasTexture)Texture;
		pointerSize =  pointerTex.Region.Size;
		ScreenSize = GetViewportRect().Size;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//move the pointer around
		SetCursorPosition(delta, GetGlobalMousePosition());

	}

	//if we detect mouse movement in the game then we're back to using the mouse
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			usingMouse = true;
		}

	}
	private void SetCursorPosition(double delta, Vector2 mousePos)
	{
		velocity = Vector2.Zero; // The player's movement vector.

		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
		}
		
		if (Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
		}

		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
		}

		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
		}

		//use the movement options to move the cursor
		if (velocity.Length() > 0)
		{
			usingMouse = false;
			velocity = velocity.Normalized() * pointerSpeed;
			Position += velocity * (float)delta;
		}
		//use the mouse to move the cursor (no movement from the arrow keys)
		else if(usingMouse)
		{
			Position = mousePos;
		}

		//clamp the movement
		float endScreenX = (ScreenSize.X/camera.Zoom.X)-(pointerSize.X/camera.Zoom.X);
		float endScreenY = (ScreenSize.Y/camera.Zoom.Y)-(pointerSize.Y/camera.Zoom.Y);

		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, endScreenX),
			y: Mathf.Clamp(Position.Y, 0, endScreenY)
		);
	}
}
