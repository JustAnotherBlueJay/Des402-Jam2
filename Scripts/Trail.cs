using Godot;
using System;
using System.Security.AccessControl;

public partial class Trail : Line2D
{
    
    [Export] private int maxLength = 20;
    [Export] private CharacterBody2D player;

    public override void _Process(double delta)
    {   
        //add new trail point at player pos
        Vector2 position = player.GlobalPosition;
        AddPoint(position);

        //remove the oldest point when max length is reached
        if (Points.Length > maxLength)
        {
            RemovePoint(0);
        }
    }
}
