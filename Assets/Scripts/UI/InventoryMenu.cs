using Godot;
using System;

public partial class InventoryMenu : Control
{
	[Export]
	TabContainer tabParent;

	// disables when player movement is disabled
	private bool canShowMenu = true;

    public override void _Ready()
    {
		SignalHandler.Instance.Connect(SignalHandler.SignalName.TogglePlayerMovement, Callable.From((bool toggle)=> OnToggleMovement(toggle)), (uint)ConnectFlags.Deferred);

    }

	void OnToggleMovement(bool toggle)
	{
		canShowMenu = toggle;
	}

    public override void _Process(double delta)
	{
		//TODO: pause & unpause
		if(canShowMenu && Input.IsActionJustPressed("escape"))
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
