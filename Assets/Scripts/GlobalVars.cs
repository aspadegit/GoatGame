using Godot;
using System;
using System.Collections.Generic;

public partial class GlobalVars : Node
{
	//should probably only be loaded on scenes where the goat menu could show up
	//TODO: thoughts: json stores the goat ideas, THIS stores which ones you have
	public static Dictionary<int, Goat> goats; // ID, Goat
	public static Dictionary<int, Material> materials;
	public static Dictionary<int, Job> jobs = new Dictionary<int, Job>(); //ID, Job
	public static Dictionary<int, Machine> machines = new Dictionary<int, Machine>(); //ID, Machine
	public static Dictionary<int, Recipe> recipes = new Dictionary<int, Recipe>();

	public static Dictionary<int, int> materialsObtained = new Dictionary<int, int>(); //materialID, amountOfThatMaterial
	public static Dictionary<int, int> machineInventory = new Dictionary<int, int>(); //machineID, amountOfThatMachine

	public static readonly int restingJobID = 0;

	public static int currentDay = 0;
	public static int timeLimit = 5;

	public override void _Ready()
	{
		//TODO: UPDATE THIS
		goats = new Dictionary<int, Goat>();
		goats.Add(0, new Goat("Chell", 0, "Test Class", 100, 1, 0));
		goats.Add(1, new Goat("Wheat", 1, "Test Class", 100, 1, 0));

		CreateMaterials();
		CreateRecipes();
		CreateMachines();

		//TODO: EDIT ME
		Dictionary<Material,int> d = new Dictionary<Material, int>();

		//TODO: UPDATE THIS
		jobs.Add(0, new Job("Rest", 0, d));
		d = new Dictionary<Material, int>();
		d.Add(materials[0], 5);
		jobs.Add(1, new Job("Mining", 1, d));
		jobs.Add(2, new Job("Logging", 2, d));
		d = new Dictionary<Material, int>();
		d.Add(materials[0], 1);
		d.Add(materials[1], 1);
		jobs.Add(3, new Job("Research", 3, d));
		jobs.Add(4, new Job("Farming", 4, d));

	

	}

	private void CreateMaterials()
	{
		//TODO: UPDATE THIS
		materials = new Dictionary<int, Material>();
		Material logs = new Material("Logs", 0, "Wood");
		materials.Add(0, logs);
		materials.Add(1, new Material("Rocks", 1, "Stone"));

	}

	private void CreateRecipes()
	{
		Dictionary<int,int> d = new Dictionary<int, int>();
		d.Add(0, 3);
		d.Add(1, 2);

		recipes.Add(0, new Recipe(0, d));

		d = new Dictionary<int, int>();
		d.Add(1, 5);
		recipes.Add(1, new Recipe(1, d));
	}

	private void CreateMachines()
	{
		machines.Add(0, new Machine("Arrow Tower", 0, "Ranged", 1, 50, "A machine that shoots arrows.", recipes[0]));
		machines.Add(1, new Machine("Cannon Tower", 0, "Ranged", 1, 80, "A cannon that shoots cannonballs.", recipes[1]));
	}

}
