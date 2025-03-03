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

	//emitted in the inventory menu when a row is hovered over
	[Signal]
	public delegate void OnInventoryHoverEventHandler(InventoryRow row);

	//uses an array for ease of other signals needing parameters, but path should only be [0] in there
	[Signal]
	public delegate void ChangeSceneEventHandler(string[] scenePath);

	[Signal]
	public delegate void AnnounceTilemapEventHandler(TileMap tileMap);

	public override void _Ready() {
		Instance = this;
	}


}
