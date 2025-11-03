using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class PlayerController : CharacterBody2D
{
	[Export] private int playerID;
	
	float spinDuration = 0.5f;
	int spinRevolutions = 1;

	float maxSpeed = 300.0f;

	float acceleration = 10f;
	float deceleration = 30f;
	private bool active = true;
	private bool isSpinning = false;

	private GpuParticles2D starParticles;
	public override void _Ready()
	{
		//connecting to the game managers events
		GameManager.E_ConstellationCompleted += OnConstellationCompleted;
		GameManager.E_NewConstellationLoaded += OnNewConstellationLoaded;

		starParticles = GetNode<GpuParticles2D>("./StarParticles");
	}
	
	public override void _Input(InputEvent @craft)
	{
        if (((Input.IsActionPressed("player_1_spin") && playerID == 0) || (Input.IsActionPressed("player_2_spin") && playerID == 1)) && !isSpinning)
        {
			doSpin();
		}
	}

	private async void doSpin()
	{
		isSpinning = true;

		EmitStars();

		float targetRotation = Rotation + (Mathf.Tau * spinRevolutions);

		var tween = CreateTween();
		tween.TweenProperty(this, "rotation", targetRotation, spinDuration).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);

		await ToSignal(tween, Tween.SignalName.Finished);

		isSpinning = false;

	}
	
	
	public override void _PhysicsProcess(double delta)
	{

		if (!active)
		{
			//stops the player conserving momentum into the next constellation
			Velocity = Vector2.Zero;
			return;
		}


		// Get the input direction and handle the movement/deceleration.

		Vector2 direction = GetInputDirection();

		if (direction != Vector2.Zero)
		{
			//velocity moves towards Direction * maxSpeed by the amount of acceleration  
			Velocity = Velocity.MoveToward(direction * maxSpeed, acceleration);
		}
		else
		{
			// Velocity gradually lowered to 0
			Velocity = Velocity.MoveToward(Vector2.Zero, deceleration);

		}
		//rotate the player by the angle of the players Y vector to velocity(diren of movement)
		Rotate(Transform.Y.AngleTo(Velocity));
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

		EmitStars();
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

	//enables the star explosion particle effect
	private void EmitStars()
	{
		starParticles.Emitting = true;
	}


}
