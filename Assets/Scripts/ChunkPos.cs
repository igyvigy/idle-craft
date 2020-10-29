using System;

[Serializable]
public struct ChunkPos : IEquatable<ChunkPos>
{
    public int x, z;
    public ChunkPos(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public bool Equals(ChunkPos other)
    {
        return this.x == other.x && this.z == other.z;
    }
    public override string ToString()
    {
        return "ChunkPos: " + x + " " + z;
    }
}