using Godot;
using System;

public partial class SignalHandler : Node
{
	public static SignalHandler Instance {get; private set;}
	[Signal]
    public delegate void ShowTextboxEventHandler(string text);

	[Signal]
	public delegate void ShowYesNoEventHandler();
	public override void _Ready() {
		Instance = this;
	}


}
