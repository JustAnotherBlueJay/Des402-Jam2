using Godot;
using System;

public partial class SoftlockSpot : Area2D
{
    //the star point that causes the softlock
    [Export] StarPoint starpoint;

    // the lines to be made one way
    [Export] StarLine[] starlines;

    private bool isPlayerInArea = false;

    public override void _Ready()
    {
        //connect to the areas signals
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;

    }

    //shouldnt be in process but was easier
    public override void _Process(double delta)
    {
        if (starpoint.isComplete && isPlayerInArea)
        {
            EnableOneWayCollisions();
        }
    }

    private void EnableOneWayCollisions()
    {
        foreach (StarLine starline in starlines)
        {
            starline.SetOneWayCollision(true);
        }
    }

    //when a player enters the area2D set isPlayerInArea to true
    private void OnBodyEntered(Node2D body)
    {
        if (body is PlayerController)
        {
            isPlayerInArea = true;
        }
    }

    //when a player exits the area2D set isPlayerInArea to false
    private void OnBodyExited(Node2D body)
    {
        if (body is PlayerController)
        {
            isPlayerInArea = true;
        }
    }
}
