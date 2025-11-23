using Godot;
using System.Collections.Generic;
using System;

public partial class StarPoint : Area2D
{

    private static float inactiveBrightness = 1.0f;

    [Export] public bool isTarget = false; 
	[Export] public bool isComplete = false; 
	[Export] public Color activeColor = new Color(1, 1, 0.8f); // glowing yellow
	[Export] public Color inactiveColor = new Color(0.2f, 0.2f, 0.7f); // dim blue

	private bool isResetTarget = false;
	
	 //[Export] public Godot.Collections.Array<NodePath> ConnectedStarPaths = new();

	//public List<StarPoint> ConnectedStars = new();
	
	private Sprite2D sprite;

	// Signal for Constellation
	[Signal]
	public delegate void StarCompletedEventHandler();

	public override void _Ready()
	{
		if (isTarget)
		{
			isResetTarget = true;
		}

		//GD.Print(Name, " connections: ", string.Join(", ", ConnectedStars));
		sprite = GetNode<Sprite2D>("Sprite2D");
		UpdateVisual();
		
		/*foreach (var path in ConnectedStarPaths)
		{
			var node = GetNodeOrNull<StarPoint>(path);
			if (node != null)
				ConnectedStars.Add(node);
		}*/

		BodyEntered += OnBodyEntered;
	}

    public void ResetStar()
    {

        GD.Print("me1! " + isResetTarget);

        if (isResetTarget)
        {
			GD.Print("me2!");
            isComplete = false;
			isTarget = true;
        }

		UpdateVisual();
    }

    private void OnBodyEntered(Node2D body)
	{
		if (isComplete || !isTarget)
			return;
			
			

		if (body is PlayerController)
		{
			PlayerController pc = body as PlayerController;
			pc.LockToStar(GlobalPosition, false);

			isComplete = true;
			UpdateVisual();
			EmitSignal(SignalName.StarCompleted);
		}
	}


    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.Equal))
        {
            inactiveBrightness = Mathf.Clamp(inactiveBrightness + 0.0005f, 0.1f, 1);
            UpdateVisual();
        }

        if (Input.IsKeyPressed(Key.Minus))
        {
            inactiveBrightness = Mathf.Clamp(inactiveBrightness - 0.0005f, 0.1f, 1);
            UpdateVisual();
        }
    }

    private void UpdateVisual()
	{
		if (sprite == null)
		{
			return;
		}
		
        if (isComplete)
        {
            sprite.Modulate = activeColor;
        }
        else
        {
            sprite.Modulate = new Color(inactiveColor.R * inactiveBrightness, inactiveColor.G * inactiveBrightness, inactiveColor.B * inactiveBrightness, 1);
        }
    }
}
