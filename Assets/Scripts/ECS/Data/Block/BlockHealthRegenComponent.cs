
using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct BlockHealthRegenComponent : IComponentData
{
    public float HealthRegen;
}