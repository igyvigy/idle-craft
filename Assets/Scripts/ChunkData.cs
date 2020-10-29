[System.Serializable]

struct ChunkData
{
    public ChunkPos pos;
    public BlockData[,,] blocks;

    public ChunkData(ChunkPos pos, BlockData[,,] blocks)
    {
        this.pos = pos;
        this.blocks = blocks;
    }
}