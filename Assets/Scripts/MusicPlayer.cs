using Godot;
using System;

public partial class MusicPlayer : AudioStreamPlayer2D
{
	private const float DEFAULT_DB = -6;
	public override void _Ready()
	{
		SetVolume();
		SignalHandler.Instance.Connect(SignalHandler.SignalName.SettingsChanged, Callable.From(()=>SetVolume()));
	}

	private void SetVolume()
	{
		
		float percentToDecibel = 10 * (float)Math.Log10(GlobalVars.settings.masterVolume * GlobalVars.settings.musicVolume);
		VolumeDb = DEFAULT_DB + percentToDecibel;	//percentToDecibel is the CHANGE from the default
		
	}
}
