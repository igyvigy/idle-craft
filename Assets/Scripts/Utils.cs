using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.EventSystems;

public class Utils
{
    public static int4 ChunkPosbyPosition(Vector3 position)
    {

        int chunkPosX = Mathf.FloorToInt((position.x + 1) / Chunk.Width);
        int chunkPosZ = Mathf.FloorToInt((position.z + 1) / Chunk.Width);
        return new int4(chunkPosX, 0, chunkPosZ, 0);
    }

    public static int3 CoordByPosition(Vector3 position)
    {
        int4 cp = ChunkPosbyPosition(position);
        int bix = Mathf.FloorToInt(position.x) - (cp.x * Chunk.Width) + 1;

        int biy = Mathf.FloorToInt(position.y);
        int biz = Mathf.FloorToInt(position.z) - (cp.z * Chunk.Width) + 1;

        return new int3(bix, biy, biz);
    }

    public static int3 CoordByPositionOnChunk(Vector3 position, int4 cp)
    {
        int bix = Mathf.FloorToInt(position.x) - (cp.x * Chunk.Width) + 1;
        int biy = Mathf.FloorToInt(position.y);
        int biz = Mathf.FloorToInt(position.z) - (cp.z * Chunk.Width) + 1;

        return new int3(bix, biy, biz);
    }

    public static (int3, int3, int3) ToTuple(int3x3 data)
    {
        return (data.c0, data.c1, data.c2);
    }

    public static int3x3 FromTuple(int3 i0, int3 i1, int3 i2)
    {
        return new int3x3(
            i0.x, i1.x, i2.x,
            i0.y, i1.y, i2.y,
            i0.z, i1.z, i2.z
            );
    }

    public static Vector3 CenterOfBlockWithWorldPos(int3 bp)
    {
        return new Vector3(bp.x + 0.5f, bp.y + 0.5f, bp.z + 0.5f);
    }
    public static int3 WorldBlockPosForCenter(Vector3 center)
    {
        return new int3(Mathf.RoundToInt(center.x - 0.5f), Mathf.RoundToInt(center.y - 0.5f), Mathf.RoundToInt(center.z - 0.5f));
    }
    public static Vector3 PositionForIndex3DOnChunk(Index3D id, ChunkPos cp)
    {
        return new Vector3(cp.x + id.x - 0.5f, id.y + 0.5f, cp.z + id.z - 0.5f);
    }
    public static Vector3 Int3ToVector(int3 id)
    {
        return new Vector3(id.x, id.y, id.z);
    }
    public static int3 WorldBlockPosition(int3 coord, int4 chunkPos)
    {
        return new int3((chunkPos.x * Chunk.Width) + coord.x - 1, (chunkPos.y * Chunk.Height) + coord.y, (chunkPos.z * Chunk.Width) + coord.z - 1);
    }
    public static (int4, int3) ChunkPosAndCoordForPosition(Vector3 position)
    {
        int4 cp = ChunkPosbyPosition(position);
        int bix = Mathf.FloorToInt(position.x) - (cp.x * Chunk.Width) + 1;
        int biy = Mathf.FloorToInt(position.y);
        int biz = Mathf.FloorToInt(position.z) - (cp.z * Chunk.Width) + 1;

        return (cp, new int3(bix, biy, biz));
    }
    public static (int4, int3) ChunkPosAndCoordForWorldBlockPos(int3 wbp)
    {
        int chunkPosX = Mathf.FloorToInt((wbp.x + 1) / Mathf.Floor(Chunk.Width));
        int chunkPosZ = Mathf.FloorToInt((wbp.z + 1) / Mathf.Floor(Chunk.Width));
        int bix = Mathf.FloorToInt((wbp.x) - (chunkPosX * Mathf.Floor(Chunk.Width)) + 1);
        int biy = Mathf.FloorToInt(wbp.y);
        int biz = Mathf.FloorToInt((wbp.z) - (chunkPosZ * Mathf.Floor(Chunk.Width)) + 1);
        return (new int4(chunkPosX, 0, chunkPosZ, 0), new int3(bix, biy, biz));
    }
    public static int3 WorldChunkPosition(int4 chunkPos)
    {
        return new int3(chunkPos.x * Chunk.Width - 1, chunkPos.y * Chunk.Height, chunkPos.z * Chunk.Width - 1);
    }

