using System.Collections.Generic;
using Godot;
public class Job
{
	public string Name {get; private set;}
	public int ID {get; private set;}
	public Dictionary<Material, int> Result {get; private set;} // (Material, the average amount of it that you get)
	
	public int Strain {get; set;} //how much stamina it takes
	public int ExpReward {get; set;} //how much exp it gives

	public Job()
	{
	   Name = "[UNITIALIZED NAME]";
	   ID = -1;
	   Result = null;
	}

	public Job(string Name, int ID, Dictionary<Material, int> Result, int Strain, int ExpReward)
	{
		this.Name = Name;
		this.ID = ID;
		this.Result = Result;
		this.Strain = Strain;
		this.ExpReward = ExpReward;
	}

	public override string ToString()
	{
		string returnStr = "JOB PRINT\nName: " + Name + "\nResult: ";
		foreach(KeyValuePair<Material, int> pair in Result)
		{
			returnStr += "(";
			returnStr += pair.Key.Name;
			returnStr += ", ";
			returnStr += pair.Value;
			returnStr += ")\n";
		}

		return returnStr;
	}

}   
