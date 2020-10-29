using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct BlockTypeComponent : IComponentData
{
    public int Value;
}
