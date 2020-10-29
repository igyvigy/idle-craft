using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
public delegate void IThirdPersonSelectCallback();
public interface IThirdPersonSelect
{
    void AddOrRemoveBlock(int3 blockWorldpos, int3 block);
    bool StackIncludes(int3 blockWorldpos);
    bool hasBlocksToDestroy { get; }
    int3? nextBlock { get; }
    int3? nextBlockWorldPos { get; }
    int3x3? nextBlockData { get; }
    List<int3x3> blocksToDestroy { get; }
}

[Serializable]
public class BlockDestroying : IThirdPersonSelect
{
    List<int3x3> _blocksToDestroy = new List<int3x3>();
    public List<int3x3> blocksToDestroy => _blocksToDestroy;
    public bool hasBlocksToDestroy
    {
        get { return _blocksToDestroy.Count > 0; }
    }
    public int3? nextBlock
    {
        get
        {
            if (nextBlockData != null) return nextBlockData.Value.c1;
            else return null;
        }
    }
    public int3? nextBlockWorldPos
    {
        get
        {
            if (nextBlockData != null) return nextBlockData.Value.c0;
            else return null;
        }
    }

    // x -> curHp
    // y -> maxHp
    // z -> 0
    public int3? nextBlockHealth
    {
        get
        {
            if (nextBlockData != null) return nextBlockData.Value.c2;
            else return null;
        }
    }
    public int3x3? nextBlockData
    {
        get
        {
            if (_blocksToDestroy.Count > 0)
            {
                return _blocksToDestroy[0];
            }
            else return null;
        }
    }

    public void ClearQueue()
    {
        _blocksToDestroy = new List<int3x3>();
    }

    public void AddOrRemoveBlock(int3 blockWorldpos, int3 block)
    {
        if (StackIncludes(blockWorldpos))
        {
            PopBlock(blockWorldpos);
        }
        else
        {
            PushTopBlock(blockWorldpos, block);
        }
        UpdateDestroyBlockQueueUI();
    }
    public List<BlockUI> blockUIs = new List<BlockUI>();
    public void UpdateDestroyBlockQueueUI()
    {
        for (var i = 0; i < _blocksToDestroy.Count; i++)
        {
            int3 blockWorldPos = _blocksToDestroy[i].c0;
            int3 block = _blocksToDestroy[i].c1;
            string text = (i).ToString();
            if (i == 0) text = "";
            if (i < blockUIs.Count)
            {
                Vector3 pos = Utils.CenterOfBlockWithWorldPos(blockWorldPos);
                blockUIs[i].gameObject.SetActive(true);
                blockUIs[i].Setup(pos, block, text);
            }
            else
            {
                BlockUI bUI = BlockUI.Create(blockWorldPos, block, text);
                blockUIs.Add(bUI);
            }
        }
        if (blockUIs.Count > _blocksToDestroy.Count)
        {
            var remains = blockUIs.GetRange(_blocksToDestroy.Count, blockUIs.Count - _blocksToDestroy.Count);
            foreach (var r in remains)
            {
                r.gameObject.SetActive(false);
            }
        }
    }

    public bool StackIncludes(int3 blockWorldpos)
    {
        bool res = false;
        if (hasBlocksToDestroy)
            foreach (var b in _blocksToDestroy)
                if (b.c0.Equals(blockWorldpos))
                {
                    res = true;
                }
        return res;
    }

    public void PushTopBlock(int3 blockWorldpos, int3 block)
    {
        _blocksToDestroy.Add(Utils.FromTuple(blockWorldpos, block, new int3(0)));
    }
    int3? PopBlock(int3 blockWorldpos)
    {
        int3? res = null;
        int3x3? val = null;
        if (hasBlocksToDestroy)
            foreach (var b in _blocksToDestroy)
                if (b.c0.Equals(blockWorldpos))
                {
                    res = b.c1;
                    val = b;
                }
        if (val != null) _blocksToDestroy.Remove(val.Value);
        return res;
    }

    int3x2? PopTopBlock()
    {
        int3x2? res = null;
        int3x3? val = null;
        if (hasBlocksToDestroy)
        {
            val = _blocksToDestroy[_blocksToDestroy.Count - 1];
            res = new int3x2(
                val.Value.c0.x, val.Value.c1.x, val.Value.c0.y,
                    val.Value.c1.y, val.Value.c0.z, val.Value.c1.z
            );
        }
        if (val != null) _blocksToDestroy.RemoveAt(_blocksToDestroy.Count - 1);
        return res;
    }

    public int3x2? PopBottomBlock()
    {
        int3x2? res = null;
        int3x3? val = null;
        if (hasBlocksToDestroy)
        {
            val = _blocksToDestroy[0];
            res = new int3x2(
                val.Value.c0.x, val.Value.c1.x, val.Value.c0.y,
                    val.Value.c1.y, val.Value.c0.z, val.Value.c1.z
            );
        }
        if (val != null) _blocksToDestroy.RemoveAt(0);
        return res;
    }



}