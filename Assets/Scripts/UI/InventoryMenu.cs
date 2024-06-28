using Godot;
using System;

public partial class InventoryMenu : Control
{
	[Export]
	TabContainer tabParent;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//TODO: pause & unpause
		if(Input.IsActionJustPressed("escape"))
		{
			if(!IsVisibleInTree())
			{
				InstantiateAllChildren();
				Show();
			}
			else
			{
				Hide();
			}
		}
	}

	void InstantiateAllChildren()
	{
		//remove all of the old machine instances (TODO: not always do this)
		foreach(Node child in tabParent.GetChildren())
		{
			InventoryTab tab = child as InventoryTab;
			tab.InstantiateChildren();
		}
	}
}
