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
	public Timer dialogueBuffer;

	[Export]
	public Camera2D camera;
	private CutsceneCamera cameraScript;

	[Signal]
	public delegate void NextStepDialogueEventHandler(string[] name, string[] text);

	public PackedScene location; 

	List<CutsceneActor> actors = new List<CutsceneActor>();

	List<Step> cutsceneSteps = new List<Step>();

	private bool playingCutscene = false;
	private int currentStep = 0;

	private string[] curDialogueName;
	private string[] curDialogueText;

	//CONSIDER: PackedScenes are stored in the json data...
		// for "flashbacks" and scene changes
		// slower to load
		// could have "standard" location loaded in RAM (PackedScene)
		// and "flashback" / separate locations have to load in
		// considering many modern games have to load for flashbacks, this is probably fair

	public override void _Ready()
	{
		cameraScript = GetNode<CutsceneCamera>("Camera");

		//TODO: set location
		try
		{
			DecodeJson();
			playingCutscene = true; //TODO: sometihng else sets this value
			StartCutscene();
		}
		catch(Exception e)
		{
			GD.PrintErr(e.Message);
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
		dialogueBuffer.Start();
		curDialogueName = name;
		curDialogueText = text;
		
	}
	// to avoid race conditions when showing the dialogue box
		//TODO: add a "box opening" animation to hide this wait (and avoid flashing)
	public void DialogueBufferTimeout()
	{
		// hacking existing textbox script
		EmitSignal(SignalName.NextStepDialogue, curDialogueName, curDialogueText);
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
		JsonArray startingPos = header["StartingPos"].AsArray();

		if(actors.Count != startingPos.Count)
			throw new IndexOutOfRangeException("Length of actors in " + cutsceneJsonPath + " does not equal the length of starting positions.");

		for(int i = 0; i < actors.Count; i++)
		{
			DecodeActor(actors[i], startingPos[i].AsArray());
		}



		JsonNode cameraNode = header["Camera"];
		DecodeCamera(cameraNode);
	}


	private void DecodeCamera(JsonNode cameraNode)
	{
		// set the zoom & offset of the camera
		float zoom = (float)cameraNode["Zoom"];

		JsonArray offset = cameraNode["Offset"].AsArray();
		Vector2 cameraOffset = new Vector2((float)offset[0], (float)offset[1]);


		// get the position of the camera
		JsonArray position = cameraNode["Position"].AsArray();
		Vector2 cameraPosition = new Vector2(0,0);
		if(position.Count == 2)
		{
			camera.Position = new Vector2((float)position[0], (float)position[1]);
		}

		// focus is the index of the Actor to focus on
		int focus = (int)cameraNode["Focus"];
		Node focusActor = null;
		if(focus >= 0 && focus < actorParent.GetChildren().Count)
		{
			focusActor = actorParent.GetChild(focus);
		}

		// set these values in CutsceneCamera.cs
		if(focusActor != null)
		{
			cameraScript.SetUp(zoom, cameraOffset, cameraPosition, actors, new KeyValuePair<int,Node>(focus, focusActor));
		}
		else
		{
			cameraScript.SetUp(zoom, cameraOffset, cameraPosition, actors);
		}


	}

	// create an individual actor and add it to the list & tree

		//change later:
			// make scenes for actors
			// put actors into their starting positions ahead of time
			// each cutscene has its own scene, in this case
	private void DecodeActor(JsonNode actor, JsonArray position)
	{
		//create
		CutsceneActor currentActor = cutsceneActorScene.Instantiate<CutsceneActor>();

		//set details
		currentActor.actorName = (string)actor;
		if(position.Count == 2)
			currentActor.Position = new Vector2((float)position[0], (float)position[1]);

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
			// camera
			else if(type == "Camera")
			{
				DecodeCameraStep(step);
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
	private void DecodeCameraStep(JsonNode camera)
	{
		string action = (string)camera["Action"];
		float length = (float)camera["Length"];
		JsonNode param = camera["Parameters"];

		CameraStep cameraStep = new CameraStep(cameraScript, action, length, timer, param);
		cutsceneSteps.Add(cameraStep);
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

	public class CameraStep : Step {
		CutsceneCamera Camera { get; set;}
		Timer timer {get; set;}

		string CameraAction {get; set;}

		JsonNode Parameters {get; set;}
		public CameraStep(CutsceneCamera camera, string action, float length, Timer timer, JsonNode param) 
		{
			Camera = camera;
			CameraAction = action;
			Length = length;
			this.timer = timer;
			Parameters = param;
		}
		private void DecodeAction()
		{
			if(CameraAction == "Pan")
			{
				// call CutsceneCamera function
				int focus = (int)Parameters["Focus"];
				float[] coords = new float[]{(float)Parameters["Coords"].AsArray()[0],(float)Parameters["Coords"].AsArray()[1]};
				Camera.Pan(Length, coords, focus);
			}
			else if(CameraAction == "Zoom")
			{
				Camera.ChangeZoom();

			}
			else if(CameraAction == "ChangeFocus")
			{
				throw new NotImplementedException("Camera Action ChangeFocus is not yet implemented.");

			}
			else
			{
				throw new NotSupportedException("Camera Action " + CameraAction + " is not supported. Is it a typo?");
			}
			
		}

        public override void Play()
        {
			DecodeAction();
			timer.Start((double)Length);
        }
    }
	public class Action : Step {

		CutsceneActor Actor { get; set; }
		protected string Function {get; set;}
		protected JsonArray Parameters {get; set;}

		protected Timer timer { get; set; }

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
