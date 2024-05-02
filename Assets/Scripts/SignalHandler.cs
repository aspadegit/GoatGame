using Godot;
using System;

public partial class SignalHandler : Node
{
	public static SignalHandler Instance {get; private set;}
	[Signal]
    public delegate void ShowTextboxEventHandler(string text);
	public override void _Ready() {
		Instance = this;
	}


}
