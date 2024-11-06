using Godot;
using System;
using System.Collections.Generic;

public partial class GoatSelectRow : Control
{

	[Export]
	ProgressBar staminaBar; //green bar

	[Export]
	ProgressBar staminaRestoreBar; //light green bar

	[Export]
	ProgressBar expBar;	//blue bar

	TextureButton selectButton;
	VBoxContainer bigHitbox;
	MarginContainer popUpMargin;
	HBoxContainer materialHBox;
	int currentJobNum = 0;
	int jobTotal = GlobalVars.jobs.Count;
	int goatID = -1;

	readonly int[] textureOffsetX = {55, 163, 271, 379, 487};

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
		//successful parse
		else
		{
			SetupBars();

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
		PretendDoJob(GlobalVars.goats[goatID]);

	}

	private void ChangeTexture()
	{
		AtlasTexture tex = (AtlasTexture)selectButton.TextureNormal;
		Rect2 newTex = new Rect2(textureOffsetX[currentJobNum], tex.Region.Position.Y, tex.Region.Size);
		tex.Region = newTex;

		Job job = GlobalVars.goats[goatID].AssignedJob;

		// hide all of them
		foreach(Node child in materialHBox.GetChildren())
		{
			((TextureRect)child).Hide();
		}
		if(job.Name != "Rest")
		{
			int i = 0;
			foreach(KeyValuePair<Material, int> pair in job.Result)
			{
				string matName = pair.Key.Name;
				Texture2D matTex = pair.Key.Texture;
				TextureRect currRect = materialHBox.GetNode<TextureRect>("Material"+i);

				currRect.Texture = matTex;
				currRect.Show();

				materialHBox.GetNode<TextureRect>("Rest").Hide();

				i++;

			}
		}
		//show rest
		else
		{
			
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

	private void PretendDoJob(Goat goat)
	{
		staminaBar.Value = goat.CurrentStamina;
		expBar.GetParent<ProgressBar>().Value = goat.Exp;
		Job job = goat.AssignedJob;
		DecreaseStaminaBar(job.Strain);
		IncreaseExpBar(job.ExpReward);
	}
	private void DecreaseStaminaBar(int amount)
	{
		//resting job
		if(amount < 0)
		{
			staminaRestoreBar.Show();
		}
		//not resting job
		else
		{
			staminaRestoreBar.Hide();
			//preventing underflow
			if(staminaBar.Value - amount < staminaBar.MinValue)
				staminaBar.Value = staminaBar.MinValue;
			else
				staminaBar.Value -= amount;
		}
	}

	private void IncreaseExpBar(int amount)
	{
		ProgressBar parent = expBar.GetParent<ProgressBar>();

		if(parent.Value + amount >= parent.MaxValue)
			parent.Value = parent.MaxValue;
		else
			parent.Value += amount;
	}

	private void SetupBars()
	{
		Goat goat = GlobalVars.goats[goatID];
		ChangeBarsMinMax(staminaBar, 0, (int)goat.Stamina);
		ChangeBarsMinMax(expBar, 0, 100);	//TODO: adjust to exp min / max
		staminaBar.Value = goat.CurrentStamina;
		staminaBar.GetParent<ProgressBar>().Value = goat.CurrentStamina;

		expBar.Value = goat.Exp;
		expBar.GetParent<ProgressBar>().Value = goat.Exp;
		staminaRestoreBar.Show();
	}

	private void ChangeBarsMinMax(ProgressBar barChild, int min, int max)
	{
		barChild.MinValue = min;
		barChild.MaxValue = max;

		barChild.GetParent<ProgressBar>().MinValue = min;
		barChild.GetParent<ProgressBar>().MaxValue = max;
	}
}
