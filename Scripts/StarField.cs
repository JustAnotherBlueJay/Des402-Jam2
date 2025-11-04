using Godot;
using System;
using System.Runtime.Serialization;
using System.Threading;

public partial class StarField : Node2D
{
    //if the stars should be mving right now
    public bool isMoving = false;

    //multi mesh responsible for drawing the stars
    private MultiMeshInstance2D multiMesh;

    //number of stars to spawn
    [Export] private int instanceCount = 25;

    //size of the screen
    Vector2 screenSize = new Vector2(1024, 512);

    //array of every star in this star field
    //this isnt really needed but if people want to do stuff that affects individual stars it might help
    private StarFieldStar[] starFieldStars;

    //direction the stars will move
    Vector2 starDirection = new Vector2 (0f,-1f);

    //current velocity of the stars
    Vector2 starVelocity = Vector2.Zero;

    [Export] Color starColorModulate = new Color(0.5f,0.5f,0.5f,0.75f);
    [Export] float minimumStarScale = 0.1f;
    [Export] float maximumStarScale = 0.2f;
    [Export] float starSpeed = 2f;
    [Export] float starAcceleration = 0.005f;
    [Export] float starDeceleration = 0.01f;

    public override void _Ready()
    {

        //create the starfieldStars array
        starFieldStars = new StarFieldStar[instanceCount];


        //reference the multi mesh and set its instance count
        multiMesh = GetNode<MultiMeshInstance2D>("./MultiMeshInstance2D");
        multiMesh.Multimesh.InstanceCount = instanceCount;

        //creates the stars
        GenerateStars();

        //connect to the game manager signals to know when to start and stop moving
        GameManager.E_ConstellationCompleted += OnConstellationCompleted;
        GameManager.E_NewConstellationLoaded += OnNewConstellationLoaded;

    }


    public override void _PhysicsProcess(double delta)
    {
        //if the stars are ment to be moving, accelerate
        if (isMoving)
        {
            starVelocity = starVelocity.MoveToward(starDirection * starSpeed, starAcceleration);
        }
        //if the stars shouldnt be moving decelerate
        else
        {
            starVelocity = starVelocity.MoveToward(Vector2.Zero, starDeceleration);
        }
        
        //keep moving until velocity = 0
        if (starVelocity != Vector2.Zero)
        {
            MoveStars();
        }
    }

    private void GenerateStars()
    {
        //create every star instance
        for (int i = 0; i < instanceCount; i++)
        {
            //random position and rotation for the star
            Vector2 position = new Vector2((float)GD.RandRange(-512f,512f), (float)GD.RandRange(-256f,256f));
            float rotation = (float)GD.RandRange(0f, 2f * MathF.PI);

            //random scale of 1:1 ratio
            Vector2 scale = new Vector2((float)GD.RandRange(minimumStarScale, maximumStarScale),0f); 
            scale.Y = scale.X ;

            //create the stars transform
            Transform2D initialTransform = new Transform2D(rotation, scale, 0f, position);

            //set this mesh instances position and colour
            multiMesh.Multimesh.SetInstanceTransform2D(i, initialTransform);
            multiMesh.Multimesh.SetInstanceColor(i, starColorModulate);

            //create a new StarFieldStar, this handles the movement of each individual star
            starFieldStars[i] = new StarFieldStar(i,initialTransform);


        }
    }

    //stores the current stars transform when itterating
    Transform2D starTransform;
    private void MoveStars()
    {
        //iterate over every star
        foreach (StarFieldStar star in starFieldStars)
        {
            //tell the star to calculate its new position
            star.Move(starVelocity);

            //get the stars new position
            starTransform = star.GetTransform2D();

            //if not on screen wrap back around
            if (!IsOnScreen(starTransform.Origin)) 
            {
                //set it to the bottom the screen and get the new transform
                star.SetYposition(screenSize.Y / 2); 
                starTransform = star.GetTransform2D();
            }

            //update the multi mesh
            multiMesh.Multimesh.SetInstanceTransform2D(star.id, starTransform);
        }
    }

    //returns if a given vector is within the screen
    private bool IsOnScreen(Vector2 position)
    {
        if ((position.X > -screenSize.X / 2 && position.X < screenSize.X / 2) && (position.Y > -screenSize.Y / 2 && position.Y < screenSize.Y / 2))
        {
            return true;
        }

        return false;
    }

    //when a constellation is completed start moving the stars
    private void OnConstellationCompleted()
    {
        isMoving = true;
    }

    //when a new constellation is loaded stop moving the stars
    private void OnNewConstellationLoaded()
    {
        isMoving = false;
    }
}

