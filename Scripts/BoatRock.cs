using Godot;
using System;
using System.Collections.Generic;

public partial class BoatRock : Node2D
{

    [Export] public float bobAmplitude = 6;
    [Export] public float bobSpeed = 1.2f;
    [Export] public float rockAmplitude = 4;
    [Export] public float rockSpeed = 0.9f;
    //changeable in editor of course, I've softened them up a little therw

    private Vector2 startPos;
    private float time;

    public override void _Ready()
    {
        startPos = GlobalPosition;
    }

    public override void _Process(double delta)
    {
        time += (float)delta;

        
        float y = Mathf.Sin(time * bobSpeed) * bobAmplitude;
        float angle = Mathf.Sin(time * rockSpeed) * rockAmplitude;

        GlobalPosition = new Vector2(startPos.X, startPos.Y + y);
        RotationDegrees = angle;

    }
}