using Godot;
using System;
using System.Collections.Generic;

public partial class GlobalVars : Node
{
	//TODO: thoughts: json stores the goat ideas, THIS stores which ones you have
	public static List<Goat> goats;
	public override void _Ready()
	{
		goats = new List<Goat>();
		goats.Add(new Goat("Chell", "Test Class", 100, 1, 0));
		GD.Print(goats[0].Name);
	}

}
