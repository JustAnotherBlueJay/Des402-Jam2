using Godot;
using System;

//script for each star in a StarField
public partial class StarFieldStar : Node
{
    public int id = -1;
    private Transform2D transform;


    //constructor
    public StarFieldStar(int id, Transform2D transform)
    {
        this.id = id;
        this.transform = transform;
    }

    //move this stars transform by velocity
    public void Move(Vector2 velocity)
    {
        transform.Origin += velocity;

    }

    //set the stars position
    public void SetYposition(float position)
    {
        transform.Origin.Y = position;
    }

    //return this stars transform
    public Transform2D GetTransform2D()
    {
        return transform;
    }
}
