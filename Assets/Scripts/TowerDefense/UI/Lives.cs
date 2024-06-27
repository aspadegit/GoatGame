using Godot;
using System;

public partial class Lives : Control
{
	[Export]
	Control gameOverScreen;
	[Export]
	Node towerDefenseParent;

	[Export]
	public int numEnemies = 99;
	[Export]
	public int numLives = 99;

	private Label numLivesLabel;
	private Label numEnemiesLabel;
	public override void _Ready()
	{
		//setup signals
		SignalHandler.Instance.Connect(SignalHandler.SignalName.EnemyBreakthrough, Callable.From((int param)=> DecreaseLives(param)), (uint)ConnectFlags.Deferred);
		SignalHandler.Instance.Connect(SignalHandler.SignalName.OnEnemyDeath, Callable.From(()=>DecreaseEnemies(1)), (uint)ConnectFlags.Deferred);
		
		//get nodes
		numLivesLabel = GetNode<Label>("PanelMargin/HBoxMargin/HBoxContainer/NumLives");
		numEnemiesLabel = GetNode<Label>("PanelMargin/HBoxMargin/HBoxContainer/NumEnemiesRemaining");
		
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
			gameOverScreen.Show();
			towerDefenseParent.GetTree().Paused = true;
		}
	}

}
