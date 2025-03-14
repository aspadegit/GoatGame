using Godot;
using System;

public partial class UiSfxPlayer : Control
{
	[Export]
	AudioStreamPlayer2D stream;

	[Export]
	AudioStreamWav hover;

	[Export]
	AudioStreamWav click;

	[Export]
	AudioStreamWav slide;

	private Timer tickTimer;
	
	private Control parent;
	private const float DEFAULT_DB = 2;

	private bool playingTick = false;

	public override void _Ready()
	{
		parent = GetParent<Control>();
		tickTimer = GetNode<Timer>("Timer");
		parent.FocusExited += OnFocus;
		parent.MouseEntered += OnFocus;

		if(parent is Button)
		{
			Button buttonParent = (Button)parent;
			buttonParent.Pressed += OnClick;
		}
		else if(parent is Slider)
		{
			Slider slideParent = (Slider)parent;
			slideParent.Connect("value_changed", Callable.From((float value) => OnChange(value)));
		}
	}

	// when hovering
	public void OnFocus()
	{
		if(parent.IsVisibleInTree())
			PlayStream(hover);
	}

	// when sliders slide
	public void OnChange(float value)
	{
		if(parent.IsVisibleInTree() && !playingTick)
		{
			PlayTick();
			PlayStream(slide);

		}
	}

	public void OnClick()
	{
		PlayStream(click);
	}

	public void TimerTimeout()
	{
		playingTick = false;
	}

	private void PlayStream(AudioStreamWav s)
	{
		stream.Stream = s;
		SetVolume();
		stream.Play();
			
	}

	// prevents slider ticks from playing too fast over each other
	private void PlayTick()
	{
		playingTick = true;
		tickTimer.Start();
		
	}

	private void SetVolume()
	{
		float percentToDecibel = 10 * (float)Math.Log10(GlobalVars.settings.effectsVolume * GlobalVars.settings.masterVolume);
		stream.VolumeDb = DEFAULT_DB + percentToDecibel;
	}
}
