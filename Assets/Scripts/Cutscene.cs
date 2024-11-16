using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;

public partial class Cutscene : Node
{
	[Export]
	public string cutsceneJsonPath;

	[Export]
	public Node2D actorParent;

	[Export]
	public PackedScene cutsceneActorScene;

	[Export]
	public Timer timer;

	public PackedScene location; 

	List<CutsceneActor> actors = new List<CutsceneActor>();

	List<Step> cutsceneSteps = new List<Step>();

	private bool playingCutscene = false;
	private int currentStep = 0;


	//CONSIDER: PackedScenes are stored in the json data...
		// for "flashbacks" and scene changes
		// slower to load
		// could have "standard" location loaded in RAM (PackedScene)
		// and "flashback" / separate locations have to load in
		// considering many modern games have to load for flashbacks, this is probably fair

	public override void _Ready()
	{
		//TODO: set location
		try
		{
			DecodeJson();
			playingCutscene = true; //TODO: sometihng else sets this value
			StartCutscene();
		}
		catch(Exception e)
		{
			GD.PrintErr(e.StackTrace);
			EndCutscene();
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// if (time < nextTime (set by Length of Step))
			// next Step in cutscene
			
		if(playingCutscene)
		{
			
		}
	}
	
	private void RunDialogue(string[] name, string[] text)
	{
		GD.Print("not implemented dialogue");

	}


	private void DecodeJson()
	{
		//reads in the file
		
		FileAccess file = FileAccess.Open(GlobalVars.cutscenePath + cutsceneJsonPath, FileAccess.ModeFlags.Read);
		string fileText = file.GetAsText();
		file.Close();

		//converts the file to json
		JsonNode jsonNode = JsonNode.Parse(fileText)!;

		DecodeHeader(jsonNode["Header"]);
		DecodeScene(jsonNode["Scene"].AsArray());

		
	}

	private void DecodeHeader(JsonNode header)
	{
		string sceneName = (string)header["Name"];
		string location = (string)header["Location"];

		JsonArray actors = header["Actors"].AsArray();

		foreach(JsonNode actor in actors)
		{
			DecodeActor(actor);
		}
	}

	// create an individual actor and add it to the list & tree

		//change later:
			// make scenes for actors
			// put actors into their starting positions ahead of time
			// each cutscene has its own scene, in this case
	private void DecodeActor(JsonNode actor)
	{
		//create
		CutsceneActor currentActor = cutsceneActorScene.Instantiate<CutsceneActor>();

		//set details
		currentActor.actorName = (string)actor;

		//add
		actors.Add(currentActor);
		actorParent.AddChild(currentActor);

	}

	private void DecodeScene(JsonArray scene)
	{
		foreach(JsonNode step in scene)
		{
			string type = (string)step["Type"];
			
			// action
			if(type == "Action")
			{
				DecodeAction(step);
			}
			// dialogue
			else
			{
				DecodeDialogue(step);
			}
		}	
	}

	private void DecodeAction(JsonNode action)
	{
		int actorIndex = (int)action["Actor"];	// index of actor in the list
		string func = (string)action["Function"]; // what the actor should do
		
		JsonArray jsonParams = action["Parameters"].AsArray();
		
		float length = (float)action["Length"];

		Action newAction = new Action(actors[actorIndex], func, jsonParams, length, timer);
		cutsceneSteps.Add(newAction);
	}

	private void DecodeDialogue(JsonNode dialogue)
	{
		Dialogue newDialogue = new Dialogue(RunDialogue);
		cutsceneSteps.Add(newDialogue);
	}	

	public void StartCutscene()
	{
		currentStep = 0;
		// play first step
		cutsceneSteps[0].Play();

	}
	
	// when the timer runs out
	public void TimerTimeout()
	{
		NextStep();
	}

	private void NextStep()
	{
		currentStep++;
		if(currentStep >= cutsceneSteps.Count)
		{
			EndCutscene();
		}
		else
		{
			// play next step
			cutsceneSteps[currentStep].Play();
		}
	}

	private void EndCutscene()
	{
		CleanUpActors();

		currentStep = 0;
		//TODO: other stuff
	}

	private void CleanUpActors()
	{
		//remove all actors from parent (may not need in the future)
		foreach(Node2D actor in actorParent.GetChildren())
		{
			actor.QueueFree();
		}
		
		actors.Clear();
	}


	public abstract class Step {
		public float Length {get; set;}
		public abstract void Play();
	}
	
	public class Dialogue : Step {

		public delegate void RunDialogue(string[] name, string[] text);
		RunDialogue func;
		public Dialogue(RunDialogue d)
		{
			func = d;
		}
        public override void Play()
        {
			string[] temp = {};
			func(temp,temp);
            //throw new NotImplementedException();
        }
    }
	public class Action : Step {

		CutsceneActor Actor { get; set; }
		string Function {get; set;}
		JsonArray Parameters {get; set;}

		Timer timer { get; }

		public Action(CutsceneActor actor, string func, JsonArray param, float length, Timer timer)
		{
			Actor = actor;
			Function = func;
			Parameters = param;
			Length = length;
			this.timer = timer;
		}

		public override void Play()
		{
			Actor.PlayAction(Function, Parameters, Length);
			timer.Start((double)Length);
		}
	}
}
