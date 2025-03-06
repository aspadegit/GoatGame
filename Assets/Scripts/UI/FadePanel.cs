using Godot;
using System;

public partial class FadePanel : Panel
{
	public enum FadeType {
		FADE_OUT = 0,
		FADE_IN = 1
	}

	[Signal]
	public delegate void FinishFadeEventHandler();

	private bool shouldFade = false;
	delegate void Fader();
	Fader func;

    public override void _Ready()
    {
        func = FadeToBlack;
		shouldFade = false;
    }

    public override void _Process(double delta)
	{
		if(shouldFade)
		{
			func.Invoke();
		}
	}

	// sets individual functions to run in Process based on the passed-in type
	public void Fade(FadeType type)
	{
		switch(type)
		{
			case FadeType.FADE_OUT:
				func = FadeToBlack;
				break;
			case FadeType.FADE_IN:
				func = FadeIn;
				break;

			default:
				func = FadeToBlack;
				GD.PrintErr("Unsupported fade provided in FadePanel.cs");
				break;

		}
		shouldFade = true;
	}

	// 0 transparency black screen to 1
	private void FadeToBlack()
	{
		// floating points are inaccurate. 0.98 vs 1 transparency: good enough
		if(Modulate.A >= 0.98)
		{
			Modulate = new Color(Modulate.R, Modulate.B, Modulate.G, 1);
			shouldFade = false;
			EmitSignal(SignalName.FinishFade);
			return;
		}
		Modulate = new Color(Modulate.R, Modulate.B, Modulate.G, (float)Mathf.Lerp(Modulate.A, 1, 0.1)); 
	}

	private void FadeIn()
	{
		shouldFade = false;
		throw new NotImplementedException("Fade-ins on FadePanel.cs not yet implemented.");
	}
}
