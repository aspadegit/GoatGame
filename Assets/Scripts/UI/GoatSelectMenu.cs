using Godot;
using System;

public partial class GoatSelectMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	Button cancelButton;
	public override void _Ready()
	{
		SignalHandler.Instance.Connect(SignalHandler.SignalName.ShowGoatMenu, Callable.From(()=> OnShowGoatMenu()), (uint)ConnectFlags.Deferred);
		cancelButton = GetNode<Button>("MainContainer/MainVBoxContainer/TopMenu/CancelButton");
		cancelButton.Pressed += HideAndReset;

	}

	private void OnShowGoatMenu()
	{
		Show();
		cancelButton.CallDeferred("grab_focus");
	}

	private void HideAndReset()
	{
		Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
