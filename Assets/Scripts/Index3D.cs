using System;
public struct Index3D: IEquatable<Index3D>
{
    public int x, y, z;
    public Index3D(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public bool Equals(Index3D other)
    {
        return this.x == other.x && this.y == other.y && this.z == other.z;
    }
    public override string ToString()
    {
        return "Index3D: " + x + " " + y + " " + z;
    }
}