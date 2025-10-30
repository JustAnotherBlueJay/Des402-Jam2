using Godot;
using System.Collections.Generic;
using System;

public partial class StarPoint : Area2D
{
	[Export] public bool IsTarget = false; 
	[Export] public bool IsComplete = false; 
	[Export] public Color ActiveColor = new Color(1, 1, 0.8f); // glowing yellow
	[Export] public Color InactiveColor = new Color(0.2f, 0.2f, 0.5f); // dim blue
	
	 //[Export] public Godot.Collections.Array<NodePath> ConnectedStarPaths = new();

	//public List<StarPoint> ConnectedStars = new();
	
	private Sprite2D sprite;

	// Signal for Constellation
	[Signal]
	public delegate void StarCompletedEventHandler();

	public override void _Ready()
	{
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
	
	private void OnBodyEntered(Node2D body)
	{
		GD.Print("You has touched star");
		if (IsComplete || !IsTarget)
			return;
			
			

		if (body is PlayerController)
		{
			IsComplete = true;
			UpdateVisual();
			EmitSignal(SignalName.StarCompleted);
		}
	}

	private void UpdateVisual()
	{
		if (sprite == null) return;
		sprite.Modulate = IsComplete ? ActiveColor : InactiveColor;
	}
}
