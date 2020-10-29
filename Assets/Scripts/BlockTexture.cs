using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BlockTiles
{
    public Tile top, side, bottom;
    public BlockTiles(Tile tile)
    {
        top = side = bottom = tile;
    }

    public BlockTiles(Tile top, Tile side, Tile bottom)
    {
        this.top = top;
        this.side = side;
        this.bottom = bottom;
    }
}

public struct BlockTexture
{
    public BlockTiles tiles;
    public TilePos topPos, sidePos, bottomPos;
    public int grade;

    public BlockTexture(BlockTiles tiles, int grade)
    {
        this.grade = grade;
        this.tiles = tiles;
        topPos = TilePos.Get(tiles.top, grade);
        sidePos = TilePos.Get(tiles.side, grade);
        bottomPos = TilePos.Get(tiles.bottom, grade);
    }

    public static BlockTexture Get(BlockType blockType, int blockGrade)
    {
        return new BlockTexture(blockTexturesByBlockType[blockType], blockGrade);
    }

    public static Dictionary<BlockType, BlockTiles> blockTexturesByBlockType = new Dictionary<BlockType, BlockTiles>(){
        {BlockType.Dirt, new BlockTiles(Tile.Dirt)},
        {BlockType.Grass, new BlockTiles(Tile.Grass, Tile.GrassSide, Tile.Dirt)},
        {BlockType.Stone, new BlockTiles(Tile.Stone)},
        {BlockType.Trunk, new BlockTiles(Tile.TreeCX, Tile.TreeSide, Tile.TreeCX)},
        {BlockType.Leaves, new BlockTiles(Tile.Leaves)},
        {BlockType.BedRock, new BlockTiles(Tile.Bedrock)},
    };
}