    public static Vector3 CentrifyPosition(Vector3 pos)
    {
        int chunkPosX = Mathf.FloorToInt((pos.x + 1) / Mathf.Floor(Chunk.Width));
        int chunkPosZ = Mathf.FloorToInt((pos.z + 1) / Mathf.Floor(Chunk.Width));
        int bix = Mathf.FloorToInt((pos.x) - (chunkPosX * Mathf.Floor(Chunk.Width)) + 1);
        int biy = Mathf.FloorToInt(pos.y);
        int biz = Mathf.FloorToInt((pos.z) - (chunkPosZ * Mathf.Floor(Chunk.Width)) + 1);
        var wbp = new int3((chunkPosX * Chunk.Width) + bix - 1, biy, (chunkPosZ * Chunk.Width) + biz - 1);
        return CenterOfBlockWithWorldPos(wbp);
    }

    public static int to1D(int3 coords)
    {
        return (coords.y * Chunk.Width * Chunk.Width) + (coords.z * Chunk.Width) + coords.x;
    }
    public static int to1D(int x, int y, int z)
    {
        return (y * Chunk.Width * Chunk.Width) + (z * Chunk.Width) + x;
    }
    public static int3 to3DBlocks(int idx)
    {
        int y = idx / (Chunk.Width * Chunk.Width);
        idx -= (y * Chunk.Width * Chunk.Width);
        int z = idx / Chunk.Width;
        int x = idx % Chunk.Width;
        return new int3(x, y, z);
    }
    public static int3 GetLeftBlock(int3 coord, int4 chunkPos)
    {
        int3 newCoord = new int3(coord.x - 1, coord.y, coord.z);
        int newBlockIndex = Utils.to1D(newCoord);
        if (coord.x == 0)
        {
            int4 newChunkPos = new int4(chunkPos.x - 1, chunkPos.y, chunkPos.z, chunkPos.w);
            int3 newChunkCoord = new int3(Chunk.Width - 1, coord.y, coord.z);
            int newChunkBlockIndex = Utils.to1D(newChunkCoord);
            if (WorldSettings.Chunks.ContainsKey(newChunkPos))
            {
                return WorldSettings.Chunks[newChunkPos].blocks[newChunkBlockIndex];
            }
            else
            {
                return WorldSettings.GetBlock(Utils.WorldBlockPosition(newChunkCoord, newChunkPos));
            }
        }
        else
        {
            int3[] blocks = WorldSettings.Chunks[chunkPos].blocks;
            return blocks[newBlockIndex];
        }
    }

