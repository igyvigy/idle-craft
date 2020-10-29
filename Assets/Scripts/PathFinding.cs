using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public enum PathFindingMode
{
    Diagonal, Squared
}
[Serializable]
public class PathFinding
{
    public PathFindingMode mode = PathFindingMode.Diagonal;
    public Vector3 startPos, endPos;
    public List<Vector3> visited
    {
        get
        {
            var res = new List<Vector3>();
            for (var i = 0; i < (closed.Count + opened.Count); i++)
            {
                res.Add(i < closed.Count ? closed[i] : opened[i - closed.Count].pos);
            }
            return res;
        }
    }
    public List<PathNode> pathNodes = new List<PathNode>();
    public List<PathNode> opened = new List<PathNode>();
    public List<Vector3> closed = new List<Vector3>();
    public bool isClosed = false;

    public delegate void IPathFindingBuildPathCallback();

    public PathFinding(Vector3 startPos, Vector3 endPos)
    {
        this.startPos = Utils.CentrifyPosition(startPos);
        this.endPos = Utils.CentrifyPosition(endPos);
        closed = new List<Vector3> { this.startPos };
    }
    PathNode TryFindNextPathNode(ref Vector3 curPos)
    {
        var nearPathNodes = GetNearPathNodes(curPos, startPos, endPos, mode);
        var filtered = new List<PathNode>();
        foreach (var n in nearPathNodes) if (!visited.Contains(n.pos)) filtered.Add(n);
        PathNode result = null;
        if (filtered.Count > 0)
        {
            var best = filtered[0];
            result = best;
        }
        return result;
    }

    void OpenNodes(List<PathNode> nodes)
    {
        var toRemove = new List<PathNode>();
        foreach (var n in opened)
            foreach (var node in nodes)
                if (n.pos.Equals(node.pos)) toRemove.Add(n);
        foreach (var pos in toRemove)
            opened.Remove(pos);
        opened.AddRange(nodes);
    }
    int ieCounter = 0;
    public IEnumerator BuildPathToTarget(IPathFindingBuildPathCallback callback, int triesCount = 1000)
    {
        ieCounter = triesCount / 10;
        while (ieCounter > 0)
        {
            var curPos = startPos;
            OpenPossibleNewMoves(curPos);
            for (var t = 0; t < triesCount / 100; t++)
            {
                var nextNode = SuggestNextNode();
                if (nextNode != null)
                {
                    curPos = nextNode.pos;
                    OpenPossibleNewMoves(curPos, nextNode);
                    isClosed = CheckIfEnd(nextNode);
                    if (isClosed)
                    {
                        callback();
                        yield break;
                    }
                }
            }
            yield return 1;
            ieCounter -= 1;
        }
        callback();
    }
    public void ToggleMode()
    {
        mode = mode == PathFindingMode.Diagonal ? PathFindingMode.Squared : PathFindingMode.Diagonal;
    }
    public List<PathNode> OpenPossibleNewMoves(Vector3 pos, PathNode origin = null)
    {
        var nearPathNodes = GetNearPathNodes(pos, startPos, endPos, mode);
        var filtered = new List<PathNode>();
        foreach (var n in nearPathNodes) if (!visited.Contains(n.pos))
            {
                n.origin = origin;
                filtered.Add(n);
            }
        opened.AddRange(filtered);

        return filtered;
    }

    public PathNode SuggestNextNode()
    {
        if (opened.Count == 0) return null;
        SortNodes(ref opened);
        var chosen = opened[0];
        CloseOpenedNode(chosen);
        return chosen;
    }

    public PathNode GetOpenedNodeAtPosition(Vector3 pos)
    {
        return opened.Find(n => n.pos.Equals(pos));
    }
    public void CloseOpenedNode(PathNode node)
    {
        closed.Add(node.pos);
        opened.Remove(node);
    }

    public void Clear()
    {
        pathNodes = new List<PathNode>();
        closed = new List<Vector3>();
        opened = new List<PathNode>();
        startPos = Vector3.zero;
        endPos = Vector3.zero;
        mode = PathFindingMode.Diagonal;
    }
    public void DrawPath()
    {
        if (pathNodes.Count > 0)
        {
            Vector3? prePos = null;
            foreach (var pathNode in pathNodes)
            {
                if (prePos != null) Debug.DrawLine(prePos.Value, pathNode.pos, Color.red);
                prePos = pathNode.pos;
            }
        }
    }

