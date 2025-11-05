using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Ocean : Sprite2D
{
    [Export] PlayerController player1;
    [Export] PlayerController player2;
    Vector2 textureSize;

    private ShaderMaterial myShader;
    public override void _Ready()
    {
        //size of the ocean image, used for converting from local space to UV space
        textureSize = new Vector2(Texture.GetWidth() * Scale.X,Texture.GetHeight() * Scale.Y);

        myShader = (ShaderMaterial)this.Material;

    }

    public override void _Process(double delta)
    {
        //get the players positions in UV sapce
        Vector2 p1UV = GetPlayerPosAsUV(player1);
        Vector2 p2UV = GetPlayerPosAsUV(player2);

        //Update the shaders with the new positions
        myShader.SetShaderParameter("player_1_pos", p1UV);
        myShader.SetShaderParameter("player_2_pos", p2UV);


    }

    private Vector2 GetPlayerPosAsUV(PlayerController player)
    {
        //get player position in local space
        Vector2 playerPosition = player.GlobalPosition - GlobalPosition;

        //return player position in UV space
        return new Vector2((playerPosition.X / textureSize.X) + 0.5f, (playerPosition.Y / textureSize.Y) + 0.5f);

    }
}
