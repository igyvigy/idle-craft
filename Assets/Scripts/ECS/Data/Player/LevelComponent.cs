using System;
using Unity.Entities;

[Serializable]
public struct LevelComponent : IComponentData
{
    public float Value;
}
