using Godot;
using System;


public partial class Enemy : Node2D
{
	private ProgressBar health_bar;
	private ProgressBar damage_bar;
	private Timer timer;
	private PathFollow2D pathFollow;
	private AnimatedSprite2D animation;
	private ShaderMaterial shaderMat;
	private Vector2 prevPosition;
	bool shouldStart = false;
	float deathDissolveVal = 0.0f;
	bool dying = false;
	public int Health { get; private set; 
	}


	public override void _Ready()
	{
		dying = false;
		animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		health_bar = GetNode<ProgressBar>("HealthBar");
		damage_bar = GetNode<ProgressBar>("DamageBar");
		timer = GetNode<Timer>("Timer");
		shaderMat = (ShaderMaterial)animation.Material;
		Health = 300;
		health_bar.Value = Health;
		health_bar.MaxValue = Health;
		damage_bar.Value = Health;
		damage_bar.MaxValue = Health;
	}

	public void Setup(PathFollow2D enemyPathFollow, int enemyNum)
	{
		//set up variables
		pathFollow = enemyPathFollow;
		pathFollow.ProgressRatio = 0;
		Position = pathFollow.Position;
		prevPosition = Position; 

		//set names
		Name = Name + "_" + enemyNum;
		pathFollow.Name = pathFollow.Name + "_" + enemyNum;

		shouldStart = true;
	}

	public override void _Process(double delta)
	{
		health_bar.Value = Health;
		
		if(dying)
			animation.Stop();
		if(shouldStart && !dying)
		{
			prevPosition = Position;	//set old position

			//set new position
			pathFollow.ProgressRatio += 0.05f * (float)delta;
			Position = pathFollow.Position;
			AdjustAnimation();
		}
	}
		
		
	public override void _PhysicsProcess(double delta)
	{
		if(dying)
		{
			deathDissolveVal += 1.5f * (float)delta;
			shaderMat.SetShaderParameter("dissolveState", deathDissolveVal);

			if(deathDissolveVal >= 1)
				Destroy();
		}
	}

	//changes the animation based on the instantaneous velocity 
	private void AdjustAnimation()
	{
		Vector2 velocity = Position - prevPosition;

		if(velocity.Length() > 0)
			animation.Play();
		else
			animation.Stop();

		//left/right
		if(Health > 50 && Math.Abs(velocity.X) > Math.Abs(velocity.Y))
		{
			animation.Animation = "walk_side";
			//right
			if(velocity.X > 0)
			{
				animation.FlipH = false;
			}
			//left
			else
			{
				animation.FlipH = true;
			}

		}
		//up/down
		else if(Health > 50)
		{
			animation.FlipH = false;
			//down
			if(velocity.Y > 0)
			{
				animation.Animation = "walk_down";
			}
			//up
			else
			{
				animation.Animation = "walk_up";

			}
		}
		else
		{
			animation.Animation = "damage";
		}
	}
	
	 public void TakeDamage(int damage)
	{
		Health -= damage;
		health_bar.Value = Health;
		if (Health <= 0)
		{
			StartDying();
		}
	}
		
	private void StartDying()
	{
		dying = true;
		GetNode<CpuParticles2D>("DeathParticles").Emitting = true;
	}

	//deletes itself and its path
	private void Destroy()
	{

		pathFollow.QueueFree();
		QueueFree();

	}

	//destroy when no longer visible
	private void OnVisibleOnScreenNotifier2DScreenExited()
	{
		Destroy();
	}
}



