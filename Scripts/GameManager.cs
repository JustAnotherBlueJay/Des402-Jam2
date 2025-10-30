using Godot;
using System.Collections.Generic;

public partial class GameManager : Node2D
{
	[Export] private Constellation[] constellations; 
	[Export] private float fadeDuration = 1.5f;

	private int currentIndex = 0;

	public override void _Ready()
	{
		GD.Print($"[GM] Ready. Total constellations = {constellations.Length}");

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
	if (index < 0 || index >= constellations.Length) 
		return;

	var c = constellations[index];
	GD.Print($"[GM] ShowConstellation({index})");

	// Fade out previous constellation if any
	if (currentIndex >= 0 && currentIndex < constellations.Length && currentIndex != index)
	{
		FadeConstellation(constellations[currentIndex], fadeIn: false);
		// Wait for fade out to finish before showing next
		await ToSignal(GetTree().CreateTimer(fadeDuration + 0.5f), "timeout");
	}

	// Move to target position (for off-screen handling)
	c.Position = c.TargetPosition;

	// Fade in visuals
	FadeConstellation(c, fadeIn: true);

	// Connect completion signal
	c.Connect(Constellation.SignalName.ConstellationComplete,
			  new Callable(this, nameof(OnConstellationFinished)),
			  flags: (uint)ConnectFlags.OneShot);

	// Update current index
	currentIndex = index;
}


	private async void OnConstellationFinished()
{
	GD.Print("[GM] Constellation finished!");

	var c = constellations[currentIndex];

	// fades out current constellation
	FadeConstellation(c, fadeIn: false);

	// Wait for fade duration plus optional extra delay
	float delayBetween = 4.5f; // seconds
	await ToSignal(GetTree().CreateTimer(fadeDuration + delayBetween), "timeout");
	
	

	// Move onto next constellation
	currentIndex++;
	if (currentIndex >= constellations.Length)
	{
		GD.Print("[GM] All constellations complete!");
		return;
	}

	// fades in next constellation
	ShowConstellation(currentIndex);
}

	private void SetConstellationVisible(Constellation c, bool visible)
{
	if (c == null) return;

	c.Visible = visible;

	if (visible)
		c.Position = c.TargetPosition; // on screen now
	else
		c.Position = new Vector2(c.TargetPosition.X, -1000); // off-screen now
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
		if (child == c.SeaCreature) 
			continue;

		if (child is CanvasItem canvas)
		{
			var tween = c.CreateTween();
			tween.TweenProperty(canvas, "modulate:a", fadeIn ? 1f : 0f, fadeDuration);
		}
	}
}
}
