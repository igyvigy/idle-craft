using System;
using Unity.Entities;

[Serializable]
public struct HealthComponent : IComponentData
{
    public float Value;
}
