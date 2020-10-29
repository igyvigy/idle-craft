using UnityEngine;
using Unity.Jobs;

[System.Serializable]
public struct BlockData
{
    public int level;
    public int type;

    public BlockData(BlockType type)
    {
        this.type = (sbyte)type;
        this.level = 1;
    }

    public BlockData(Block block)
    {
        this.type = (sbyte)block.type;
        this.level = block.level;
    }
}
