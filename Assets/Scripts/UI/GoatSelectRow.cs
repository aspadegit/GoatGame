using Godot;
using System;

public partial class GoatSelectRow : Control
{
	TextureButton selectButton;
	int currentJobNum = 0;
	int jobTotal = 4;

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
		
	}

	private void SelectGoat()
	{
		currentJobNum++;
		if(currentJobNum >= jobTotal)
		{
			currentJobNum = 0;
		}
		ChangeTexture();
	}

	private void ChangeTexture()
	{
		AtlasTexture tex = (AtlasTexture)selectButton.TextureNormal;
		Rect2 newTex = new Rect2(textureOffsetX[currentJobNum], tex.Region.Position.Y, tex.Region.Size);
		tex.Region = newTex;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
