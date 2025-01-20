using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;

public partial class Cutscene : Node
{
	//TODO: "SimultaneousAction" Step that allows for multiple actions across different actors
	
	[Export]
	public string cutsceneJsonPath;

	[Export]
	public Node2D actorParent;

	[Export]
	public PackedScene cutsceneActorScene;

	[Export]
	public Timer timer;

	[Export]
	public Camera2D camera;

	[Signal]
	public delegate void NextStepDialogueEventHandler(string[] name, string[] text);

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
		// hacking existing textbox script
		EmitSignal(SignalName.NextStepDialogue, name, text);

		// NextStep() is called via signal when the "hidden" signal is emitted by the textbox
			// in the node menu
			// ...i just know future me is gonna forget that
				// hi it's future me. i did forget that
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

		JsonNode cameraNode = header["Camera"];
		DecodeCamera(cameraNode);
	}

	private void DecodeCamera(JsonNode cameraNode)
	{
		// set the zoom of the camera
		int zoom = (int)cameraNode["Zoom"];
		camera.Zoom = new Vector2(zoom, zoom);

		// focus is the index of the Actor to focus on
		int focus = (int)cameraNode["Focus"];

		if(focus >= 0 && focus < actorParent.GetChildren().Count)
		{
			Node focusActor = actorParent.GetChild(focus);
			camera.Reparent(focusActor);
		}

		JsonArray offset = cameraNode["Offset"].AsArray();
		if(offset.Count > 0)
		{
			camera.Offset = new Vector2((float)offset[0], (float)offset[1]);
		}

		// set the position of the camera
		JsonArray position = cameraNode["Position"].AsArray();
		if(position.Count > 0)
		{
			camera.Position = new Vector2((float)position[0], (float)position[1]);
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
		JsonArray name = dialogue["Name"].AsArray();
		JsonArray text = dialogue["Text"].AsArray();
		Dialogue newDialogue = new Dialogue(RunDialogue, name, text);
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

	public void NextStep()
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
		RunDialogue function;
		string[] name;
		string[] text;
		public Dialogue(RunDialogue function, JsonArray name, JsonArray text)
		{
			this.function = function;
			ParseJsonArr(name, out this.name);
			ParseJsonArr(text, out this.text);
		}
        public override void Play()
        {
			function(name,text);
        }

		private void ParseJsonArr(JsonArray arr, out string[] str)
		{

			str = new string[arr.Count];

			for(int i = 0; i < arr.Count; i++)
			{
				str[i] = (string)arr[i];
			}

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
