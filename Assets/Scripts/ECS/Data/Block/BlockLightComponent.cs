using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct BlockLightComponent : IComponentData
{
    public float? Value;
}
