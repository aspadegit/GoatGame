using System.Collections.Generic;
using System.Linq;
using Godot;
public class Shot
{
	public string Name {get; set;}
	public int ID {get; set;}
	public string Type {get; set;}
	public int NumEnemiesToHit {get; set;}
	public float AoeRange {get; set;} //how far multihits can go (scales by the size of the hitbox)
	public int Damage {get; set;}

	public string TexturePath {get; set;}

	public Shot(string Name, int ID, string Type, int NumEnemiesToHit, float AoeRange, int Damage, string TexturePath)
	{
		this.Name = Name;
		this.ID = ID;
		this.Type = Type;
		this.NumEnemiesToHit = NumEnemiesToHit;
		this.AoeRange = AoeRange;
		this.Damage = Damage;
		this.TexturePath = TexturePath;
	}

	//handles the number of enemies that are shot
	public virtual List<Enemy> GetShotEnemies(List<Enemy> enemiesCanHit, List<Enemy> enemiesInAoe, int targetedIndex)
	{
		List<Enemy> result = new List<Enemy>();

		//can't hit any
		if(targetedIndex == -1)
			return result;

		result.Add(enemiesCanHit[targetedIndex]);

		//can only hit 1; no need to sort; just return here (small optimization)
		if(NumEnemiesToHit == 1)
			return result;

		//sort the list by how close other enemies are to the targeted enemies
		Vector2 checkPos = enemiesCanHit[targetedIndex].Position;

		//TODO: this probably isn't necessary, so remove once all shot types are finished if it's still unused
		List<Enemy> sortedList = enemiesInAoe.OrderBy(enemy=>enemy.Position.DistanceTo(checkPos)).ToList(); 
		
		//starts at 1 to account for the first enemy (added earlier)
		for(int i = 1; i < NumEnemiesToHit; i++)
		{
			//avoid index out of bounds
			if(i >= sortedList.Count)
				break;

			Enemy curEnemy = sortedList[i];

			result.Add(curEnemy);
		}

		return result;
	}


}   
