using Godot;
using System;

public partial class TowerDefenseSignals : Node
{
	public static TowerDefenseSignals Instance {get; private set;}

	//a tower select row sends a signal to the tower scene on what machine ID has been clicked
	[Signal]
	public delegate void TowerSelectEventHandler(int param);

	//enemy emits a signal when it breaks through the towers, to decrease lives

	[Signal]
	public delegate void EnemyBreakthroughEventHandler(int param);

	//emitted by an enemy as it starts to die
	[Signal]
	public delegate void OnEnemyDeathEventHandler();


	public override void _Ready() {
		Instance = this;
	}


}
