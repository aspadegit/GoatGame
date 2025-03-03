using Godot;
using System;


public partial class Npc : Area2D
{

	[Export] 
	CollisionObject2D ThisCollider;

	[Export]
	public string dialoguePath {get; set;} = "";

	private bool hasClicked = false;

	public override void _Ready() {

		ThisCollider.InputEvent += Clicked;
	}

	void Clicked(Node viewport, InputEvent @event, long shapeIdx){

		if(!hasClicked && Input.IsActionJustPressed("left_click")){
			EmitInteract();
			hasClicked = true;
		}

		if(Input.IsActionJustReleased("left_click"))
		{
			hasClicked = false;
		}

	}

	public void EmitInteract()
	{
		SignalHandler.Instance.EmitSignal(SignalHandler.SignalName.ShowTextbox, dialoguePath);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}



}
