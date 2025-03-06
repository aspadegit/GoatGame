using Godot;
using System;

public partial class SbPauseMenuSettings : MarginContainer
{
	[Export]
	SbPauseMenu parent;

	[Export]
	HSlider masterSlider;

	[Export]
	HSlider effectsSlider;

    public override void _Ready()
    {
		SetSliderValues();
		VisibilityChanged += OnVisChange;
    }

	private void OnVisChange()
	{
		if(IsVisibleInTree())
		{
			SetSliderValues();
		}
	}



    public void OnMasterSlider(float value)
	{
		GlobalVars.settings.masterVolume = ConvertValue(value);
	}

	public void OnFxSlider(float value)
	{
		GlobalVars.settings.effectsVolume = ConvertValue(value);
	}

	private float ConvertValue(float value)
	{
		
		return value / 100;
	}

	private void SetSliderValues()
	{
        masterSlider.Value = GlobalVars.settings.masterVolume * 100;
        effectsSlider.Value = GlobalVars.settings.effectsVolume * 100;

	}
}
