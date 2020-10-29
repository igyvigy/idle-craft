using System;
public struct BlockPos : IEquatable<BlockPos>
{
    public int x;
    public int y;
    public int z;

    public BlockPos(Index3D id, ChunkPos cp)
    {
        this.x = cp.x + id.x;
        this.y = id.y;
        this.z = cp.z + id.z;
    }

    public BlockPos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public bool Equals(BlockPos other)
    {
        return this.x == other.x && this.y == other.y && this.z == other.z;
    }

    public override string ToString()
    {
        return "BlockPos: " + x + " " + y + " " + z;
    }
}