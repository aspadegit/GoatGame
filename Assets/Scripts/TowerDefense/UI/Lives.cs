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

	//initial numbers for lives/enemies
	[Export]
	public int numEnemies = 99;
	[Export]
	public int numLives = 99;

	//at the moment, just goes to tilescript & calls OnPause, so that the blue square doesn't hang around awkwardly
	[Signal]
	public delegate void PauseGameEventHandler();
	
	//tracks what lives/enemies is currently at
	private int curEnemies;
	private int curLives;

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
		
		ResetText();

	}

	private void DecreaseEnemies(int amt)
	{
		curEnemies -= amt;
		if(curEnemies < 0)
			curEnemies = 0;

		numEnemiesLabel.Text = curEnemies.ToString();
		if(curEnemies <= 0 && curLives > 0)
		{
			win = true;
			winTimer.Start();
		}
		

	}

	//when an enemy breaks through, decrease how many lives we have & update the string
	private void DecreaseLives(int amount)
	{
		curLives -= amount;

		if(curLives < 0)
			curLives = 0;

		numLivesLabel.Text = curLives.ToString();
		if(curLives <= 0)
		{
			win = false;
			winTimer.Start();
		}
	}
	
	private void _on_timer_timeout()
	{
		if (win == true)
		{
			winLevelScreen.Show();
			GD.Print("Showing win screen");
		}
		else
		{
			gameOverScreen.Show();
			GD.Print("Showing lose screen");
		}
		Pause();
	}

	//reset lives & enemies & the text
	public void ResetText()
	{
		curEnemies = numEnemies;
		curLives = numLives;

		if(numEnemiesLabel != null && numLivesLabel != null)
		{
			numEnemiesLabel.Text = curEnemies.ToString();
			numLivesLabel.Text = curLives.ToString();
		}

	}

	private void Pause()
	{
		EmitSignal(SignalName.PauseGame);
		towerDefenseParent.GetTree().Paused = true;

	}

}


