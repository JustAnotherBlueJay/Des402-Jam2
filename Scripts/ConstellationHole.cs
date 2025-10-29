using Godot;
using System;

public partial class ConstellationHole : Node2D
{

	private Area2D area;

	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		area = GetNode<Area2D>("./Area2D");

		area.BodyEntered += OnBodyEntered;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnBodyEntered(Node2D body)
	{

		if (body.GetGroups().Contains("Players"))
		{
			GD.Print("Player Entered");

		}
	}
}
