using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerController : CharacterBody2D
{
	[Export] private int playerID;

	float speed = 300.0f;
	private bool active = true;


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

	public void LockToStar(Vector2 starPos)
	{
		active = false;

		Position = starPos;
	}
}
