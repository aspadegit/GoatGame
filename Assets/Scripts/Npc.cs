using Godot;
using System;

public partial class Npc : Area2D
{

	[Export] 
	CollisionObject2D ThisCollider;
	public override void _Ready() {

		ThisCollider.InputEvent += Clicked;
	}

	void Clicked(Node viewport, InputEvent @event, long shapeIdx){

		if(Input.IsActionJustPressed("left_click")){

			EmitInteract();
			

		}

	}

	public void EmitInteract()
	{
		SignalHandler.Instance.EmitSignal(SignalHandler.SignalName.ShowTextbox, "testing new text");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


}
