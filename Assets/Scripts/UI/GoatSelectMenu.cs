using Godot;
using System;

//TODO: sorting goat menu
public partial class GoatSelectMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	Button cancelButton;
	PackedScene goatSelectRow = GD.Load<PackedScene>("res://Scenes/UI/GoatSelectRow.tscn");
	VBoxContainer goatListContainer;
	bool hasChanged = true;
	public override void _Ready()
	{
		SignalHandler.Instance.Connect(SignalHandler.SignalName.ShowGoatMenu, Callable.From(()=> OnShowGoatMenu()), (uint)ConnectFlags.Deferred);
		cancelButton = GetNode<Button>("MainContainer/MainVBoxContainer/ButtonHBox/CancelButton");
		goatListContainer = GetNode<VBoxContainer>("MainContainer/MainVBoxContainer/MainHBox/GoatListMargin/GoatListContainer");
		cancelButton.Pressed += HideAndReset;

	}

	private void OnShowGoatMenu()
	{
		Show();

		//TODO: hasChanged always true
		if(hasChanged)
			InstantiateChildren();
		cancelButton.CallDeferred("grab_focus");
	}

	private void HideAndReset()
	{
		Hide();
	}

	private void InstantiateChildren()
	{
		//remove all of the old goat instances
		foreach(Node child in goatListContainer.GetChildren())
		{
			goatListContainer.RemoveChild(child);
			child.QueueFree();
		}

		//spawn in new goat buttons
		foreach(Goat goat in GlobalVars.goats)
		{
			Node row = goatSelectRow.Instantiate();
			row.Name = goat.Name;
			SetRowInformation(goat, row);
			goatListContainer.AddChild(row);
		}
	}

	private void SetRowInformation(Goat goat, Node row)
	{
		Label nameLabel = row.GetNode<Label>("GoatSelectMargin/GoatSelectHBox/GoatMargin/GoatVBox/NameLabel");
		Label classLevelLabel = row.GetNode<Label>("GoatSelectMargin/GoatSelectHBox/StaminaClassMargin/StaminaClassVBox/ClassLevelLabel");
		nameLabel.Text = goat.Name;
		classLevelLabel.Text = goat.Class + " LV. " + goat.Level;

	}

}
