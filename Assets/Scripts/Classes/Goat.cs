using System.Collections.Generic;
using Godot;
public class Goat
{
	public string Name {get; set;}
	public int ID {get; private set;}
	public string Class {get; private set;} //TODO: potentially, make this a subclass (Coding term not gaming term)
	public float Stamina {get; private set;}
	public float CurrentStamina {get; private set;}
	public int Level {get; private set;}
	public int Exp { get; private set;}
	public Job AssignedJob { get; set; }

	public Goat()
	{
		Name = "[UNINITIALIZED GOAT]";
		Class = "[UNINITIALIZED CLASS]";
		ID = -1;
		Stamina = -1;
		Level = -1;
		Exp = -1;

		//TODO: make this work with whatever GlobalVars becomes
		AssignedJob = null;  
	}
	public Goat(string Name, int ID, string Class, float Stamina, int Level, int Exp)
	{
		this.Name = Name;
		this.ID = ID; 
		this.Class = Class;
		this.Stamina = Stamina;
		this.Level = Level;
		this.Exp = Exp;

		AssignedJob = null;
		CurrentStamina = Stamina;
		
	}

	//TODO: refine
	public void DoJob()
	{
		//resting restores stamina
		
		if(AssignedJob.Strain < 0)
		{
			CurrentStamina = Stamina;
		}
		else
		{
			CurrentStamina -= AssignedJob.Strain;
		}

		Exp += AssignedJob.ExpReward;

		//TODO: exp function where it's not just 100 every time
		if(Exp >= 100)
		{
			LevelUp();
		}
	}

	private void LevelUp()
	{
		Level++;
		Exp -= 100; //TDOO: change in regards to function

		//shouldnt happen
		if(Exp < 0)
			Exp = 0;

	}

	public override string ToString()
	{
		return "GOAT PRINT\nName: " + Name + "\nID: " + ID + "\nClass: " + Class + "\nStamina: " + Stamina + "\nLevel: " + Level + "\nExp: " + Exp + "\nJob: " + AssignedJob;
	}
}   
