using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

[Serializable]
public struct Block : IEquatable<Block?>
{

    // int3x2 
    // c0 x, y, z - pos
    // c1  x - type
    //     y - level
    //     z - reserved

    public const float Height = 1f;

    [SerializeField] public BlockType type;
    [SerializeField] public int level;

    public int3 data
    {
        get
        {
            return new int3((sbyte)type, level, 0);
        }
    }
    public Block(int3 data)
    {
        this.type = (BlockType)data.x;
        this.level = data.y;
    }
    public Block(BlockData data)
    {
        this.type = (BlockType)data.type;
        this.level = data.level;
    }
    public Block(BlockType type, int level)
    {
        this.type = type;
        this.level = level;
    }
    public Block(BlockType type)
    {
        this.type = type;
        this.level = 1;
    }

    public bool Equals(Block? b)
    {
        if (b == null) return false;
        return type == b.Value.type && level == b.Value.level;
    }

    public override string ToString()
    {
        return "Block: " + level + " " + type;
    }
    public static Dictionary<BlockType, float> baseHitpointsForType = new Dictionary<BlockType, float>(){
        {BlockType.Air, 0},
        {BlockType.Grass, 12},
        {BlockType.Dirt, 10},
        {BlockType.Stone, 50},
        {BlockType.Trunk, 20},
        {BlockType.Leaves, 5},
        {BlockType.BedRock, 0},
    };
    public static float GetMaxHealth(int3 block)
    {
        return baseHitpointsForType[(BlockType)block.x] * block.y;// * 10;
    }

    public static Dictionary<BlockType, float> baseRegenForType = new Dictionary<BlockType, float>(){
        {BlockType.Air, 0},
        {BlockType.Grass, 1},
        {BlockType.Dirt, 0},
        {BlockType.Stone, 0},
        {BlockType.Trunk, 0},
        {BlockType.Leaves, 3},
        {BlockType.BedRock, 9999},
    };
    public static float GetRegen(int3 block)
    {
        return baseRegenForType[(BlockType)block.x] * block.y;
    }

    public static Dictionary<BlockType, float> baseRespawnForType = new Dictionary<BlockType, float>(){
        {BlockType.Air, 0},
        {BlockType.Grass, 120},
        {BlockType.Dirt, 120},
        {BlockType.Stone, 300},
        {BlockType.Trunk, 240},
        {BlockType.Leaves, 60},
        {BlockType.BedRock, 9999},
    };
    public static float GetRespawn(int3 block)
    {
        return baseRespawnForType[(BlockType)block.x] * block.y;
    }

    public List<Item> DropItems()
    {
        switch (type)
        {
            case BlockType.Grass:
                return new List<Item> { Item.Make(ItemType.BlockGrass, this.level) };
            case BlockType.Stone:
                return new List<Item> { Item.Make(ItemType.BlockStone, this.level) };
            case BlockType.Dirt:
                return new List<Item> { Item.Make(ItemType.BlockDirt, this.level) };
            case BlockType.Trunk:
                return new List<Item> { Item.Make(ItemType.BlockTrunk, this.level) };
            case BlockType.Leaves:
                return new List<Item> { Item.Make(ItemType.BlockLeaves, this.level) };
            default: return new List<Item>();
        }
    }
}
[Serializable]
public enum BlockType
{
    Air, Dirt, Grass, Stone, Trunk, Leaves, BedRock

}