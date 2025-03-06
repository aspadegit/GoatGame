using Godot;
using System;

// this script attaches as a child scene beneath the button that should grab focus when visibility changes in a menu
public partial class FocusGrabber : Control
{
	[Export]
	public Control visibilityWatch; //the control node whose visibility dictates this button grabbing focus

	private Button parentButton;

	public override void _Ready()
	{
		parentButton = GetParent<Button>();
		visibilityWatch.VisibilityChanged += OnVisChange;
	}

	public void OnVisChange()
	{
		if(visibilityWatch.IsVisibleInTree())
		{

			GetParent().CallDeferred("grab_focus");
		}
	}
}
