using Godot;
using System;
using System.Collections.Generic;

public partial class SbPauseMenu : Control
{
	private bool canShowMenu = true;
	[Export]
	public Control pageParent;

	[Export]
	public Control mainPage;

	[Export]
	public Control buttonParent;

	public Stack<Control> pageStack;

	public override void _Ready()
	{
		pageStack = new Stack<Control>();
		SignalHandler.Instance.Connect(SignalHandler.SignalName.TogglePlayerMovement, Callable.From((bool toggle)=> OnToggleMovement(toggle)), (uint)ConnectFlags.Deferred);
	}
	
	void OnToggleMovement(bool toggle)
	{
		if(!IsVisibleInTree())
			canShowMenu = toggle;
	}
	public override void _Process(double delta)
	{
		//TODO: pause & unpause
		if(Input.IsActionJustPressed("escape"))
		{
			if(canShowMenu && !IsVisibleInTree())
			{
				ShowMenu();
				SignalHandler.Instance.EmitSignal(SignalHandler.SignalName.TogglePlayerMovement, false);
			}
			else if(IsVisibleInTree())
			{
				Close();
			}
		}
	}

	public void OnResume()
	{
		Close();
	}

	public void ChangePage(Button caller, Variant path)
	{
		Control newPage = caller.GetNode<Control>((NodePath)path);
		//hide old page
		pageStack.Peek().Hide();

		//add & show new page
		pageStack.Push(newPage);
		newPage.Show();
	}

	public void BackPage()
	{
		if(pageStack.Count == 0)
		{
			Close();
		}
		else
		{
			// remove the current page from top of stack
			Control current = pageStack.Pop();

			//show the previous page
			pageStack.Peek().Show();
			current.Hide();
		}
		
	}

	private void ShowMenu()
	{
		Show();
		SetupButtons();
		pageStack.Push(mainPage);
	}

	private void SetupButtons()
	{
		foreach(Control container in buttonParent.GetChildren())
		{
			//if it opens a menu
			if(container.HasMeta("opensMenu") && (bool)container.GetMeta("opensMenu"))
			{
				Button child = container.GetChild<Button>(0);
				if(child.HasMeta("newMenu"))
					child.Connect("pressed", Callable.From(()=> ChangePage(child, child.GetMeta("newMenu"))));
			}
		}
	}

	private void Close()
	{
		//TODO: unpause game
		Hide();

		pageStack.Clear();

		//hide all children
		foreach(Control child in pageParent.GetChildren())
		{
			child.Hide();
		}

		//show the main page
		mainPage.Show();

		// let player move again
		SignalHandler.Instance.EmitSignal(SignalHandler.SignalName.TogglePlayerMovement, true);
		
	}

	
}
