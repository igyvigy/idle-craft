
using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct BlockHealthComponent : IComponentData
{
    public int Health;
    public int MaxHealth;
}