using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public partial class RowScript : Node
{
	[Export]
	Godot.Collections.Array<Label> labels = new Godot.Collections.Array<Label>();

	[Export]
	Godot.Collections.Array<TextureRect> images = new Godot.Collections.Array<TextureRect>();

	//<string, TextureButton> = name of button, button path
	[Export]
	Godot.Collections.Dictionary<string, NodePath> buttons = new Godot.Collections.Dictionary<string, NodePath>();

	// Dictionary<string[], Action> : string[0] = name of button, string[1] = function to change, Action = what function that is
	protected Dictionary<string[], Action> buttonActions = new Dictionary<string[], Action>();

	public virtual void Setup<T>(T value){}

	public virtual void Setup(string[] labelTexts, Texture2D[] textures)
	{
		//check equal length
		if(labelTexts.Length != labels.Count)
		{
			GD.PrintErr("In row " + Name + ", the amount of labels requiring text does not equal the amount of strings provided.");
			return;
		}
		//set strings
		for(int i = 0; i < labels.Count; i++)
		{
			labels[i].Text = labelTexts[i];
		}

		//check length
		if(textures.Length != images.Count)
		{
			GD.PrintErr("In row " + Name + ", the amount of images does not equal the amount of textures provided.");
			return;
		}

		//set textures
		for(int i = 0; i < textures.Length; i++)
		{
			images[i].Texture = textures[i];
		}

		if(buttons.Count > 0)
			SetupButtons();
	}

	private void SetupButtons()
	{
		foreach(KeyValuePair<string[], Action> pair in buttonActions)
		{
			//separate out the pair for readability
			string buttonName = pair.Key[0];
			string buttonConnection = pair.Key[1];
			Action action = pair.Value;

			//get the corresponding button and connect it to its action
			TextureButton btn = GetNode<TextureButton>(buttons[buttonName]);
			btn.Connect(buttonConnection, Callable.From(action));

		}
	}

}
