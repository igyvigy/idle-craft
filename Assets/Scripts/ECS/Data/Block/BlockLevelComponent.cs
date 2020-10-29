using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct BlockLevelComponent : IComponentData
{
    public int Value;
}
