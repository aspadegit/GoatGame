using Godot;
using System;

public partial class SignalHandler : Node
{
	public static SignalHandler Instance {get; private set;}

	//shows and sets up the textbox by taking in a json path
	[Signal]
    public delegate void ShowTextboxEventHandler(string path);

	//continues an open textbox with an additional array of text
	[Signal]
	public delegate void ContinueTextboxEventHandler(string[] text);

	//shows the yes/no textboxes in the UI node
	[Signal]
	public delegate void ShowYesNoEventHandler(string[] param);

	//shows the goat menu
	[Signal]
	public delegate void ShowGoatMenuEventHandler();
	public override void _Ready() {
		Instance = this;
	}


}
