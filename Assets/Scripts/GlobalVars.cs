using Godot;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;

//TODO: (potential) inventory is one big <int,int> dict
public partial class GlobalVars : Node
{
	//should probably only be loaded on scenes where the goat menu could show up
	//TODO: thoughts: json stores the goat ideas, THIS stores which ones you have

	// ========================= SETTINGS ===========================================
	public static Settings settings = new Settings();

	// ========================= DATA DICTIONARIES ================================
	public static Dictionary<int, Goat> goats; // ID, Goat
	public static Dictionary<string, Material> materials;
	public static Dictionary<int, Job> jobs; //ID, Job
	public static Dictionary<int, Machine> machines; //ID, Machine
	public static Dictionary<string, Recipe> recipes; //name, Recipe
	public static Dictionary<string, Item> items; //name, item
	public static Dictionary<string, Shot> shots; //name, Shot

	// =========================== INVENTORY DICTIONARIES =================================
	public static Dictionary<string, int> materialsObtained = new Dictionary<string, int>(); //materialName, amountOfThatMaterial
	public static Dictionary<int, int> machineInventory = new Dictionary<int, int>(); //machineID, amountOfThatMachine
	public static Dictionary<string, int> itemInventory = new Dictionary<string, int>(); //itemName, amountOfThatItem

	public static readonly string spriteShotPath = "res://Assets/SpriteFrames/Towers/Shots/";
	public static readonly string texMaterialsPath = "res://Assets/Sprites/Items/Materials/";

	public static readonly int restingJobID = 0;

	private static readonly string jsonStartingPath = "res://Assets/Data/";
	public static readonly string cutscenePath = jsonStartingPath + "Cutscenes/";

	public static int currentDay = 0;
	public static int timeLimit = 5;

	public override void _Ready()
	{
		loadJSON("materials.json", "materials", parseMaterials);
		loadJSON("recipes.json", "recipes", parseRecipes);
		loadJSON("shots.json", "shots", parseShots);
		loadJSON("machines.json", "machines", parseMachines);
	    loadJSON("items.json", "items", parseItems);
		loadJSON("jobs.json", "jobs", parseJobs);

		machineInventory.Add(1, 5); //TODO: DELETE ME
		machineInventory.Add(0, 10); //TODO: DELETE ME

		materialsObtained.Add("Logs", 5); //TODO: DELETE ME
		materialsObtained.Add("Wheat", 10); //TODO: DELETE ME
		materialsObtained.Add("Rocks", 15); //TODO: DELETE ME

		itemInventory.Add("Magic Scroll", 5); //TODO: DELETE ME
		//TODO: UPDATE THIS
		goats = new Dictionary<int, Goat>();
		goats.Add(0, new Goat("Chell", 0, "Test Class", 100, 1, 0));
		goats.Add(1, new Goat("Wheat", 1, "Test Class", 100, 1, 0));

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
			string texPath = (string)material["texture"];
			var loadedTex = GD.Load(texMaterialsPath + texPath);
			Material m;

			//load with the texture only if successful
			if(loadedTex != null)
				m = new Material((string)material["name"], (int)material["id"], (string)material["description"], (string)material["type"], loadedTex as Texture2D, (int)material["value"]);
			else
				 m = new Material((string)material["name"], (int)material["id"], (string)material["description"], (string)material["type"], (int)material["value"]);
			
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

	private void parseItems(JsonArray array){
		items = new Dictionary<string, Item>();

		foreach(JsonNode item in array){
			int id = (int)item["id"];
			string name =(string)item["name"];
			string description = (string)item["description"];
			int value = (int)item["value"];
			int sellValue = (int)item["sellValue"];

			JsonArray statsNode = (JsonArray)item["stats"];
			Dictionary<string, float> stats = new Dictionary<string, float>();
			
			foreach(JsonNode stat in statsNode){
				stats.Add((string)stat["name"], (float)stat["amount"]);
			}

			Item newItem = new Item(name, id, description, stats, value, sellValue);
			items.Add(name, newItem);
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
			float aoeRange = (float)shot["aoeRange"];
			int damage = (int)shot["damage"];
			string texPath = (string)shot["texture"];

			//create the new object
			Shot s = new Shot(name, id, type, numEnemies, aoeRange, damage, texPath);
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

			JsonArray coords = machine["texture"].AsArray();
			int[] textureCoords = new int[] {(int)coords[0], (int)coords[1],(int)coords[2],(int)coords[3]};

			JsonArray sizeArr = machine["size"].AsArray();
			int[] size = new int[] {(int)sizeArr[0], (int)sizeArr[1]};

			Machine newMach = new Machine(name, id, type, level, range, fireRate, shot, desc, recipe, textureCoords, size);
			machines.Add(id, newMach);
		}

	}

	private void parseJobs(JsonArray array)
	{
		jobs = new Dictionary<int, Job>();
		foreach(JsonNode job in array)
		{
			int id = (int)job["id"];
			string name = (string)job["name"];
			
			int strain = (int)job["strain"];
			int expReward = (int)job["expReward"];

			JsonArray jsonResults = job["result"].AsArray();
			Dictionary<Material, int> results = new Dictionary<Material, int>();
			foreach(JsonNode result in jsonResults)
			{
				Material mat = materials[(string)result["name"]];
				int amt = (int)result["amount"];
				results.Add(mat, amt);
			}

			Job newJob = new Job(name, id, results, strain, expReward);
			jobs.Add(id, newJob);
		}

	}

}
