using Godot;
using System;

public partial class TextEntry : Control
{

	private Timer timer;
	private TextureButton keyboardFocus;
	
	[Export]
	public Label innerText;
	[Export]
	public Label blinkLabel;

	[Export]
	public bool lettersAllowed;

	[Export]
	public bool numbersAllowed;

	[Export]
	public int lengthLimit;

	[Export]
	public Node rowParent; // optional, currently used for ShopRow

	private bool isHover = false;
	private bool clickedFocus = false;
	private bool blinkState = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer = GetNode<Timer>("BlinkTimer");
		keyboardFocus = GetNode<TextureButton>("KeyboardFocus");
	}

	public override void _Process(double delta)
	{
		// a click anywhere else will remove the focus on the text box
		if (Input.IsActionPressed("left_click"))
		{
			if(!isHover)
			{
				EndFocus();
			}
			else
			{
				OnClick();
			}
		}

		if(Input.IsActionPressed("escape") || Input.IsActionPressed("ui_text_submit"))
		{
			EndFocus();
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (clickedFocus && @event is InputEventKey keyEvent && keyEvent.Pressed)
		{

			string text = innerText.Text;

			// delete characters from the end
			if(keyEvent.KeyLabel.ToString() == "Backspace")
			{
				if(text.Length > 0)
				{
					innerText.Text = text.Substr(0, text.Length-1);
				}
				SignalHandler.Instance.EmitSignal(SignalHandler.SignalName.TextInputChanged, this);
			}
			// not backspace, so we're adding characters
			else if(text.Length < lengthLimit)
			{
				//space is the only "letter" special character allowed
				if(lettersAllowed && keyEvent.KeyLabel.ToString() == "Space")
				{
					innerText.Text = text + " ";
					
				}
				// normal letters & digits
				else
				{
					CheckCharacter(keyEvent, text);
				}

				SignalHandler.Instance.EmitSignal(SignalHandler.SignalName.TextInputChanged, this);

			}
				
			
		}
	}

	public void OnClick()
	{
		clickedFocus = true;
		blinkLabel.Show();
		timer.Start();
		keyboardFocus.CallDeferred("grab_focus");
		Blink();
	}

	public void OnHoverEnter()
	{
		isHover = true;

		if(!clickedFocus)
		{
			Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0.5f);
		}


	}

	public void OnHoverLeave()
	{
		isHover = false;
		Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 1f);

	}

	public void OnTimeout()
	{
		Blink();
	}

	// connected from a parent that has increase & decrease buttons
	public void Increase()
	{
		ChangeNumberValue(1);
	}

	public void Decrease()
	{
		ChangeNumberValue(-1);
	}

	private void ChangeNumberValue(int multiplier)
	{
		if(!lettersAllowed && numbersAllowed)
		{
			int parsedNum;
			if(innerText.Text.Length < 1)
			{
				innerText.Text = "00";
			}
			else if(Int32.TryParse(innerText.Text, out parsedNum))
			{
				parsedNum += 1 * multiplier;

				parsedNum = Math.Clamp(parsedNum, 0, 99);

				string amount = (parsedNum < 10)? ("0" + parsedNum) : parsedNum.ToString();

				innerText.Text = amount;

				SignalHandler.Instance.EmitSignal(SignalHandler.SignalName.TextInputChanged, this);

			}
			else
			{
				GD.PrintErr("Integer could not be parsed in ChangeNumberValue in TextEntry.cs.");
			}

		}
	}

	private void Blink()
	{
		if(clickedFocus)
		{
			if(!blinkState)
			{
				blinkLabel.SelfModulate = new Color(1,1,1,0.8f);
				blinkState = true;
			}
			else
			{
				blinkLabel.SelfModulate = new Color(1,1,1,0);
				blinkState = false;
			}
		}
	}

	private void CheckCharacter(InputEventKey keyEvent, string text)
	{
		char character = (char)keyEvent.Unicode;
		
		// checks to make sure it's not a special character
		if(Char.IsLetterOrDigit(character))
		{
			//can only be one character long so don't need to check if both
			if(numbersAllowed && Char.IsNumber(character))
			{
				innerText.Text = text + character;
			}

			if(lettersAllowed && Char.IsLetter(character))
			{
				innerText.Text = text + character;
			}
		}
	}
	

	private void EndFocus()
	{
		if(clickedFocus)
			ChangeNumberValue(0); // reformat if it's only numbers allowed (already checks in the func)

		timer.Stop();
		blinkLabel.Hide();
		// turn off the | if it's on
		if(blinkState)
			Blink();

		clickedFocus = false;
	}
}
