using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct BlockWorldPosComponent : IComponentData
{
    public int3 WoldPos;
}
