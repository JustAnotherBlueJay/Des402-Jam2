using Godot;
using System;
using System.Collections.Generic;

public partial class Constellation : Node2D
{
	[Signal]
	public delegate void ConstellationCompleteEventHandler();
	
	[Export] public Vector2 TargetPosition = new Vector2(0, 0);
	
	
	//rules for Jason (and me) on how to make more constellations
	[Export] private Sprite2D creature;               // drag the current sea creature sprite here
	[Export] private StarPoint[] stars;                // drag all StarPoints here
	[Export] private Node2D[] lines;                   // drag all Lines here
	[Export] private StarPoint[] lineStarA;            // For each line, drag its "start" star
	[Export] private StarPoint[] lineStarB;            // For each line, drag its "end" star

	[Export] private Node2D[] playerStartPositions; 	//where the player will spawn when the constellation is loaded
	
	public Sprite2D SeaCreature => creature;

	public override void _Ready()
	{
		// Hide our sea creature initialy
		if (creature != null)
			creature.Modulate = new Color(1, 1, 1, 0);

		// Connect star signals
		foreach (var s in stars)
		{
			if (s != null)
				s.StarCompleted += OnStarCompleted;
		}

		// Hide all lines at start
		foreach (var l in lines)
		{
			if (l != null)
				l.Visible = false;
		}

		//calling the lines group to hide on start
		GetTree().CallGroup("Lines","HideLine");
		
		UpdateLines();

		GD.Print($" Found {stars.Length} stars and {lines.Length} lines.");
	}

	private void OnStarCompleted()
	{
		GD.Print("Star completed signal received!");
		UpdateLines();

		if (AllTargetsComplete())
		{
			GD.Print("Constellation complete!");
			FadeCreature(true);
			EmitSignal(SignalName.ConstellationComplete);
		}
	}

	private bool AllTargetsComplete()
	{
		foreach (var s in stars)
		{
			if (s.IsTarget && !s.IsComplete)
				return false;
		}
		return true;
	}

	private void UpdateLines()
	{
		int linesShown = 0;

		for (int i = 0; i < lines.Length; i++)
		{
			var line = lines[i];
			if (line == null || i >= lineStarA.Length || i >= lineStarB.Length)
				continue;

			var a = lineStarA[i];
			var b = lineStarB[i];

			if (a == null || b == null)
				continue;

			// Show line if both stars are complete
			if (a.IsComplete && b.IsComplete)
			{
				//try and see if the lines are StarLines, if they are show them that way
				//if they are the old lines then show them that way
				try
				{
					StarLine starLine = line as StarLine;
					starLine.ShowLine();
				}
				catch(Exception)
				{
					line.Visible = true;
					linesShown++;

				}
			}
			else
			{
				line.Visible = false;
			}
		}

		GD.Print($"Showing {linesShown} lines.");
	}

	private async void FadeCreature(bool fadeIn)
	{
		if (creature == null) return;

		var tween = CreateTween();
		tween.TweenProperty(creature, "modulate:a", fadeIn ? 1f : 0f, 2f);
		await ToSignal(tween, Tween.SignalName.Finished);
	}

	public Vector2 GetPlayerStartPosition(int playerID)
	{
		return playerStartPositions[playerID].GlobalPosition;

	}
}
