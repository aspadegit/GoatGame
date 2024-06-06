using Godot;
using System;
using System.Collections.Generic;

public partial class GlobalVars : Node
{
	//should probably only be loaded on scenes where the goat menu could show up
	//TODO: thoughts: json stores the goat ideas, THIS stores which ones you have
	public static Dictionary<int, Goat> goats; // ID, Goat
	public static List<Material> materials;
	public static List<Job> jobs;
	public override void _Ready()
	{
		//TODO: UPDATE THIS
		goats = new Dictionary<int, Goat>();
		goats.Add(0, new Goat("Chell", 0, "Test Class", 100, 1, 0));
		goats.Add(1, new Goat("Wheat", 1, "Test Class", 100, 1, 0));

		//TODO: UPDATE THIS
		materials = new List<Material>();
		Material logs = new Material("Logs", 1, "Wood");
		materials.Add(logs);

		//TODO: EDIT ME
		jobs = new List<Job>();

		Dictionary<Material,int> d = new Dictionary<Material, int>();

		//TODO: UPDATE THIS
		jobs.Add(new Job("Rest", 0, d));
		d = new Dictionary<Material, int>();
		d.Add(logs, 5);
		jobs.Add(new Job("Mining", 1, d));
		jobs.Add(new Job("Logging", 2, d));
		d = new Dictionary<Material, int>();
		d.Add(logs, 1);
		jobs.Add(new Job("Research", 3, d));
		jobs.Add(new Job("Farming", 4, d));

	}

}
