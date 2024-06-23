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

	//shows the list of jobs as they are being completed menu
	[Signal]
	public delegate void ShowPerformJobsEventHandler();

	[Signal]
	public delegate void ShowBuildMenuEventHandler();

	//a tower select row sends a signal to the tower scene on what machine ID has been clicked
	[Signal]
	public delegate void TowerSelectEventHandler(int param);

	public override void _Ready() {
		Instance = this;
	}


}
