using Godot;
using System;
using System.Collections.Generic;

public partial class CutsceneCamera : Camera2D
{
	// Called when the node enters the scene tree for the first time.

	private KeyValuePair<int, Node> currentFocus = new KeyValuePair<int, Node>(); // <ActorID, Node>
	private KeyValuePair<int, Node> prevFocus = new KeyValuePair<int, Node>();

	private List<CutsceneActor> actors;

	bool continuous = false;

	[Export]
	 Timer timer;

	delegate void CurrentMethod(object[] parameters);
	CurrentMethod m;
	object[] parameters;

	public override void _Ready()
	{
	}

	public void SetUp(float zoom, Vector2 offset, Vector2 position, List<CutsceneActor> actors, KeyValuePair<int, Node> focus = default(KeyValuePair<int,Node>))
	{
		Zoom = new Vector2(zoom, zoom);
		Offset = offset;
		this.actors = actors;

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
		//focus on nothing
		if(newFocus.Key == -1 && currentFocus.Key != -1)
		{
			// parent/actors/actor (so we need the grandparent)
			Reparent(currentFocus.Value.GetParent().GetParent(), true);
		}
		//reparent to someone else
		else if(newFocus.Key != -1)
		{
			prevFocus = currentFocus;
			currentFocus = newFocus;
			Reparent(newFocus.Value, true);
		}
		
	}

	public void Pan(float length, float[] newPosition, int focus)
	{
		if(focus == -1)
		{	
			//when we have no parent we want to use globalposition (since it should be the same as position)	
			parameters = new object[]{ newPosition[0], newPosition[1], GlobalPosition.X, GlobalPosition.Y};

			ChangeFocus(new KeyValuePair<int, Node>(-1, null));
		}	
		else
		{

			if(focus < actors.Count)
				ChangeFocus(new KeyValuePair<int, Node>(focus, actors[focus]));		
			else
				throw new IndexOutOfRangeException("CutsceneCamera.cs: Actors List does not contain index " + focus);

			// we want to use the starting position as the one after the change
			parameters = new object[]{ newPosition[0], newPosition[1], Position.X, Position.Y};

		}

		m = Pan;
		continuous = true;
		timer.Start(length);
		timer.OneShot = true;

	}

	// { new pos[0], new pos [1], starting pos [0], starting pos [1]}
	private void Pan(object[] parameters)
	{
		if(timer.TimeLeft > 0)
		{
			float progress = (float)(1.0 - (timer.TimeLeft / timer.WaitTime));

			float[] newPos = new float[]{(float)parameters[0], (float)parameters[1]};
			float[] oldPos = new float[]{(float)parameters[2], (float)parameters[3]};

			float[] amount = new float[]{newPos[0]-oldPos[0], newPos[1]-oldPos[1]};

			float progressX = progress*amount[0];
			float progressY = progress*amount[1];
			Position = new Vector2(oldPos[0]+ progressX, oldPos[1] + progressY);

		}
		else
		{
			continuous = false;
		}
	}

	public void OnTimeout()
	{
		continuous = false;
	}

	public void ChangeZoom()
	{
		throw new NotImplementedException("Function ChangeZoom in CutsceneCamera.cs is not yet implemented.");

	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(continuous)
		{
			m.Invoke(parameters);
		}
	}
}
