using Unity.Collections;

[System.Serializable]
public struct BlockPositionData
{
    public BlockData data;
    public int[] pos;

    public BlockPositionData(BlockData data, int[] pos)
    {
        this.data = data;
        this.pos = pos;
    }

    public static BlockData[,,] makeDict(NativeList<BlockPositionData> list)
    {
        var dict = new BlockData[Chunk.Width + 2, Chunk.Width, Chunk.Width + 2];
        foreach (var data in list)
        {
            dict[data.pos[0], data.pos[1], data.pos[2]] = data.data;
        }
        return dict;
    }
}