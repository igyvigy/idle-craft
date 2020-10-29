using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct BlockPosComponent : IComponentData
{
    public int3 Pos;
}
