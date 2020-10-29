using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct BlockChunkComponent : ISharedComponentData
{
    public int4 Chunk;
}
