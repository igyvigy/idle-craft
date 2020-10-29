using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

public struct TilePos
{
    int xPos, yPos;
    Vector2[] uvs;

    public TilePos(int xPos, int yPos)
    {
        this.xPos = xPos;
        this.yPos = yPos;
        uvs = new Vector2[4];
        uvs[0] = new Vector2(xPos / 16f + .001f, yPos / 16f + .001f);
        uvs[1] = new Vector2(xPos / 16f + .001f, (yPos + 1) / 16f - .001f);
        uvs[2] = new Vector2((xPos + 1) / 16f - .001f, (yPos + 1) / 16f - .001f);
        uvs[3] = new Vector2((xPos + 1) / 16f - .001f, yPos / 16f + .001f);
    }

    public Vector2[] GetUVs()
    {
        return uvs;
    }
    public static TilePos Get(Tile tileType, int tileGrade)
    {

        switch (tileType)
        {
            case Tile.Dirt:
                switch (tileGrade)
                {
                    case 1: return new TilePos(2, 0);
                    case 2: return new TilePos(4, 0);
                    default: return new TilePos(0, 0);
                }
            case Tile.Grass:
                switch (tileGrade)
                {
                    case 1: return new TilePos(3, 0);
                    case 2: return new TilePos(5, 0);
                    default: return new TilePos(1, 0);
                }
            case Tile.GrassSide:
                switch (tileGrade)
                {
                    case 1: return new TilePos(2, 1);
                    case 2: return new TilePos(4, 1);
                    default: return new TilePos(0, 1);
                }
            case Tile.Stone:
                switch (tileGrade)
                {
                    case 1: return new TilePos(3, 1);
                    case 2: return new TilePos(5, 1);
                    default: return new TilePos(1, 1);
                }
            case Tile.TreeSide:
                switch (tileGrade)
                {
                    default: return new TilePos(0, 4);
                }
            case Tile.TreeCX:
                switch (tileGrade)
                {
                    default: return new TilePos(0, 3);
                }
            case Tile.Leaves:
                switch (tileGrade)
                {
                    default: return new TilePos(0, 5);
                }
            case Tile.Bedrock:
                switch (tileGrade)
                {
                    default: return new TilePos(0, 2);
                }
            default: return new TilePos(0, 0);
        }
    }

}

public enum Tile { Dirt, Grass, GrassSide, Stone, TreeSide, TreeCX, Leaves, Bedrock }