    public static int3 GetRightBlock(int3 coord, int4 chunkPos)
    {
        int3 newCoord = new int3(coord.x + 1, coord.y, coord.z);
        int newBlockIndex = Utils.to1D(newCoord);
        if (coord.x == Chunk.Width - 1)
        {
            int4 newChunkPos = new int4(chunkPos.x + 1, chunkPos.y, chunkPos.z, chunkPos.w);
            int3 newChunkCoord = new int3(0, coord.y, coord.z);
            int newChunkBlockIndex = Utils.to1D(newChunkCoord);
            if (WorldSettings.Chunks.ContainsKey(newChunkPos))
            {
                return WorldSettings.Chunks[newChunkPos].blocks[newChunkBlockIndex];
            }
            else
            {
                return WorldSettings.GetBlock(Utils.WorldBlockPosition(newChunkCoord, newChunkPos));
            }
        }
        else
        {
            int3[] blocks = WorldSettings.Chunks[chunkPos].blocks;
            return blocks[newBlockIndex];
        }
    }
    public static int3 GetFrontBlock(int3 coord, int4 chunkPos)
    {
        int3 newCoord = new int3(coord.x, coord.y, coord.z - 1);
        int newBlockIndex = Utils.to1D(newCoord);
        if (coord.z == 0)
        {
            int4 newChunkPos = new int4(chunkPos.x, chunkPos.y, chunkPos.z - 1, chunkPos.w);
            int3 newChunkCoord = new int3(coord.x, coord.y, Chunk.Width - 1);
            int newChunkBlockIndex = Utils.to1D(newChunkCoord);
            if (WorldSettings.Chunks.ContainsKey(newChunkPos))
            {
                return WorldSettings.Chunks[newChunkPos].blocks[newChunkBlockIndex];
            }
            else
            {
                return WorldSettings.GetBlock(Utils.WorldBlockPosition(newChunkCoord, newChunkPos));
            }
        }
        else
        {
            int3[] blocks = WorldSettings.Chunks[chunkPos].blocks;
            return blocks[newBlockIndex];
        }
    }
    public static int3 GetBlock(int3 worldPos, int3 offset)
    {
        var center = Utils.CenterOfBlockWithWorldPos(worldPos);
        var coord = Utils.CoordByPosition(center);
        var cp = Utils.ChunkPosbyPosition(center);
        return GetBlock(coord, cp, offset);
    }
    public static int3 GetBlock(int3 coord, int4 chunkPos, int3 offset)
    {
        if (offset.Equals(new int3(-1, 0, 1)))
        {

        }
        int3 newCoord = new int3(coord.x + offset.x, coord.y + offset.y, coord.z + offset.z);
        int newBlockIndex = Utils.to1D(newCoord);
        if (coord.z == 0 && offset.z == -1)
        {
            int4 newChunkPos = new int4(chunkPos.x, chunkPos.y, chunkPos.z - 1, chunkPos.w);
            int3 newChunkCoord = new int3(newCoord.x, newCoord.y, Chunk.Width - 1);
            int newChunkBlockIndex = Utils.to1D(newChunkCoord);
            if (WorldSettings.Chunks.ContainsKey(newChunkPos))
            {
                return WorldSettings.Chunks[newChunkPos].blocks[newChunkBlockIndex];
            }
            else
            {
                return WorldSettings.GetBlock(Utils.WorldBlockPosition(newChunkCoord, newChunkPos));
            }
        }
        if (coord.z == Chunk.Width - 1 && offset.z == 1)
        {
            int4 newChunkPos = new int4(chunkPos.x, chunkPos.y, chunkPos.z + 1, chunkPos.w);
            int3 newChunkCoord = new int3(newCoord.x, newCoord.y, 0);
            int newChunkBlockIndex = Utils.to1D(newChunkCoord);
            if (WorldSettings.Chunks.ContainsKey(newChunkPos))
            {
                return WorldSettings.Chunks[newChunkPos].blocks[newChunkBlockIndex];
            }
            else
            {

                return WorldSettings.GetBlock(Utils.WorldBlockPosition(newChunkCoord, newChunkPos));
            }
        }
        if (coord.x == 0 && offset.x == -1)
        {
            int4 newChunkPos = new int4(chunkPos.x - 1, chunkPos.y, chunkPos.z, chunkPos.w);
            int3 newChunkCoord = new int3(Chunk.Width - 1, newCoord.y, newCoord.z);
            int newChunkBlockIndex = Utils.to1D(newChunkCoord);
            if (WorldSettings.Chunks.ContainsKey(newChunkPos))
            {
                return WorldSettings.Chunks[newChunkPos].blocks[newChunkBlockIndex];
            }
            else
            {
                return WorldSettings.GetBlock(Utils.WorldBlockPosition(newChunkCoord, newChunkPos));
            }
        }
        if (coord.x == Chunk.Width - 1 && offset.x == 1)
        {
            int4 newChunkPos = new int4(chunkPos.x + 1, chunkPos.y, chunkPos.z, chunkPos.w);
            int3 newChunkCoord = new int3(0, newCoord.y, newCoord.z);
            int newChunkBlockIndex = Utils.to1D(newChunkCoord);
            if (WorldSettings.Chunks.ContainsKey(newChunkPos))
            {
                return WorldSettings.Chunks[newChunkPos].blocks[newChunkBlockIndex];
            }
            else
            {
                return WorldSettings.GetBlock(Utils.WorldBlockPosition(newChunkCoord, newChunkPos));
            }
        }
        if (coord.y == 0 && offset.y == -1)
        {
            int4 newChunkPos = new int4(chunkPos.x, chunkPos.y - 1, chunkPos.z, chunkPos.w);
            int3 newChunkCoord = new int3(newCoord.x, Chunk.Height - 1, newCoord.z);
            int newChunkBlockIndex = Utils.to1D(newChunkCoord);
            if (WorldSettings.Chunks.ContainsKey(newChunkPos))
            {
                return WorldSettings.Chunks[newChunkPos].blocks[newChunkBlockIndex];
            }
            else
            {
                return WorldSettings.GetBlock(Utils.WorldBlockPosition(newChunkCoord, newChunkPos));
            }
        }
        if (coord.y == Chunk.Height - 1 && offset.y == 1)
        {
            int4 newChunkPos = new int4(chunkPos.x, chunkPos.y + 1, chunkPos.z, chunkPos.w);
            int3 newChunkCoord = new int3(newCoord.x, 0, newCoord.z);
            int newChunkBlockIndex = Utils.to1D(newChunkCoord);
            if (WorldSettings.Chunks.ContainsKey(newChunkPos))
            {
                return WorldSettings.Chunks[newChunkPos].blocks[newChunkBlockIndex];
            }
            else
            {
                return WorldSettings.GetBlock(Utils.WorldBlockPosition(newChunkCoord, newChunkPos));
            }
        }
        int3[] blocks = WorldSettings.Chunks[chunkPos].blocks;
        return blocks[newBlockIndex];
    }
    public static int3 GetBackBlock(int3 coord, int4 chunkPos)
    {
        int3 newCoord = new int3(coord.x, coord.y, coord.z + 1);
        int newBlockIndex = Utils.to1D(newCoord);
        if (coord.z == Chunk.Width - 1)
        {
            int4 newChunkPos = new int4(chunkPos.x, chunkPos.y, chunkPos.z + 1, chunkPos.w);
            int3 newChunkCoord = new int3(coord.x, coord.y, 0);
            int newChunkBlockIndex = Utils.to1D(newChunkCoord);
            if (WorldSettings.Chunks.ContainsKey(newChunkPos))
            {
                return WorldSettings.Chunks[newChunkPos].blocks[newChunkBlockIndex];
            }
            else
            {

                return WorldSettings.GetBlock(Utils.WorldBlockPosition(newChunkCoord, newChunkPos));
            }
        }
        else
        {
            int3[] blocks = WorldSettings.Chunks[chunkPos].blocks;
            return blocks[newBlockIndex];
        }
    }
    public static int3 GetTopBlock(int3 coord, int4 chunkPos)
    {
        int3 newCoord = new int3(coord.x, coord.y + 1, coord.z);
        int newBlockIndex = Utils.to1D(newCoord);
        if (coord.y == Chunk.Height - 1)
        {
            int4 newChunkPos = new int4(chunkPos.x, chunkPos.y + 1, chunkPos.z, chunkPos.w);
            int3 newChunkCoord = new int3(coord.x, 0, coord.z);
            int newChunkBlockIndex = Utils.to1D(newChunkCoord);
            if (WorldSettings.Chunks.ContainsKey(newChunkPos))
            {
                return WorldSettings.Chunks[newChunkPos].blocks[newChunkBlockIndex];
            }
            else
            {
                return WorldSettings.GetBlock(Utils.WorldBlockPosition(newChunkCoord, newChunkPos));
            }
        }
        else
        {
            int3[] blocks = WorldSettings.Chunks[chunkPos].blocks;
            return blocks[newBlockIndex];
        }
    }
    public static int3 GetBottomBlock(int3 coord, int4 chunkPos)
    {
        int3 newCoord = new int3(coord.x, coord.y - 1, coord.z);
        int newBlockIndex = Utils.to1D(newCoord);
        if (coord.y == 0)
        {
            int4 newChunkPos = new int4(chunkPos.x, chunkPos.y - 1, chunkPos.z, chunkPos.w);
            int3 newChunkCoord = new int3(coord.x, Chunk.Height - 1, coord.z);
            int newChunkBlockIndex = Utils.to1D(newChunkCoord);
            if (WorldSettings.Chunks.ContainsKey(newChunkPos))
            {
                return WorldSettings.Chunks[newChunkPos].blocks[newChunkBlockIndex];
            }
            else
            {
                return WorldSettings.GetBlock(Utils.WorldBlockPosition(newChunkCoord, newChunkPos));
            }
        }
        else
        {
            int3[] blocks = WorldSettings.Chunks[chunkPos].blocks;
            return blocks[newBlockIndex];
        }
    }

