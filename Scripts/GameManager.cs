using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node2D
{
	[Export] private Constellation[] constellations; 
	[Export] private float fadeDuration = 1.5f;

	//Event called when a constellation is completed
	static public Action E_ConstellationCompleted;
	//Event called when a new constellation is loaded
	static public Action E_NewConstellationLoaded;
	private int currentIndex = 0;

	//to make it easier to reference this script
	public static GameManager instance;

	public override void _Ready()
	{
		//setting up the singleton
		instance = this;


		// Hide all constellations at start
		foreach (var c in constellations)
		{
			if (c != null)
				SetConstellationVisible(c, false);
		}

		// Show first constellation
		if (constellations.Length > 0)
			ShowConstellation(currentIndex);
	}

	private async void ShowConstellation(int index)
{
		if (index > constellations.Length -1)
		{
			index = 0;
		}

	var c = constellations[index];

	// Fade out previous constellation if any
	if (currentIndex >= 0 && currentIndex < constellations.Length && currentIndex != index)
	{
		FadeConstellation(constellations[currentIndex], fadeIn: false);
		// Wait for fade out to finish before showing next
		await ToSignal(GetTree().CreateTimer(fadeDuration + 0.5f), "timeout");
		
	}

	// Move to target position (for off-screen handling)
	c.Position = c.TargetPosition;

        c.ResetConstellation();

        FadeConstellation(c, fadeIn: true);

	// connect up completion signal
	c.Connect(Constellation.SignalName.ConstellationComplete,
			  new Callable(this, nameof(OnConstellationFinished)),
			  flags: (uint)ConnectFlags.OneShot);

	// Update current index
	currentIndex = index;
}


	private async void OnConstellationFinished()
{
	
	//constellation completed event called
	E_ConstellationCompleted();

	var c = constellations[currentIndex];

		float delayBetween = 1.5f;
		await ToSignal(GetTree().CreateTimer(fadeDuration + delayBetween), "timeout");

		// fades out current constellation
		FadeConstellation(c, fadeIn: false);

		// Wait for fade duration plus optional extra delay

		delayBetween = 4.5f;
		await ToSignal(GetTree().CreateTimer(fadeDuration + delayBetween), "timeout");
		
		//tell the lines to hide themselves so that they can deal with collision
		c.HideLines();


		// Move onto next constellation
		currentIndex++;
	/*if (currentIndex >= constellations.Length)
	{
		//return;
	}*/

	// fades in next constellation
	E_NewConstellationLoaded();
	ShowConstellation(currentIndex);
}

	private void SetConstellationVisible(Constellation c, bool visible)
{
	if (c == null) return;

	c.Visible = visible;

	if (visible)
		c.Position = c.TargetPosition; // on screen now
	else
		c.Position = new Vector2(c.TargetPosition.X, c.TargetPosition.Y - 2000); // off-screen now
}

	private void FadeConstellation(Constellation c, bool fadeIn)
{
	if (c == null) return;

	if (fadeIn)
	{
		SetConstellationVisible(c, true);
	}

	foreach (Node child in c.GetChildren())
	{
		// Skip the creature sprite
		if (child == c.SeaCreature && fadeIn) 
			continue;

		if (child is CanvasItem canvas)
		{
			var tween = c.CreateTween();
			tween.TweenProperty(canvas, "modulate:a", fadeIn ? 1f : 0f, fadeDuration);
		}
	}
}






	//get the start position of the player for the next constelation
	static public Vector2 GetPlayerStartPosition(int playerID)
	{
		
		Constellation currentConstellation;
		//try and get the next constellation, if we cant get it the array is looping back around
		try
		{
			//the constelation we are transitioning to
			currentConstellation = instance.constellations[instance.currentIndex + 1];
		}
		catch
		{
			currentConstellation = instance.constellations[0];

		}
		if (currentConstellation == null)
		{
			GD.Print("Null");
		}

		//get the position of the player spawn point
		Vector2 playerStartPos = currentConstellation.GetPlayerStartPosition(playerID);

		if (playerStartPos.Y < 0)
		{
			//add 200 cause the constellations get moved off screen
			playerStartPos.Y += 2000;

		}
		
		return playerStartPos;
	}
}
