using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class PathNode
{
    public Vector3 pos;
    public int value;
    public int startValue;
    public int endValue;
    public PathNode origin;

    public PathNode(Vector3 nodePos, Vector3 startPos, Vector3 endPos)
    {
        this.pos = nodePos;
        this.value = 0;
        this.origin = null;
        this.startValue = Dist(nodePos, startPos);
        this.endValue = Dist(nodePos, endPos);
        this.value = startValue + endValue;
    }

    int Dist(Vector3 pos, Vector3 endPos)
    {
        return Mathf.FloorToInt(math.sqrt(math.pow(endPos.x - pos.x, 2)/* + math.pow((endPos.y - pos.y), 2)*/ + math.pow((endPos.z - pos.z), 2)) * 10);
    }
}