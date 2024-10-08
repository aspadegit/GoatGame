using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public partial class YesNoButton : MarginContainer
{
	//TODO: Idea for more dialogues
		//have the dialogue after a choice be ANOTHER json file to load
		//indicate it somehow, otherwise its just a string for a final thing to do

	// Called when the node enters the scene tree for the first time.
	Button yesButton;
	Button noButton;
	string[] param;
	public override void _Ready()
	{
		SignalHandler.Instance.Connect(SignalHandler.SignalName.ShowYesNo, Callable.From((string[] sentParam)=> OnYesNoShow(sentParam)), (uint)ConnectFlags.Deferred);
		yesButton = GetNode<Button>("ButtonVBox/YesButtonMargin/YesButton");
		noButton = GetNode<Button>("ButtonVBox/NoButtonMargin/NoButton");

		yesButton.Pressed += yesButtonPressed;
		noButton.Pressed += noButtonPressed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnYesNoShow(string[] sentParam)
	{
		if(!IsVisibleInTree())
		{
			Show();
			yesButton.CallDeferred("grab_focus");
			param = sentParam;
		}
	}
	private void yesButtonPressed()
	{
		ButtonPressed(param[0]);
		Hide();

	}
	private void noButtonPressed()
	{
		ButtonPressed(param[1]);
		Hide();
	}

	private void ButtonPressed(string toCheck)
	{
		//check if it is a command
		if(toCheck.Length > 0 && toCheck[0] == '<')
		{
			//it was in fact code
			MatchCode(toCheck);
			Hide();	//TODO: potential to not always hide() when running a command
		}
		//not a command
		else
		{
			string[] toPass = {toCheck};
			//emit the next line(s) of the code
			SignalHandler.Instance.EmitSignal(SignalHandler.SignalName.ContinueTextbox, toPass);
		}
		
	}

	// checks if the passed in string is <emit>formatted like this</emit> and returns whether that was the case
	private bool MatchCode(string toSearch)
	{
		RegEx regex = new RegEx();
		regex.Compile("<emit>.*<\\/emit>");

		//TODO: error handling when the signal name does not exist
		RegExMatch result = regex.Search(toSearch);
		if(result.GetStart() != -1)
		{
			List<string> emitParams = new List<string>();
			//starting at the 3rd entry, until you've discovered all of the emit parameters
			for(int i = 2; i < param.Length; i++)
			{
				emitParams.Add(param[i]);
			}
			string emitName = toSearch.Substring(6, toSearch.Length-13);

			//emit the signal, based on whether or not it has emit parameters
			if(emitParams.Count > 0)
				SignalHandler.Instance.EmitSignal(emitName, emitParams.ToArray<string>());
			else
				SignalHandler.Instance.EmitSignal(emitName);

			//emit hide textbox (passing in an empty string should make it close. hacky way but it's fine)
			SignalHandler.Instance.EmitSignal(SignalHandler.SignalName.ShowTextbox, "");

			return true;
		}	

		return false;
	}
}
