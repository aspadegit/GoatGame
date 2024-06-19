using Godot;
using System;
using System.Collections.Generic;

public partial class GoatSelectRow : Control
{
	TextureButton selectButton;
	VBoxContainer bigHitbox;
	MarginContainer popUpMargin;
	HBoxContainer materialHBox;
	int currentJobNum = 0;
	int jobTotal = 4;
	int goatID = -1;

	readonly int[] textureOffsetX = {55, 163, 271, 379};
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		try{
			popUpMargin = GetNode<MarginContainer>("VBoxContainer/PopUpMargin");
			bigHitbox = GetNode<VBoxContainer>("VBoxContainer");
			selectButton = GetNode<TextureButton>("VBoxContainer/GoatSelectMargin/GoatSelectHBox/SelectMargin/SelectButton");
			materialHBox = GetNode<HBoxContainer>("VBoxContainer/PopUpMargin/PopUpHBox/PopUpPanel/MaterialMargin/MaterialHBox");

			selectButton.Pressed += SelectGoat;
			selectButton.FocusEntered += Hover;
			selectButton.FocusExited += LeaveHover;

			selectButton.Connect("mouse_entered", Callable.From(Hover));
			selectButton.Connect("mouse_exited", Callable.From(LeaveHover));
		}
		catch(InvalidCastException e)
		{
			GD.PrintErr(e);
		}

		//turns the name of this row (Node) into goatID
		if(!Int32.TryParse(Name.ToString(), out goatID))
		{
			//parse attempt was unsuccessful
			GD.PrintErr("Unable to parse " + Name + " to an integer in GoatSelectRow.");
		}

	}

	private void SelectGoat()
	{
		currentJobNum++;
		if(currentJobNum >= jobTotal)
		{
			currentJobNum = 0;
		}
		UpdateGoat();
		ChangeTexture();
	}

	private void ChangeTexture()
	{
		AtlasTexture tex = (AtlasTexture)selectButton.TextureNormal;
		Rect2 newTex = new Rect2(textureOffsetX[currentJobNum], tex.Region.Position.Y, tex.Region.Size);
		tex.Region = newTex;

		//TODO: replace this with changing textures via atlas map (possibly store coords in Material obj)
		Job job = GlobalVars.goats[goatID].AssignedJob;
		if(job.Name != "Rest")
		{
			foreach(KeyValuePair<Material, int> pair in job.Result)
			{
				string matName = pair.Key.Name;
				materialHBox.GetNode<TextureRect>(matName).Show();
				materialHBox.GetNode<TextureRect>("Rest").Hide();

			}
		}
		else
		{
			foreach(Node child in materialHBox.GetChildren())
			{
				((TextureRect)child).Hide();
			}
			materialHBox.GetNode<TextureRect>("Rest").Show();
		}
	}

	private void UpdateGoat()
	{
		//get the goat
		Goat goat = GlobalVars.goats[goatID];

		//TODO: currentJobNum here is sooooo temporary
			//jobs should become a dictionary, and currentJobNum will become a new thing and there will be code here to
			//    figure out what job ID the button currently corresponds to

			//...also, if im lazy and this stays, leave this comment in for haha sillies
		goat.AssignedJob = GlobalVars.jobs[currentJobNum];
	}

	
	private void Hover()
	{
		popUpMargin.Modulate = new Color(1,1,1,1);
	}

	private void LeaveHover()
	{

		popUpMargin.Modulate = new Color(1,1,1,0);
		
	}
}
