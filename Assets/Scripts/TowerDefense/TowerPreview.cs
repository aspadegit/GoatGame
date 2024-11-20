using Godot;

public partial class TowerPreview : Node2D
{
	// Called when the node enters the scene tree for the first time.
	AnimatedSprite2D sprite;
	Sprite2D towerSprite;
	public Pointer pointer;
	private ShaderMaterial shaderMat;

	private readonly Vector4 redOut = new Vector4(0.95f, 0.5f, 0.6f, 0.6f);
	private readonly Vector4 greenOut = new Vector4(0.2f, 0.75f, 0.18f, 0.6f);
	private const float intensity = 0.625f;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("TowerSprite/ShotAnchor/ShotSprite");
		towerSprite = GetNode<Sprite2D>("TowerSprite");
		shaderMat = (ShaderMaterial)towerSprite.Material;

		shaderMat.SetShaderParameter("intensity", intensity);
	}

	public void SetMachine(Machine machineType) {

		//TODO: set machine base sprite

		var frames = GD.Load(GlobalVars.spriteShotPath + machineType.ShotType.TexturePath);
		if(frames != null)
			sprite.SpriteFrames = frames as SpriteFrames;


	}

	public void SetColor(bool canPlace)
	{
		if(canPlace)
			shaderMat.SetShaderParameter("pickedColor", greenOut);
		else
			shaderMat.SetShaderParameter("pickedColor", redOut);

	}


    public override void _Process(double delta)
    {
        Position = pointer.positionWithCamera;
    }

}
