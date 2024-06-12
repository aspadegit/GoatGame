using Godot;
using System;
using System.Collections.Generic;

public partial class GlobalVars : Node
{
	//should probably only be loaded on scenes where the goat menu could show up
	//TODO: thoughts: json stores the goat ideas, THIS stores which ones you have
	public static Dictionary<int, Goat> goats; // ID, Goat
	public static List<Material> materials;
	public static Dictionary<int, Job> jobs = new Dictionary<int, Job>(); //ID, Job

	public static Dictionary<Material, int> materialsObtained = new Dictionary<Material, int>();

	public static readonly int restingJobID = 0;

	public static int currentDay = 0;
	public static int timeLimit = 5;
	public override void _Ready()
	{
		//TODO: UPDATE THIS
		goats = new Dictionary<int, Goat>();
		goats.Add(0, new Goat("Chell", 0, "Test Class", 100, 1, 0));
		goats.Add(1, new Goat("Wheat", 1, "Test Class", 100, 1, 0));

		//TODO: UPDATE THIS
		materials = new List<Material>();
		Material logs = new Material("Logs", 0, "Wood");
		materials.Add(logs);
		materials.Add(new Material("Rocks", 1, "Stone"));

		//TODO: EDIT ME
		Dictionary<Material,int> d = new Dictionary<Material, int>();

		//TODO: UPDATE THIS
		jobs.Add(0, new Job("Rest", 0, d));
		d = new Dictionary<Material, int>();
		d.Add(logs, 5);
		jobs.Add(1, new Job("Mining", 1, d));
		jobs.Add(2, new Job("Logging", 2, d));
		d = new Dictionary<Material, int>();
		d.Add(logs, 1);
		d.Add(materials[1], 1);
		jobs.Add(3, new Job("Research", 3, d));
		jobs.Add(4, new Job("Farming", 4, d));

	

	}


}
