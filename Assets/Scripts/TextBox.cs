using Godot;
using System;
using Dictionary = Godot.Collections.Dictionary;

public partial class TextBox : MarginContainer
{
	//TODO (potentially): add an arbitrary 200-500 ms or so wait as the textbox opens up to avoid the race condition
		// guaranteed to work, less buggy, but more annoying for the player
		// potentially: add an animation so it looks intentional lol

	// Called when the node enters the scene tree for the first time.
	Label textLabel;
	Control parent;
	int keyPress = 0;
	int pagePosition = 0;
	int pageTotal = 1;
	string[] text;
	bool canProgress = true;

	public override void _Ready()
	{
		SignalHandler.Instance.Connect(SignalHandler.SignalName.ShowTextbox, Callable.From((string text)=> OnTextboxShow(text)), (uint)ConnectFlags.Deferred);
		textLabel = GetNode<Label>("TextBoxForText/Panel/Text");
		parent = GetParent<Control>();
		
		
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
		parent.MouseFilter = MouseFilterEnum.Pass;	//allow clicks anywhere else
		Hide();
		keyPress = 0;
		pagePosition = 0;
	}

	private void NextPage()
	{
		if(text[pagePosition] == "<choice>")
		{
			//EMIT YES NO
			SignalHandler.Instance.EmitSignal(SignalHandler.SignalName.ShowYesNo);
			canProgress = false;
		}
		else
		{
			textLabel.Text = text[pagePosition];
		}
	}

	private void OnTextboxShow(string dialoguePath)
	{
		//only show if it's not already shown
		if(!IsVisibleInTree())
		{
			loadDialogue(dialoguePath);
			parent.MouseFilter = MouseFilterEnum.Stop;	//prevent clicks anywhere else
			Show();
			keyPress++;
			canProgress = true;
		}
	}

	private void loadDialogue(string dialoguePath)
	{
		//TODO: ERROR HANDLING

		//end of dialogue path set in inspector
		FileAccess file = FileAccess.Open("res://Assets/Dialogue/" + dialoguePath, FileAccess.ModeFlags.Read);

		string fileText = file.GetAsText();
		file.Close();

		Json jsonFile = new Json();
		jsonFile.Parse(fileText);
		Dictionary jsonDictionary = (Dictionary)jsonFile.Data;

		parseDialogue(jsonDictionary);
		textLabel.Text = text[0];


	}
	private void parseDialogue(Dictionary dialogue)
	{
		string name = (string)dialogue["Name"];
		text = (string[]) dialogue["Text"];
		pageTotal = text.Length;
		
	}

}
