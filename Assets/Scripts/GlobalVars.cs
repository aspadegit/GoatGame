using Godot;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;

public partial class GlobalVars : Node
{
	//should probably only be loaded on scenes where the goat menu could show up
	//TODO: thoughts: json stores the goat ideas, THIS stores which ones you have
	public static Dictionary<int, Goat> goats; // ID, Goat
	public static Dictionary<string, Material> materials;
	public static Dictionary<int, Job> jobs = new Dictionary<int, Job>(); //ID, Job
	public static Dictionary<int, Machine> machines; //ID, Machine
	public static Dictionary<string, Recipe> recipes; //name, Recipe

	public static Dictionary<string, Shot> shots; //name, Shot

	public static Dictionary<string, int> materialsObtained = new Dictionary<string, int>(); //materialName, amountOfThatMaterial
	public static Dictionary<int, int> machineInventory = new Dictionary<int, int>(); //machineID, amountOfThatMachine

	public static readonly int restingJobID = 0;

	private static readonly string jsonStartingPath = "res://Assets/Data/";

	public static int currentDay = 0;
	public static int timeLimit = 5;

	public override void _Ready()
	{
		loadJSON("materials.json", "materials", parseMaterials);
		loadJSON("recipes.json", "recipes", parseRecipes);
		loadJSON("shots.json", "shots", parseShots);
		loadJSON("machines.json", "machines", parseMachines);

		machineInventory.Add(0, 5); //TODO: DELETE ME
		machineInventory.Add(1, 5); //TODO: DELETE ME
		//TODO: UPDATE THIS
		goats = new Dictionary<int, Goat>();
		goats.Add(0, new Goat("Chell", 0, "Test Class", 100, 1, 0));
		goats.Add(1, new Goat("Wheat", 1, "Test Class", 100, 1, 0));

		//TODO: EDIT ME
		Dictionary<Material,int> d = new Dictionary<Material, int>();

		//TODO: jobs must read from JSON
		jobs.Add(0, new Job("Rest", 0, d));
		d = new Dictionary<Material, int>();
		d.Add(materials["Logs"], 5);
		jobs.Add(1, new Job("Mining", 1, d));
		jobs.Add(2, new Job("Logging", 2, d));
		d = new Dictionary<Material, int>();
		d.Add(materials["Logs"], 1);
		d.Add(materials["Rocks"], 1);
		jobs.Add(3, new Job("Research", 3, d));
		jobs.Add(4, new Job("Farming", 4, d));

	

	}
	private void loadJSON(string path, string type, Action<JsonArray> subFunction)
	{
		//reads in the file
		FileAccess file = FileAccess.Open(jsonStartingPath + path, FileAccess.ModeFlags.Read);
		string fileText = file.GetAsText();
		file.Close();

		//converts the file to json
       	JsonNode jsonNode = JsonNode.Parse(fileText)!;
		JsonArray array = jsonNode[type].AsArray();
		
		subFunction.Invoke(array);
		
	}

	private void parseMaterials(JsonArray array)
	{
		materials = new Dictionary<string, Material>();
		foreach(JsonNode material in array)
		{
			Material m = new Material((string)material["name"], (int)material["id"], (string)material["description"], (string)material["type"]);
			materials.Add(m.Name, m);
		}

	}

	private void parseRecipes(JsonArray array)
	{
		recipes = new Dictionary<string, Recipe>();
		foreach(JsonNode recipe in array)
		{
			//initial stuff
			int id = (int)recipe["id"];
			string name = (string)recipe["name"];
			JsonArray requiredItems = (JsonArray)recipe["required_items"];

			Dictionary<string, int> currentRecipeItems = new Dictionary<string, int>();

			//add the required items (sub json array) to the dictionary
			foreach(JsonNode materialObj in requiredItems)
			{
				currentRecipeItems.Add((string)materialObj["name"], (int)materialObj["amount"]);
			}

			//create the new object
			Recipe newRecipe = new Recipe(name, id, currentRecipeItems);
			recipes.Add(name, newRecipe);
		}
	}

	private void parseShots(JsonArray array)
	{
		shots = new Dictionary<string, Shot>();
		foreach(JsonNode shot in array)
		{
			//initial stuff
			string name = (string)shot["name"];
			int id = (int)shot["id"];
			string type = (string)shot["type"];
			int numEnemies = (int)shot["numEnemies"];
			int aoeRange = (int)shot["aoeRange"];
			int damage = (int)shot["damage"];

			//create the new object
			Shot s = new Shot(name, id, type, numEnemies, aoeRange, damage);
			shots.Add(name, s);
		}
	}

	private void parseMachines(JsonArray array)
	{
		machines = new Dictionary<int, Machine>();
		foreach(JsonNode machine in array)
		{
			int id = (int)machine["id"];
			string name = (string)machine["name"];
			string type = (string)machine["type"];
			
			int level = (int)machine["level"];
			int dmg = (int)machine["damage"];
			int range = (int)machine["range"];
			int fireRate = (int)machine["fireRate"];

			Shot shot = shots[(string)machine["shotType"]];
			string desc = (string)machine["description"];
			Recipe recipe = recipes[(string)machine["recipe"]];

			Machine newMach = new Machine(name, id, type, level, range, fireRate, shot, desc, recipe);
			machines.Add(id, newMach);
		}

	}

}
