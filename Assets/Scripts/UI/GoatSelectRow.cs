using Godot;
using System;

public partial class GoatSelectRow : Control
{
	TextureButton selectButton;
	int currentJobNum = 0;
	int jobTotal = 4;
	int goatID = -1;

	readonly int[] textureOffsetX = {55, 163, 271, 379};
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		try{
			selectButton = GetNode<TextureButton>("GoatSelectMargin/GoatSelectHBox/SelectMargin/SelectButton");
			selectButton.Pressed += SelectGoat;
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
		ChangeTexture();
		UpdateGoat();
	}

	private void ChangeTexture()
	{
		AtlasTexture tex = (AtlasTexture)selectButton.TextureNormal;
		Rect2 newTex = new Rect2(textureOffsetX[currentJobNum], tex.Region.Position.Y, tex.Region.Size);
		tex.Region = newTex;
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

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
