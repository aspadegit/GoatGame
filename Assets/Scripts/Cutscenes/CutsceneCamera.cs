using Godot;
using System;
using System.Collections.Generic;

public partial class CutsceneCamera : Camera2D
{
	// Called when the node enters the scene tree for the first time.

	private KeyValuePair<int, Node> currentFocus = new KeyValuePair<int, Node>(); // <ActorID, Node>
	private KeyValuePair<int, Node> prevFocus = new KeyValuePair<int, Node>();

	public override void _Ready()
	{
	}

	public void SetUp(float zoom, Vector2 offset, Vector2 position, KeyValuePair<int, Node> focus = default(KeyValuePair<int,Node>))
	{
		Zoom = new Vector2(zoom, zoom);
		Offset = offset;

		// set position, if it was specified
		if(position.Length() > 0)
			GlobalPosition = position;

		// set focus, if it was specified
		if(!focus.Equals(default(KeyValuePair<int,Node>)))
		{
			currentFocus = focus;
			Reparent(focus.Value, false);
		}
	}

	private void ChangeFocus(KeyValuePair<int, Node> newFocus)
	{
		prevFocus = currentFocus;
		currentFocus = newFocus;
	}

	public void Pan()
	{
		throw new NotImplementedException();

	}

	public void ChangeZoom()
	{
		throw new NotImplementedException();

	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
