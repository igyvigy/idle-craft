
using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct BlockRespawnComponent : IComponentData
{
    public float? Respawn;
    public float MaxRespawn;
}