using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item : IEquatable<Item>
{
    public ItemType type;
    public string name;
    public string description;
    public Sprite image;
    public int level;
    public Item(ItemType type, string name, string description, Sprite image, int level)
    {
        this.type = type;
        this.name = name;
        this.description = description;
        this.image = image;
        this.level = level;
    }

    public override string ToString()
    {
        return string.Format("Item -> type {0}, level {1}", type, level);
    }
    public static Item Make(ItemType type, int level = 1)
    {
        return new Item(type, GetName(type), GetDescription(type), GetSprite(type), level);
    }

    public static Sprite GetSprite(ItemType type)
    {
        switch (type)
        {
            case ItemType.BlockGrass:
                return ItemAssets.i.grassBlock;
            case ItemType.BlockDirt:
                return ItemAssets.i.dirtBlock;
            case ItemType.BlockStone:
                return ItemAssets.i.stoneBlock;
            case ItemType.BlockTrunk:
                return ItemAssets.i.trunkBlock;
            case ItemType.BlockLeaves:
                return ItemAssets.i.leafBlock;
            default: return ItemAssets.i.dirtBlock;
        }
    }

    public static string GetName(ItemType type)
    {
        switch (type)
        {
            case ItemType.BlockGrass:
                return "Grass Block";
            case ItemType.BlockDirt:
                return "Dirt Block";
            case ItemType.BlockStone:
                return "Stone Block";
            case ItemType.BlockTrunk:
                return "Trunk Block";
            case ItemType.BlockLeaves:
                return "Leafs Block";
            default: return "???";
        }
    }
    public static string GetDescription(ItemType type)
    {
        switch (type)
        {
            case ItemType.BlockGrass:
                return "Block of grass. Can be placed in the world";
            case ItemType.BlockDirt:
                return "Dirt Block. Can be placed in the world";
            case ItemType.BlockStone:
                return "Stone Block. Can be placed in the world";
            case ItemType.BlockTrunk:
                return "Trunk Block. Can be placed in the world";
            case ItemType.BlockLeaves:
                return "Block of leaves. Can be placed in the world";
            default: return "???";
        }
    }

    public static Block? GetWoldBLock(ItemType type, int level)
    {
        switch (type)
        {
            case ItemType.BlockGrass:
                return new Block(BlockType.Grass, level);
            case ItemType.BlockDirt:
                return new Block(BlockType.Dirt, level);
            case ItemType.BlockStone:
                return new Block(BlockType.Stone, level);
            case ItemType.BlockTrunk:
                return new Block(BlockType.Trunk, level);
            case ItemType.BlockLeaves:
                return new Block(BlockType.Leaves, level);
            default: return null;
        }
    }

    public bool Equals(Item other)
    {
        if (other == null) return false;
        return this.type == other.type && this.level == other.level;
    }
}
