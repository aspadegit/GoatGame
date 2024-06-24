using System.Collections.Generic;
using System.Linq;
using Godot;
public class Shot
{
    public string Name {get; set;}
    public int ID {get; set;}
    public string Type {get; set;}
    public int NumEnemiesToHit {get; set;}
    public int AoeRange {get; set;} //how far multihits can go
    public int Damage {get; set;}

    public Shot(string Name, int ID, string Type, int NumEnemiesToHit, int AoeRange, int Damage)
    {
        this.Name = Name;
        this.ID = ID;
        this.Type = Type;
        this.NumEnemiesToHit = NumEnemiesToHit;
        this.AoeRange = AoeRange;
        this.Damage = Damage;
    }

    public virtual List<Enemy> GetShotEnemies(List<Enemy> enemiesCanHit, int targetedIndex)
    {
        List<Enemy> result = new List<Enemy>();

        //can't hit any
        if(targetedIndex == -1)
            return result;

        result.Add(enemiesCanHit[targetedIndex]);

        //can only hit 1; no need to sort; just return here (small optimization)
        if(NumEnemiesToHit == 1)
            return result;

        //sort the list by how close enemies are to the target
        Vector2 checkPos = enemiesCanHit[targetedIndex].Position;

        List<Enemy> sortedList = enemiesCanHit.OrderBy(enemy=>enemy.Position.DistanceTo(checkPos)).ToList();
        
        //starts at 1 to account for the first enemy (added earlier)
        for(int i = 1; i < NumEnemiesToHit; i++)
        {
            //avoid index out of bounds
            if(i >= sortedList.Count)
                break;

            Enemy curEnemy = sortedList[i];

            //all subsequent enemies will be too far, so break
            if(curEnemy.Position.DistanceTo(checkPos) > AoeRange)
                break;

            result.Add(curEnemy);
        }

        return result;
    }


}   