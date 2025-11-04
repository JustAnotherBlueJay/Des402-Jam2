using Godot;
using System;
using System.Security.AccessControl;

public partial class Trail : Line2D
{
    
    [Export] private int maxLength = 20;
    [Export] private CharacterBody2D player;
    private bool isActive = true;

    public override void _Process(double delta)
    {   
        if (isActive)
        {
            AddPoint();
        }

    }

    private void AddPoint()
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

    public void HideTrail()
    {
        Points = new Vector2[0];

        isActive = false;
    }

    public void ShowTrail()
    {
        isActive = true;
    }
}
