using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Collections;
using Unity.Mathematics;

[Serializable]
public class Chunk : MonoBehaviour
{
    public const int Width = 16;
    public const int Height = 100;
    public static int BlocksCount
    {
        get
        {
            return Width * Width * Height;
        }
    }
    public int3[] blocks = new int3[BlocksCount];


    // int4 
    // x, y, z - pos
    // w - direction
    //     0 - east
    //     1 - north
    //     2 - west
    //     3 - south
    public int4 chunkPos;
    private List<int> customBlockIndices = new List<int>();
    private Dictionary<int, int3> customBlockOrigins = new Dictionary<int, int3>();
    private Dictionary<int, BlockRespawn> customBlockRespawns = new Dictionary<int, BlockRespawn>();
    void Awake()
    {

    }
    void OnDestroy()
    {

    }

    void LateUpdate()
    {
        UpdateCustomBlocksIfNeeded();
    }

    public void BuildMesh()
    {
        chunkPos = Utils.ChunkPosbyPosition(transform.position);
        Mesh mesh = new Mesh();
        NativeList<Vector3> verts = new NativeList<Vector3>(Allocator.Temp);
        NativeList<int> tris = new NativeList<int>(Allocator.Temp);
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < BlocksCount; i++)
        {
            if (blocks[i].x != 0) // check if air type
            {

                int3 coord = Utils.to3DBlocks(i);
                float3 blockPos = new float3(coord.x - 1, coord.y, coord.z - 1);

                int blockGrade = Math.Min(Mathf.FloorToInt(blocks[i].y / 10), 2);

                int numFaces = 0;
                if (GetTopBlock(coord, chunkPos).x == 0)
                {
                    verts.Add(blockPos + new float3(0, 1, 0));
                    verts.Add(blockPos + new float3(0, 1, 1));
                    verts.Add(blockPos + new float3(1, 1, 1));
                    verts.Add(blockPos + new float3(1, 1, 0));
                    numFaces++;

                    uvs.AddRange(BlockTexture.Get((BlockType)blocks[i].x, blockGrade).topPos.GetUVs());
                }

                //bottom
                if (GetBottomBlock(coord, chunkPos).x == 0)
                {
                    verts.Add(blockPos + new float3(0, 0, 0));
                    verts.Add(blockPos + new float3(1, 0, 0));
                    verts.Add(blockPos + new float3(1, 0, 1));
                    verts.Add(blockPos + new float3(0, 0, 1));
                    numFaces++;

                    uvs.AddRange(BlockTexture.Get((BlockType)blocks[i].x, blockGrade).bottomPos.GetUVs());
                }

                //front
                if (GetFrontBlock(coord, chunkPos).x == 0)
                {
                    verts.Add(blockPos + new float3(0, 0, 0));
                    verts.Add(blockPos + new float3(0, 1, 0));
                    verts.Add(blockPos + new float3(1, 1, 0));
                    verts.Add(blockPos + new float3(1, 0, 0));
                    numFaces++;

                    uvs.AddRange(BlockTexture.Get((BlockType)blocks[i].x, blockGrade).sidePos.GetUVs());
                }

                //right
                if (GetRightBlock(coord, chunkPos).x == 0)
                {
                    verts.Add(blockPos + new float3(1, 0, 0));
                    verts.Add(blockPos + new float3(1, 1, 0));
                    verts.Add(blockPos + new float3(1, 1, 1));
                    verts.Add(blockPos + new float3(1, 0, 1));
                    numFaces++;

                    uvs.AddRange(BlockTexture.Get((BlockType)blocks[i].x, blockGrade).sidePos.GetUVs());
                }

                //back
                if (GetBackBlock(coord, chunkPos).x == 0)
                {
                    verts.Add(blockPos + new float3(1, 0, 1));
                    verts.Add(blockPos + new float3(1, 1, 1));
                    verts.Add(blockPos + new float3(0, 1, 1));
                    verts.Add(blockPos + new float3(0, 0, 1));
                    numFaces++;

                    uvs.AddRange(BlockTexture.Get((BlockType)blocks[i].x, blockGrade).sidePos.GetUVs());
                }

                //left
                if (GetLeftBlock(coord, chunkPos).x == 0)
                {
                    verts.Add(blockPos + new float3(0, 0, 1));
                    verts.Add(blockPos + new float3(0, 1, 1));
                    verts.Add(blockPos + new float3(0, 1, 0));
                    verts.Add(blockPos + new float3(0, 0, 0));
                    numFaces++;

                    uvs.AddRange(BlockTexture.Get((BlockType)blocks[i].x, blockGrade).sidePos.GetUVs());
                }

                int tl = verts.Length - 4 * numFaces;
                for (int idx = 0; idx < numFaces; idx++)
                {
                    NativeArray<int> range = new NativeArray<int>(6, Allocator.Temp);
                    range[0] = tl + idx * 4;
                    range[1] = tl + idx * 4 + 1;
                    range[2] = tl + idx * 4 + 2;
                    range[3] = tl + idx * 4;
                    range[4] = tl + idx * 4 + 2;
                    range[5] = tl + idx * 4 + 3;
                    tris.AddRange(range);
                    range.Dispose();
                }
            }
        }

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
        verts.Dispose();
        tris.Dispose();
    }
    public int3 GetBlockAtInt3(int3 id)
    {
        return blocks[Utils.to1D(id)];
    }
    public int3 GetBlockAtIndex3D(Index3D id)
    {
        int3 coords = new int3(id.x, id.y, id.z);
        return blocks[Utils.to1D(coords)];
    }

    public bool CanDestroyBlockAt(int id)
    {
        BlockType type = (BlockType)blocks[id].x;
        switch (type)
        {
            case BlockType.BedRock: return false;
            default: return true;
        }
    }

    public Block DestroyBlockAt(int i)
    {
        int3 was = blocks[i];
        int3 become = new int3(0, was.y, 0);
        blocks[i] = become;
        BuildMesh();
        int3 coord = Utils.to3DBlocks(i);
        Chunk[] neighbourChunks = CheckNeighbourChunks(chunkPos, coord.x, coord.y, coord.z);
        if (neighbourChunks.Length > 0)
        {
            foreach (Chunk chunk in neighbourChunks)
            {
                chunk.BuildMesh();
            }
        }
        customBlockIndices.Add(i);
        customBlockOrigins.Add(i, was);
        return new Block(was);
    }
    private void UpdateCustomBlocksIfNeeded()
    {
        if (customBlockIndices.Count > 0)
        {
            var expiredIndices = new List<int>();
            var isDestroying = false;
            foreach (var customBlockIndex in customBlockIndices)
            {
                if (!customBlockRespawns.ContainsKey(customBlockIndex))
                {

                    var block = customBlockOrigins[customBlockIndex];
                    var become = blocks[customBlockIndex];
                    if (block.x == 0) isDestroying = true;
                    var time = isDestroying ? Block.GetRespawn(become) : Block.GetRespawn(block);
                    var coord = Utils.to3DBlocks(customBlockIndex);
                    var worldBlockPos = Utils.WorldBlockPosition(coord, chunkPos);
                    var center = Utils.CenterOfBlockWithWorldPos(worldBlockPos);
                    DateTime deadline = DateTime.Now.AddSeconds(time);
                    var br = BlockRespawn.Create(center, deadline);
                    customBlockRespawns.Add(customBlockIndex, br);
                }
                var resp = customBlockRespawns[customBlockIndex];
                if (resp.SecondsTillDeadline <= 0)
                {
                    expiredIndices.Add(customBlockIndex);
                }
            }
            if (expiredIndices.Count > 0)
            {
                foreach (var i in expiredIndices)
                {
                    var resp = customBlockRespawns[i];
                    var was = customBlockOrigins[i];
                    customBlockRespawns.Remove(i);
                    customBlockOrigins.Remove(i);
                    customBlockIndices.Remove(i);
                    blocks[i] = was;
                    Destroy(resp.gameObject);
                    BuildMesh();
                    if (isDestroying)
                    {
                        int3 coord = Utils.to3DBlocks(i);
                        Chunk[] neighbourChunks = CheckNeighbourChunks(chunkPos, coord.x, coord.y, coord.z);
                        if (neighbourChunks.Length > 0)
                        {
                            foreach (Chunk chunk in neighbourChunks)
                            {
                                chunk.BuildMesh();
                            }
                        }
                    }
                }
            }
        }
    }

    public bool BuildBlock(Block block, int index)
    {
        var was = blocks[index];
        blocks[index] = block.data;
        BuildMesh();
        customBlockIndices.Add(index);
        customBlockOrigins.Add(index, was);
        return true;
    }

    Chunk[] CheckNeighbourChunks(int4 cp, int idx, int idy, int idz)
    {
        var list = new List<Chunk>();
        if (idx == 0)
        {
            int4 neighbourCp = new int4(cp.x - 1, 0, cp.z, 0);
            if (WorldSettings.Chunks.ContainsKey(neighbourCp))
            {
                list.Add(WorldSettings.Chunks[neighbourCp]);
            }
        }
        if (idx == Chunk.Width - 1)
        {
            int4 neighbourCp = new int4(cp.x + 1, 0, cp.z, 0);
            if (WorldSettings.Chunks.ContainsKey(neighbourCp))
            {
                list.Add(WorldSettings.Chunks[neighbourCp]);
            }
        }
        if (idz == 0)
        {
            int4 neighbourCp = new int4(cp.x, 0, cp.z - 1, 0);
            if (WorldSettings.Chunks.ContainsKey(neighbourCp))
            {
                list.Add(WorldSettings.Chunks[neighbourCp]);
            }
        }
        if (idz == Chunk.Width - 1)
        {
            int4 neighbourCp = new int4(cp.x, 0, cp.z + 1, 0);
            if (WorldSettings.Chunks.ContainsKey(neighbourCp))
            {
                list.Add(WorldSettings.Chunks[neighbourCp]);
            }
        }
        return list.ToArray();
    }
    int3 GetLeftBlock(int3 coord, int4 chunkPos)
    {
        int3 newCoord = new int3(coord.x - 1, coord.y, coord.z);
        int newBlockIndex = Utils.to1D(newCoord);
        if (coord.x == 0)
        {
            int4 newChunkPos = new int4(chunkPos.x - 1, chunkPos.y, chunkPos.z, chunkPos.w);
            int3 newChunkCoord = new int3(Width - 1, coord.y, coord.z);
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
            return blocks[newBlockIndex];
        }
    }
    int3 GetRightBlock(int3 coord, int4 chunkPos)
    {
        int3 newCoord = new int3(coord.x + 1, coord.y, coord.z);
        int newBlockIndex = Utils.to1D(newCoord);
        if (coord.x == Width - 1)
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
            return blocks[newBlockIndex];
        }
    }
    int3 GetFrontBlock(int3 coord, int4 chunkPos)
    {
        int3 newCoord = new int3(coord.x, coord.y, coord.z - 1);
        int newBlockIndex = Utils.to1D(newCoord);
        if (coord.z == 0)
        {
            int4 newChunkPos = new int4(chunkPos.x, chunkPos.y, chunkPos.z - 1, chunkPos.w);
            int3 newChunkCoord = new int3(coord.x, coord.y, Width - 1);
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
            return blocks[newBlockIndex];
        }
    }
    int3 GetBackBlock(int3 coord, int4 chunkPos)
    {
        int3 newCoord = new int3(coord.x, coord.y, coord.z + 1);
        int newBlockIndex = Utils.to1D(newCoord);
        if (coord.z == Width - 1)
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
            return blocks[newBlockIndex];
        }
    }
    int3 GetTopBlock(int3 coord, int4 chunkPos)
    {
        int3 newCoord = new int3(coord.x, coord.y + 1, coord.z);
        int newBlockIndex = Utils.to1D(newCoord);
        if (coord.y == Height - 1)
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
            return blocks[newBlockIndex];
        }
    }
    int3 GetBottomBlock(int3 coord, int4 chunkPos)
    {
        int3 newCoord = new int3(coord.x, coord.y - 1, coord.z);
        int newBlockIndex = Utils.to1D(newCoord);
        if (coord.y == 0)
        {
            int4 newChunkPos = new int4(chunkPos.x, chunkPos.y - 1, chunkPos.z, chunkPos.w);
            int3 newChunkCoord = new int3(coord.x, Height - 1, coord.z);
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
            return blocks[newBlockIndex];
        }
    }
}
