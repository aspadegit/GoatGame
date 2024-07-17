using Godot;
using System;
using System.Reflection;

public partial class Lives : Control
{
	[Export]
	Control gameOverScreen;
	[Export]
	Control winLevelScreen;
	[Export]
	Node towerDefenseParent;

	[Export]
	public int numEnemies = 99;
	[Export]
	public int numLives = 99;
	

	private Label numLivesLabel;
	private Label numEnemiesLabel;
	private Timer winTimer;

	private bool win;
	public override void _Ready()
	{
		//setup signals
		TowerDefenseSignals.Instance.Connect(TowerDefenseSignals.SignalName.EnemyBreakthrough, Callable.From((int param)=> DecreaseLives(param)), (uint)ConnectFlags.Deferred);
		TowerDefenseSignals.Instance.Connect(TowerDefenseSignals.SignalName.OnEnemyDeath, Callable.From(()=>DecreaseEnemies(1)), (uint)ConnectFlags.Deferred);
		
		//get nodes
		numLivesLabel = GetNode<Label>("PanelMargin/HBoxMargin/HBoxContainer/NumLives");
		numEnemiesLabel = GetNode<Label>("PanelMargin/HBoxMargin/HBoxContainer/NumEnemiesRemaining");
		winTimer = GetNode<Timer>("Timer");
		
		//set the text
		numEnemiesLabel.Text = numEnemies.ToString();
		numLivesLabel.Text = numLives.ToString();
	

	}

	private void DecreaseEnemies(int amt)
	{
		numEnemies -= amt;
		if(numEnemies < 0)
			numEnemies = 0;

		numEnemiesLabel.Text = numEnemies.ToString();
		if(numEnemies <= 0 && numLives > 0)
		{
			win = true;
			winTimer.Start();
		}
		

	}

	//when an enemy breaks through, decrease how many lives we have & update the string
	private void DecreaseLives(int amount)
	{
		numLives -= amount;

		if(numLives < 0)
			numLives = 0;

		numLivesLabel.Text = numLives.ToString();
		if(numLives <= 0)
		{
			win = false;
			winTimer.Start();
		}
	}
	
	private void _on_timer_timeout()
{
	if (win == true){
		winLevelScreen.Show();
		GD.Print("Showing win screen");
		}
	else{
		gameOverScreen.Show();
		GD.Print("Showing lose screen");
		}
		
	towerDefenseParent.GetTree().Paused = true;
}

}


