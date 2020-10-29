using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct BlockChunkIndexComponent : ISharedComponentData
{
    public int Index;
}
