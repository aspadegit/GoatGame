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

	// shows the shop menu
	[Signal]
	public delegate void ShowShopMenuEventHandler();

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

	// true = let player move, false = stop player
	[Signal]
	public delegate void TogglePlayerMovementEventHandler(bool canMove);

	// a tilemap can announce its existence by passing itself in the parameter
	[Signal]
	public delegate void AnnounceTilemapEventHandler(TileMap tileMap);

	// sends a signal when settings in the settings menu have changed
	[Signal]
	public delegate void SettingsChangedEventHandler();


	// emitted when there is a change in TextEntry.cs (for example, when the amount entered in the shop menu changes)
	[Signal]
	public delegate void TextInputChangedEventHandler(TextEntry entry);

	public override void _Ready() {
		Instance = this;
	}


}
