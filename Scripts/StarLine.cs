using Godot;
using System;

public partial class StarLine : Node2D
{
    [Export] Sprite2D mySprite;
    [Export] StaticBody2D myStaticBody;



    public override void _Ready()
    {
    }

    public void HideLine()
    {
        mySprite.Hide();
        myStaticBody.SetCollisionLayerValue(2,false);
    }

    public void ShowLine()
    {
        GD.Print("Showing Sprite");
        Visible = true;
        myStaticBody.SetCollisionLayerValue(2,true);
        mySprite.Show();
    }
}
