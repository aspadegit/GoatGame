using Godot;
using System;
using System.Text.Json.Nodes;
using System.Reflection;

// could probably become a part of NPC scenes, later on
public partial class CutsceneActor : Node2D
{
	AnimatedSprite2D sprite;
	public string actorName;

	[Export]
	Timer timer;
	bool continuousAction;

	float actionLength;

	object[] currentMethodParams;

	delegate void CurrentMethod(object[] parameters);
	CurrentMethod m;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		continuousAction = false;
		actionLength = 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(continuousAction)
		{
			m.Invoke(currentMethodParams);
		}
	}

	public void PlayAction(string func, JsonArray param, float length)
	{
		GD.Print("Will run function " + func + " with " + param.Count + " parameters for " + length + " seconds.");
		MethodInfo method = this.GetType().GetMethod(func);
		actionLength = length;

		// NOTE: ONLY ACCOUNTS FOR INTEGERS, FLOATS, AND STRINGS. 
			// so it is only partially dynamic. add more as needed
			// the remains of type safety holding on by a thin thread (gone)
		if(method != null)
		{

			// try catch for the evil type wizardry im doing here
			try{
				ParameterInfo[] paramInfo = method.GetParameters();

				// unequal parameter length
				if(paramInfo.Length != param.Count)
				{
					throw new ArgumentException("The number of " + func + "'s parameters (" + paramInfo.Length +") does not match the amount given in the JSON (" + param.Count + ").");
				}

				if(paramInfo.Length > 0)
				{
					object[] parameters = new object[param.Count];
					CheckParamType(parameters, param, paramInfo);

					currentMethodParams = parameters;
					method.Invoke(this, parameters);
				}
				else
				{
					method.Invoke(this, new object[]{});
				}
			}
			catch(Exception e)
			{
				GD.PrintErr(e.Message);
				GD.PrintErr(e.StackTrace);
			}
			
		}
		else
		{
			GD.PrintErr("Method " + func + " does not exist in CutsceneActor.cs");
		}

	}

	private void CheckParamType(object[] parameters, JsonArray param, ParameterInfo[] paramInfo)
	{
		for(int i = 0; i < param.Count; i++)
		{
			Type curParamType = paramInfo[i].ParameterType;
			
			// if int
			if(curParamType == typeof(int))
			{
				parameters[i] = (int)param[i];
			}
			else if(curParamType == typeof(float))
			{
				parameters[i] = (float)param[i];
			}
			else if(curParamType == typeof(string))
			{
				parameters[i] = (string)param[i];
			}
			else
			{
				throw new NotSupportedException("Type of type " + curParamType + " not supported");
			}

			GD.Print(" type of parameter " + i + " is " + parameters[i].GetType());
		}
	}

	//flip horizontally
	public void Flip()
	{
		continuousAction = false;
		sprite.FlipH = !sprite.FlipH;
	}

	public void Move(float x, float y)
	{
		//NOTE: will need a second timer for 2 actions on the same actor
		timer.Start(actionLength);
		continuousAction = true;

		currentMethodParams = new object[] {x, y, Position.X, Position.Y};
		m = ContinuousMove;
	}

	// parameters: { amount X total, amount Y total, starting X, Starting Y}
	private void ContinuousMove(object[] parameters)
	{
		if(timer.TimeLeft > 0)
		{
			float progress = (float)(1.0 - (timer.TimeLeft / timer.WaitTime));

			float progressX = progress*(float)parameters[0];
			float progressY = progress*(float)parameters[1];
			Position = new Vector2((float)parameters[2] + progressX, (float)parameters[3] + progressY);

		}
		else
		{
			continuousAction = false;
		}
	}

	public void TimerTimeout()
	{
		continuousAction = false;
	}

	// name of animation, and for how long
	public void PlayAnimation(string name, int length)
	{

	}
}
