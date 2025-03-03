using Godot;
using System;
using System.Linq;

public partial class PlayerAnimation : AnimatedSprite2D
{

	[Export]
	public int[] downFrames;


	[Signal]
	public delegate void animationStepEventHandler();
	private bool playSound = true;
	private bool correctFrame = false;
	public override void _Process(double delta)
	{
		correctFrame = downFrames.Contains(Frame);

		//check if the current frame is the step down
		if(IsPlaying() && playSound && correctFrame)
		{
			playSound = false;
			EmitSignal(SignalName.animationStep);	 //emit a signal so others know
		}
		else if(!correctFrame || !IsPlaying())
		{
			playSound = true;
		}
		
	}
}
