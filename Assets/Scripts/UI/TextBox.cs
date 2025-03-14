using Godot;
using System;
using Dictionary = Godot.Collections.Dictionary;

public partial class TextBox : MarginContainer
{
	//TODO (potentially): add an arbitrary 200-500 ms or so wait as the textbox opens up to avoid the race condition
		// guaranteed to work, less buggy, but more annoying for the player
		// potentially: add an animation so it looks intentional lol

	// Called when the node enters the scene tree for the first time.

	[Export]
	Panel nameBox;
	[Export]
	Label nameLabel;
	[Export]
	Label textLabel;

	Control parent;
	Dictionary jsonDictionary;
	string jsonStartingPath = "res://Assets/Dialogue/";
	int keyPress = 0;
	int pagePosition = 0;
	int pageTotal = 1;
	string[] text;
	string[] name;
	bool canProgress = true;

	bool shouldLetPlayerMove = false;
	bool noChoiceOrEmit = true; // hacking the fact that not having a choice means player can't move when textbox is done

	public override void _Ready()
	{
		SignalHandler.Instance.Connect(SignalHandler.SignalName.ShowTextbox, Callable.From((string text)=> OnTextboxShow(text)), (uint)ConnectFlags.Deferred);
		SignalHandler.Instance.Connect(SignalHandler.SignalName.ContinueTextbox, Callable.From((string[] text)=> OnTextboxContinue(text)), (uint)ConnectFlags.Deferred);
		
		parent = GetParent<Control>();

		
	}

	public void ManualSetup(string[] name, string[] text)
	{
		this.name = name;
		this.text = text;
		ResetPageVars();
		keyPress = 2;	// keyPress continues to be kinda bad. months later. lol
		SignalHandler.Instance.EmitSignal(SignalHandler.SignalName.TogglePlayerMovement, false);
	
		Show();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if(IsVisibleInTree() && keyPress > 1)
		{
			ProgressText();
		}

		//avoiding race condition of getting the click to open & close at the same time... (sets keyPress to 2, forcing ProgressText() to wait 1 loop cycle...)
		if(canProgress && IsVisibleInTree() && (Input.IsActionJustPressed("left_click") || Input.IsActionJustPressed("interact")))
			keyPress++;

		//avoiding race conditions where showing with LMB and hiding with interact would increment keypress when hidden
		if(!IsVisibleInTree())
			keyPress = 0;
	}

	private void ProgressText()
	{
		if (canProgress && (Input.IsActionJustPressed("left_click") || Input.IsActionJustPressed("interact")))
		{
			//show next line of text, or close
			pagePosition++;
			if(pagePosition >= pageTotal)
				HideAndReset();
			else
				NextPage();
		}
	}

	private void HideAndReset()
	{
		//TODO: emit allow player mvmt (consider how this will affect cutscenes)
		if(shouldLetPlayerMove || noChoiceOrEmit)
			SignalHandler.Instance.EmitSignal(SignalHandler.SignalName.TogglePlayerMovement, true);
		parent.MouseFilter = MouseFilterEnum.Pass;	//allow clicks anywhere else
		Hide();
		keyPress = 0;
		pagePosition = 0;
	}

	private void NextPage()
	{
		//set next name
		NextName();

		//TODO: may change this if more types of choices are available...
		if(text[pagePosition] == "<choice>")
		{
			//EMIT YES NO
			SignalHandler.Instance.EmitSignal(SignalHandler.SignalName.ShowYesNo, jsonDictionary["SignalParam"]);
			canProgress = false;
			noChoiceOrEmit = false;

		}
		else
		{
			//set next text
			textLabel.Text = text[pagePosition];

		}
	}

	private void NextName()
	{
		//namebox should disappear
		if(name[pagePosition] == "")
		{
			nameBox.Hide();
			nameLabel.Text = "";
			return;
		}
	
		nameBox.Show();

		// - is only used to continue the previous name (to avoid having to type the same name a bunch)
		if(name[pagePosition] != "-")
		{
			nameLabel.Text = name[pagePosition];
		}

	}

	//sets up the textbox. returns success
	private bool OnTextboxShow(string dialoguePath)
	{
		if(dialoguePath == null || dialoguePath.Length < 1)
		{
			HideAndReset();
			return false;
		}
		//only show if it's not already shown
		if(!IsVisibleInTree())
		{
			SignalHandler.Instance.EmitSignal(SignalHandler.SignalName.TogglePlayerMovement, false);
			shouldLetPlayerMove = false;
			loadDialogue(dialoguePath);
			parent.MouseFilter = MouseFilterEnum.Stop;	//prevent clicks anywhere else
			Show();
			keyPress++;
			canProgress = true;

			if(name.Length > 0)
				nameBox.Show();

			return true;
		}

		return false;
	}

	private bool OnTextboxContinue(string[] newText)
	{
		shouldLetPlayerMove = true;

		if(newText == null || newText.Length < 1)
		{
			GD.PrintErr("Emitted text via ContinueTextbox that was was empty or null!");
			HideAndReset();
			return false;
		}
		text = newText;
		canProgress = true;
		ResetPageVars();
		return true;
	}

	private void loadDialogue(string dialoguePath)
	{
		if(dialoguePath == null || dialoguePath.Length < 1)
			HideAndReset();

		//end of dialogue path set in inspector
		//reads in the file
		FileAccess file = FileAccess.Open(jsonStartingPath + dialoguePath, FileAccess.ModeFlags.Read);
		string fileText = file.GetAsText();
		file.Close();

		//converts the file to json, then to a dictionary
		Json jsonFile = new Json();
		Error errorResult = jsonFile.Parse(fileText);

		if(errorResult == Error.Ok)
		{
			jsonDictionary = (Dictionary)jsonFile.Data;

			//examine the json
			parseDialogue();
		}
		else
		{
			GD.PrintErr("Unable to parse JSON file at " + jsonStartingPath + dialoguePath);
			HideAndReset();
		}

	}


	//get values from the json
	private void parseDialogue()
	{
		name = (string[])jsonDictionary["Name"];
		text = (string[]) jsonDictionary["Text"];

		ResetPageVars();		
	}

	private void ResetPageVars()
	{
		pagePosition = 0;
		pageTotal = text.Length;
		textLabel.Text = text[0];
		noChoiceOrEmit = true;
		shouldLetPlayerMove = false;

		//update name
		if(name.Length > 0)
		{
			nameBox.Show();
			nameLabel.Text = name[0];
		}
		else
		{
			nameBox.Hide();
		}
	}

}