    public static bool IsPointerOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        else
        {
            bool result = false;
            PointerEventData pe = new PointerEventData(EventSystem.current);
            pe.position = Input.mousePosition;
            List<RaycastResult> hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pe, hits);
            if (hits.Count > 0)
            {
                foreach (var hit in hits)
                {
                    Debug.Log(hit.gameObject.name);
                }
                result = true;
            }
            return result;
        }
    }

    public static bool IsTargetReachable(Vector3 from, Vector3 target)
    {
        var cp = Utils.ChunkPosbyPosition(from);
        var bottomPlayerPos = Utils.WorldBlockPosition(Utils.CoordByPositionOnChunk(from, cp), cp);
        var topPlayerPos = new int3(bottomPlayerPos.x, bottomPlayerPos.y + 1, bottomPlayerPos.z);
        var targetCp = Utils.ChunkPosbyPosition(target);
        var taretPos = Utils.WorldBlockPosition(Utils.CoordByPositionOnChunk(target, targetCp), targetCp);

        bool result = false;
        int3[] GetMods()
        {
            return new int3[] {
                new int3(bottomPlayerPos.x + 1, bottomPlayerPos.y, bottomPlayerPos.z + 1),
                new int3(bottomPlayerPos.x + 1, bottomPlayerPos.y, bottomPlayerPos.z - 1),
                new int3(bottomPlayerPos.x + 1, bottomPlayerPos.y, bottomPlayerPos.z),
                new int3(bottomPlayerPos.x - 1, bottomPlayerPos.y, bottomPlayerPos.z + 1),
                new int3(bottomPlayerPos.x - 1, bottomPlayerPos.y, bottomPlayerPos.z - 1),
                new int3(bottomPlayerPos.x - 1, bottomPlayerPos.y, bottomPlayerPos.z),
                new int3(bottomPlayerPos.x, bottomPlayerPos.y, bottomPlayerPos.z + 1),
                new int3(bottomPlayerPos.x, bottomPlayerPos.y, bottomPlayerPos.z - 1),
                new int3(bottomPlayerPos.x, bottomPlayerPos.y, bottomPlayerPos.z),

                new int3(bottomPlayerPos.x + 1, bottomPlayerPos.y - 1, bottomPlayerPos.z + 1),
                new int3(bottomPlayerPos.x + 1, bottomPlayerPos.y - 1, bottomPlayerPos.z - 1),
                new int3(bottomPlayerPos.x + 1, bottomPlayerPos.y - 1, bottomPlayerPos.z),
                new int3(bottomPlayerPos.x - 1, bottomPlayerPos.y - 1, bottomPlayerPos.z + 1),
                new int3(bottomPlayerPos.x - 1, bottomPlayerPos.y - 1, bottomPlayerPos.z - 1),
                new int3(bottomPlayerPos.x - 1, bottomPlayerPos.y - 1, bottomPlayerPos.z),
                new int3(bottomPlayerPos.x, bottomPlayerPos.y - 1, bottomPlayerPos.z + 1),
                new int3(bottomPlayerPos.x, bottomPlayerPos.y - 1, bottomPlayerPos.z - 1),
                new int3(bottomPlayerPos.x, bottomPlayerPos.y - 1, bottomPlayerPos.z),

                new int3(topPlayerPos.x + 1, topPlayerPos.y, topPlayerPos.z + 1),
                new int3(topPlayerPos.x + 1, topPlayerPos.y, topPlayerPos.z - 1),
                new int3(topPlayerPos.x + 1, topPlayerPos.y, topPlayerPos.z),
                new int3(topPlayerPos.x - 1, topPlayerPos.y, topPlayerPos.z + 1),
                new int3(topPlayerPos.x - 1, topPlayerPos.y, topPlayerPos.z - 1),
                new int3(topPlayerPos.x - 1, topPlayerPos.y, topPlayerPos.z),
                new int3(topPlayerPos.x, topPlayerPos.y, topPlayerPos.z + 1),
                new int3(topPlayerPos.x, topPlayerPos.y, topPlayerPos.z - 1),
                new int3(topPlayerPos.x, topPlayerPos.y, topPlayerPos.z),

                new int3(topPlayerPos.x + 1, topPlayerPos.y + 1, topPlayerPos.z + 1),
                new int3(topPlayerPos.x + 1, topPlayerPos.y + 1, topPlayerPos.z - 1),
                new int3(topPlayerPos.x + 1, topPlayerPos.y + 1, topPlayerPos.z),
                new int3(topPlayerPos.x - 1, topPlayerPos.y + 1, topPlayerPos.z + 1),
                new int3(topPlayerPos.x - 1, topPlayerPos.y + 1, topPlayerPos.z - 1),
                new int3(topPlayerPos.x - 1, topPlayerPos.y + 1, topPlayerPos.z),
                new int3(topPlayerPos.x, topPlayerPos.y + 1, topPlayerPos.z + 1),
                new int3(topPlayerPos.x, topPlayerPos.y + 1, topPlayerPos.z - 1),
                new int3(topPlayerPos.x, topPlayerPos.y + 1, topPlayerPos.z),
            };
        }
        foreach (var p in GetMods()) if (taretPos.Equals(p))
            {
                result = true;
                break;
            }

        return result;
    }

}