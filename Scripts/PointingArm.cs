using Godot;
using System.Collections.Generic;
using System;

public partial class PointingArm : Node2D
{

    [Export] public Node2D target;         
    [Export] public float minAngle = -45f; 
    [Export] public float maxAngle = 45f;

    [Export] public float angleModifier = 0; //this is just since the arms aren't 90 degrees by default in their spritemap

    private float lastValidAngle = 0f;

    public override void _Process(double delta)
    {
        if (target == null) return;

        // gets the vector from arm to target
        Vector2 direction = (target.GlobalPosition - GlobalPosition).Normalized();

        float angleDeg = Mathf.RadToDeg(Mathf.Atan2(direction.Y, direction.X));

        
        // adjusts the angle accordngly
        angleDeg += (90 + angleModifier);

        // If angle is inside bounds then apply and store
        if (angleDeg >= minAngle && angleDeg <= maxAngle)
        {
            RotationDegrees = angleDeg;
           // RotationDegrees = Mathf.Lerp(RotationDegrees, angleDeg, 0.1f);// Will maybe add this again later   
            lastValidAngle = angleDeg;
        }
        else
        {
            //otherwise keep  last valid angle
            RotationDegrees = lastValidAngle;
        }
    }
}