    public static List<PathNode> GetNearPathNodes(Vector3 pos, Vector3 startPos, Vector3 endPos, PathFindingMode mode)
    {
        List<PathNode> _pathNodes = new List<PathNode>();
        var cpCoord = Utils.ChunkPosAndCoordForPosition(pos);
        int4 cp = cpCoord.Item1;
        int3 coord = cpCoord.Item2;

        void Try(int3 offset)
        {
            if (Utils.GetBlock(coord, cp, offset).x == 0) // check ofsetted to be air
            {
                if (Utils.GetBlock(new int3(coord.x, coord.y - 1, coord.z), cp, offset).x != 0) //check block under offsetted to be not air
                {
                    if (Utils.GetBlock(new int3(coord.x, coord.y + 1, coord.z), cp, offset).x == 0) // check block above ofsetted to be air
                    {
                        if (offset.y == -1)
                        { // for blocks which are lower need to make sure that there is extra space above
                            if (Utils.GetBlock(new int3(coord.x, coord.y + 2, coord.z), cp, offset).x != 0) return;
                        }
                        if (offset.y == 1)
                        { // for upper blocks need to make sure that character can jump
                            if (Utils.GetBlock(new int3(coord.x, coord.y + 2, coord.z), cp, int3.zero).x != 0) return;
                        }
                        var v = new Vector3(pos.x + offset.x, pos.y + offset.y, pos.z + offset.z);
                        var PathNode = new PathNode(v, startPos, endPos);
                        _pathNodes.Add(PathNode);
                    }
                }
            }
        }

        Try(new int3(-1, -1, 0));
        Try(new int3(0, -1, -1));
        Try(new int3(0, -1, 1));
        Try(new int3(1, -1, 0));



        Try(new int3(-1, 0, 0));
        Try(new int3(0, 0, -1));
        Try(new int3(0, 0, 1));
        Try(new int3(1, 0, 0));

        Try(new int3(-1, 1, 0));
        Try(new int3(0, 1, -1));
        Try(new int3(0, 1, 1));
        Try(new int3(1, 1, 0));

        if (mode == PathFindingMode.Diagonal)
        {

            Try(new int3(-1, -1, -1));
            Try(new int3(-1, -1, 1));
            Try(new int3(1, -1, -1));
            Try(new int3(1, -1, 1));


            Try(new int3(-1, 0, -1));
            Try(new int3(-1, 0, 1));
            Try(new int3(1, 0, -1));
            Try(new int3(1, 0, 1));

            Try(new int3(-1, 1, -1));
            Try(new int3(-1, 1, 1));
            Try(new int3(1, 1, -1));
            Try(new int3(1, 1, 1));
        }

        SortNodes(ref _pathNodes);
        return _pathNodes;
    }
    public static List<PathNode> ClosestPathBlockTowardsPos(Vector3 blockPos, Vector3 towardsPos)
    {
        var pathNodes = PathFinding.GetNearPathNodes(blockPos, blockPos, Utils.CentrifyPosition(towardsPos), PathFindingMode.Diagonal);
        var lower = PathFinding.GetNearPathNodes(new Vector3(blockPos.x, blockPos.y - 1, blockPos.z), blockPos, Utils.CentrifyPosition(towardsPos), PathFindingMode.Diagonal);
        var lower2 = PathFinding.GetNearPathNodes(new Vector3(blockPos.x, blockPos.y - 2, blockPos.z), blockPos, Utils.CentrifyPosition(towardsPos), PathFindingMode.Diagonal);
        foreach (var n in lower) if (pathNodes.Find(pn => pn.pos.Equals(n.pos)) == null) pathNodes.Add(n);
        foreach (var n in lower2) if (pathNodes.Find(pn => pn.pos.Equals(n.pos)) == null) pathNodes.Add(n);
        SortNodes(ref pathNodes);
        return pathNodes;
    }

    internal bool CheckIfEnd(PathNode n)
    {
        var isEnd = n.pos.Equals(endPos);
        List<PathNode> resultPath = new List<PathNode>();
        if (isEnd)
        {
            closed = new List<Vector3>();
            opened = new List<PathNode>();
            resultPath.Add(new PathNode(endPos, startPos, endPos));
            PathNode parent = n.origin;
            while (parent != null)
            {
                resultPath.Add(parent);
                parent = parent.origin;
            }
            resultPath.Reverse();
            pathNodes = resultPath;
        }
        return isEnd;
    }

    static void SortNodes(ref List<PathNode> nodes)
    {
        nodes.Sort((PathNode a, PathNode b) =>
        {
            if (a != b)
            {
                return a.value - b.value;
            }
            else
            {
                return a.endValue - b.endValue;
            }
        });
    }
}
