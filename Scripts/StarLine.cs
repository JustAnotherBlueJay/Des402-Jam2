using Godot;
using System;

public partial class StarLine : Node2D
{
	[Export] Sprite2D mySprite;
	[Export] StaticBody2D myStaticBody;

	public bool isVisible = false;


    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.Space))
		{
			GD.Print("Space Ppessed");

			FadeIn();
		}
    }

    public void HideLine()
	{
		isVisible = false;
		
		mySprite.Hide();
		myStaticBody.SetCollisionLayerValue(2,false);

	}

	public void ShowLine()
	{
		isVisible = true;

		Visible = true;

		FadeIn();
		myStaticBody.SetCollisionLayerValue(2,true);
		mySprite.Show();

	}

	private async void FadeIn()
	{

		mySprite.SetInstanceShaderParameter("progress",0f);


		Tween tween = CreateTween();

		tween.TweenProperty(mySprite, "instance_shader_parameters/progress", 1, 1.5f);
		await ToSignal(tween, "finished");
	}
}
