using Godot;
using System;
using System.Collections.Generic;

public partial class Constellation : Node2D
{
	[Signal]
	public delegate void ConstellationCompleteEventHandler();
	
	[Export] public Vector2 TargetPosition = new Vector2(0, 0);
	
	
	//rules for Jason (and me) on how to make more constellations
	[Export] private Node2D creature;               // drag the node holding sea creature and constellation name here
	[Export] private StarPoint[] stars;                // drag all StarPoints here
	[Export] private Node2D[] lines;                   // drag all Lines here
	[Export] private StarPoint[] lineStarA;            // For each line, drag its "start" star
	[Export] private StarPoint[] lineStarB;            // For each line, drag its "end" star

	//[Export] public AudioStream fadeInSound;
	//private AudioStreamPlayer soundPlayer;

	[Export] private Node2D[] playerStartPositions; 	//where the player will spawn when the constellation is loaded
	
	public Node2D SeaCreature => creature;

	public override void _Ready()
	{

		//soundPlayer = new AudioStreamPlayer();
		//AddChild(soundPlayer);

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
		foreach (StarLine line in lines)
		{
			line.HideLine();
		}

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
			if (s.isTarget && !s.isComplete)
				return false;
		}
		return true;
	}

	public void UpdateLines()
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
			if (a.isComplete && b.isComplete)
			{
				//try and see if the lines are StarLines, if they are show them that way
				//if they are the old lines then show them that way
				try
				{
				
					StarLine starLine = line as StarLine;

					if (!starLine.isVisible) {starLine.ShowLine();}
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

		// GD.Print($"Showing {linesShown} lines.");
	}

	private async void FadeCreature(bool fadeIn)
	{
		if (creature == null) return;

		GetNode<AudioStreamPlayer>("AudioStreamPlayer").Play();

		var tween = CreateTween();
		tween.TweenProperty(creature, "modulate:a", fadeIn ? 1f : 0f, 2f);
		await ToSignal(tween, Tween.SignalName.Finished);
	}


    public void ResetConstellation()
    {
        foreach (var s in stars)
        {
            if (s == null) continue;
            s.ResetStar(); 
        }

		foreach (StarLine starLine in lines)
		{
            if (starLine == null) continue;
			starLine.SetOneWayCollision(false);
		}
		UpdateLines();

        // Hide creature again
        //if (creature != null)
           // creature.Modulate = new Color(1, 1, 1, 0);
    }



    public Vector2 GetPlayerStartPosition(int playerID)
	{
		return playerStartPositions[playerID].GlobalPosition;

	}

	public void HideLines()
	{
		foreach (StarLine starLine in lines)
		{
			starLine.HideLine();
		}
	}
}
