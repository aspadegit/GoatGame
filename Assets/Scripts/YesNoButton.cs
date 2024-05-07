using Godot;
using System;

public partial class YesNoButton : MarginContainer
{
	//TODO: Idea for more dialogues
		//have the dialogue after a choice be ANOTHER json file to load
		//indicate it somehow, otherwise its just a string for a final thing to do

	// Called when the node enters the scene tree for the first time.
	TextureButton yesButton;
	TextureButton noButton;
	public override void _Ready()
	{
		SignalHandler.Instance.Connect(SignalHandler.SignalName.ShowYesNo, Callable.From(()=> OnYesNoShow()), (uint)ConnectFlags.Deferred);
		yesButton = GetNode<TextureButton>("ButtonVBox/YesButtonMargin/YesButton");
		noButton = GetNode<TextureButton>("ButtonVBox/NoButtonMargin/NoButton");

		yesButton.Pressed += yesButtonPressed;
		noButton.Pressed += noButtonPressed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnYesNoShow()
	{
		if(!IsVisibleInTree())
		{
			Show();
			yesButton.CallDeferred("grab_focus");
		}
	}
	private void yesButtonPressed()
	{
		GD.Print("Pressed yes!");
		

	}
	private void noButtonPressed()
	{
		GD.Print("Pressed no!");

	}
}
