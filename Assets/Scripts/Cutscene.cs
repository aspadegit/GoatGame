using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

public partial class Cutscene : Node
{
	[Export]
	public string cutsceneJsonPath;
	public PackedScene location; 

	List<Step> cutsceneSteps;

	//CONSIDER: PackedScenes are stored in the json data...
		// for "flashbacks" and scene changes
		// slower to load
		// could have "standard" location loaded in RAM (PackedScene)
		// and "flashback" / separate locations have to load in
		// considering many modern games have to load for flashbacks, this is probably fair

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//TODO: set location
	}

	private void DecodeJson()
	{
		//reads in the file
		FileAccess file = FileAccess.Open(GlobalVars.cutscenePath + cutsceneJsonPath, FileAccess.ModeFlags.Read);
		string fileText = file.GetAsText();
		file.Close();

		//converts the file to json
	   	JsonNode jsonNode = JsonNode.Parse(fileText)!;


	}

	private void RunDialogue(string[] name, string[] text)
	{

	}

	private void DecodeAction()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// if (time < nextTime (set by Length of Step))
			// next Step in cutscene
			
	}

	public void StartCutscene()
	{
		location.Instantiate();

		// load actors

		// begin cutscene
	}
	

	public class Step {
		public float Length {get; set;}
	}

	public class Action : Step {
		public Action()
		{

		}
	}
}
