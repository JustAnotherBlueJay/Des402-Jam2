using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class PlayerController : CharacterBody2D
{
	[Export] private int playerID;

	float speed = 300.0f;
	private bool active = true;

    public override void _Ready()
    {
		//connecting to the game managers events
		GameManager.E_ConstellationCompleted += OnConstellationCompleted;
		GameManager.E_NewConstellationLoaded += OnNewConstellationLoaded;
    }
    public override void _PhysicsProcess(double delta)
	{

		if (!active)
		{
			return;
		}

		Vector2 velocity = Velocity;

		// Get the input direction and handle the movement/deceleration.

		Vector2 direction = GetInputDirection();

		if (direction != Vector2.Zero)
		{
			velocity = new Vector2(direction.X * speed, direction.Y * speed);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, speed);

		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private Vector2 GetInputDirection()
	{
		switch (playerID)
		{
			case 0:
				return Input.GetVector("player_1_move_left", "player_1_move_right", "player_1_move_up", "player_1_move_down");

			case 1:
				return Input.GetVector("player_2_move_left", "player_2_move_right", "player_2_move_up", "player_2_move_down");
		}

		return Vector2.Zero;
	}

	public void LockToStar(Vector2 starPos, bool setActive)
	{
		active = setActive;

		Position = starPos;
	}

	public async void OnConstellationCompleted()
	{
		//when a constelation is completed fade the player out then move them to the start position for the next constelation
		await FadeOut();

		//Update positions
		Position = GameManager.GetPlayerStartPosition(playerID);

	}

	public async void OnNewConstellationLoaded()
	{
		//when a new constelation loads in show the player again and set them active
		await FadeIn();

		active = true;
	}

	//gradually fades the player out
	private async Task FadeOut()
	{
		Tween tween = CreateTween();

		tween.TweenProperty(this, "modulate:a", 0, 1.5f);
		await ToSignal(tween, "finished");

	}

	//gradually fades the player in
	private async Task FadeIn()
	{
		Tween tween = CreateTween();

		tween.TweenProperty(this, "modulate:a", 1, 1.5f);
		await ToSignal(tween, "finished"); 
	}


}
