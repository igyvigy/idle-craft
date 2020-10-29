using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using System;
using Unity.Transforms;
using System.Linq;

class WorldSettings : MonoBehaviour
{
    public Dictionary<int4, Chunk> chunks = new Dictionary<int4, Chunk>();
    public static Dictionary<int4, Chunk> Chunks
    {
        get
        {
            return instance.chunks;
        }
    }
    public List<Chunk> pooledChunks = new List<Chunk>();
    public Dictionary<int4, Wall> walls = new Dictionary<int4, Wall>();
    public static Dictionary<int4, Wall> Walls
    {
        get
        {
            return instance.walls;
        }
    }
    public List<Wall> pooledWalls = new List<Wall>();
    public List<int4> toGenerate = new List<int4>();
    public List<int4> toDestroy = new List<int4>();

    public int4? curChunkPos;
    public const int ChunkDist = 8;
    public const int ChunkLineWidth = 32;

    public const float BlockRespawn = 120;
    FastNoise noise = new FastNoise(1);

    Player player;

    EntityManager manager;

    static WorldSettings instance;
    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }
    void OnDestroy()
    {

    }
    void Start()
    {
        player = TagResolver.i.player;
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        BuildChunksAroundPosition(player.transform.position, true);
    }
    void LateUpdate()
    {
        BuildChunksAroundPosition(player.transform.position);
    }
    void BuildChunksAroundPosition(Vector3 pos, bool instant = false)
    {
        int4 chunkPos = Utils.ChunkPosbyPosition(pos);
        int3 worldPos = Utils.WorldChunkPosition(chunkPos);
        var halfLineWidth = ChunkLineWidth / 2;
        if (curChunkPos == null || curChunkPos.Value.x != chunkPos.x || curChunkPos.Value.z != chunkPos.z)
        {
            curChunkPos = chunkPos;
            var ir = math.pow(ChunkDist - 0.5, 2);
            for (int i = chunkPos.x - ChunkDist; i <= chunkPos.x + ChunkDist; i++)
                for (int j = chunkPos.z - ChunkDist; j <= chunkPos.z + ChunkDist; j++)
                {
                    var ii = math.pow(i - chunkPos.x, 2);
                    var ij = math.pow(j - chunkPos.z, 2);
                    if (ii + ij > ir) continue;
                    int4 cp = new int4(i, 0, j, 0);
                    if ((i < -halfLineWidth || i > halfLineWidth) && (j < -halfLineWidth || j > halfLineWidth))
                    {
                        continue;
                    }

                    if (!chunks.ContainsKey(cp))
                    {
                        if (instant)
                        {
                            MakeBlocksAndBuildChunk(cp);
                        }
                        else
                        {
                            toGenerate.Add(cp);
                        }
                    }
                }

            foreach (KeyValuePair<int4, Chunk> c in chunks)
            {
                int4 cp = c.Key;
                var ii = math.pow(cp.x - chunkPos.x, 2);
                var ij = math.pow(cp.z - chunkPos.z, 2);
                if (ii + ij > ir)
                {
                    toDestroy.Add(c.Key);
                }
            }
            int4[] currentToGenerate = new int4[toGenerate.Count];
            toGenerate.CopyTo(currentToGenerate);
            foreach (int4 cp in currentToGenerate)
            {
                var ii = math.pow(cp.x - chunkPos.x, 2);
                var ij = math.pow(cp.z - chunkPos.z, 2);
                if (ii + ij > ir)
                {
                    toGenerate.Remove(cp);
                }
            }
            foreach (int4 cp in toDestroy)
            {
                chunks[cp].gameObject.SetActive(false);
                pooledChunks.Add(chunks[cp]);
                chunks.Remove(cp);
            }
            toDestroy.Clear();
            StartCoroutine(DelayBuildChunks());
        }
    }

    void MakeBlocksAndBuildChunk(int4 chunkPos)
    {
        NativeArray<int3> blocks = new NativeArray<int3>(Chunk.BlocksCount, Allocator.Temp);

        for (int i = 0; i < Chunk.BlocksCount; i++)
        {
            int3 coord = Utils.to3DBlocks(i);
            int3 block = GetBlock(Utils.WorldBlockPosition(coord, chunkPos));
            blocks[i] = block;
        }

        BuildChunk(chunkPos, blocks.ToArray());
        blocks.Dispose();
    }

    IEnumerator DelayBuildChunks()
    {
        while (toGenerate.Count > 0)
        {
            int4 cp = toGenerate[0];

            var blocks = new NativeArray<int3>(Chunk.BlocksCount, Allocator.TempJob);

            BuildChunkJob job = new BuildChunkJob()
            {
                chunkX = cp.x,
                chunkZ = cp.z,
                count = Chunk.BlocksCount,
                width = Chunk.Width,
                blocks = blocks
            };

            JobHandle handle = job.Schedule();

            yield return new WaitForSeconds(0.4f);
            handle.Complete();
            BuildChunk(cp, blocks.ToArray());
            blocks.Dispose();
            if (toGenerate.Count > 0) toGenerate.RemoveAt(0);
        }
    }

    struct BuildChunkJob : IJob
    {
        [ReadOnly] public int chunkX;
        [ReadOnly] public int chunkZ;
        [ReadOnly] public int count;
        [ReadOnly] public int width;
        public NativeArray<int3> blocks;
        public void Execute()
        {
            for (int i = 0; i < count; i++)
            {
                int3 coord = to3DBlocks(i, width);
                int3 block = GetBlock(WorldBlockPosition(coord, chunkX, chunkZ, width));
                blocks[i] = block;
            }

        }
        int3 WorldBlockPosition(int3 coord, int chunkX, int chunkZ, int width)
        {
            return new int3((chunkX * width) + coord.x - 1, coord.y, (chunkZ * width) + coord.z - 1);
        }
        int3 to3DBlocks(int idx, int width)
        {
            int y = idx / (width * width);
            idx -= (y * width * width);
            int z = idx / width;
            int x = idx % width;
            return new int3(x, y, z);
        }
    }

    private void BuildChunk(int4 chunkPos, int3[] blocks)
    {
        if (chunks.ContainsKey(chunkPos)) return;
        Chunk chunk;
        if (pooledChunks.Count > 0)
        {
            chunk = pooledChunks[0];
            chunk.gameObject.SetActive(true);
            pooledChunks.RemoveAt(0);
            chunk.transform.position = new Vector3(chunkPos.x * Chunk.Width, chunkPos.y * Chunk.Height, chunkPos.z * Chunk.Width);
        }
        else
        {
            GameObject chunkGO = Instantiate(GameAssets.i.pfChunk.gameObject, new Vector3(chunkPos.x * Chunk.Width, chunkPos.y * Chunk.Height, chunkPos.z * Chunk.Width), Quaternion.identity);
            chunk = chunkGO.GetComponent<Chunk>();
        }

        GenerateTrees(chunkPos, ref blocks);
        chunk.blocks = blocks;

        WaterChunk wat = chunk.transform.GetComponentInChildren<WaterChunk>();
        wat.SetLocs(chunk.blocks);
        wat.BuildMesh();

        chunk.BuildMesh();
        chunks.Add(chunkPos, chunk);

    }

    public static int3 GetBlock(int3 worldBlockPosition)
    {
        int3 block = instance.GetBlock(worldBlockPosition.x, worldBlockPosition.y, worldBlockPosition.z);

        return block;
    }
    int3 GetBlock(int x, int y, int z)
    {
        int numOfBlocksToRaiseGrade = 18;
        float gradeWider = 1.2f;
        float wider = 1.2f;

        float simplex1 = instance.noise.GetSimplex(x * .8f * wider, z * .8f * wider) * 10;
        float simplex2 = instance.noise.GetSimplex(x * 3f * wider, z * 3f * wider) * 10 * (instance.noise.GetSimplex(x * .3f * wider, z * .3f * wider) + .5f);

        float heightMap = simplex1 + simplex2;

        //add the 2d noise to the middle of the terrain chunk
        float baseLandHeight = Chunk.Height * .5f + heightMap; //-30 + heightMap;//

        //3d noise for caves and overhangs and such
        float caveNoise1 = instance.noise.GetPerlinFractal(x * 5f * wider, y * 10f, z * 5f * wider);
        float caveMask = instance.noise.GetSimplex(x * .3f * wider, z * .3f * wider) + .3f;

        //stone layer heightmap
        float simplexStone1 = instance.noise.GetSimplex(x * 1f * wider, z * 1f * wider) * 10;
        float simplexStone2 = (instance.noise.GetSimplex(x * 5f * wider, z * 5f * wider) + .5f) * 20 * (instance.noise.GetSimplex(x * .3f * wider, z * .3f * wider) + .5f);

        float stoneHeightMap = simplexStone1 + simplexStone2;
        float baseStoneHeight = Chunk.Height * .4f + stoneHeightMap; //-45 + stoneHeightMap;//

        BlockType blockType = BlockType.Air; // Air
        float distanceFromCenter = math.sqrt(math.abs(x * x) + math.abs(z * z));
        int level = Mathf.FloorToInt(distanceFromCenter * gradeWider / numOfBlocksToRaiseGrade);

        if (y <= baseLandHeight)
        {
            level += (int)baseLandHeight - y + 1;
            blockType = BlockType.Dirt;

            //just on the surface, use a grass type
            if (y > baseLandHeight - 1 && y > WaterChunk.waterHeight - 1)
                blockType = BlockType.Grass;

            if (y <= baseStoneHeight)
                blockType = BlockType.Stone;
        }
        else
        {
            level += y - (int)baseLandHeight + 1;
        }

        if (caveNoise1 > math.max(caveMask, .2f))
            blockType = BlockType.Air;

        if (y == 0)
        {
            blockType = BlockType.BedRock;
        }

        var block = new int3((sbyte)blockType, level, 0);

        int endOfWorldBias = (ChunkLineWidth / 2 * Chunk.Width);

        if (x <= -endOfWorldBias - 1 && z <= -endOfWorldBias - 1)
        {
            return new int3(6, 0, 0);
        }
        else if (x <= -endOfWorldBias - 1 && z >= endOfWorldBias + Chunk.Width - 2)
        {
            return new int3(6, 0, 0);
        }
        else if (x >= endOfWorldBias + Chunk.Width - 2 && z >= endOfWorldBias + Chunk.Width - 2)
        {
            return new int3(6, 0, 0);
        }
        else if (x >= endOfWorldBias + Chunk.Width - 2 && z <= -endOfWorldBias - 1)
        {
            return new int3(6, 0, 0);
        }

        return block;
    }

    void GenerateTrees(int4 cp, ref int3[] blocks)
    {
        System.Random rand = new System.Random(cp.x * Chunk.Width * 10000 + cp.z * Chunk.Width);

        float simplex = noise.GetSimplex(cp.x * Chunk.Width * .8f, cp.z * Chunk.Width * .8f);

        if (simplex > 0)
        {
            simplex *= 2f;
            int treeCount = Mathf.FloorToInt((float)rand.NextDouble() * 5 * simplex);

            NativeList<int2> planted = new NativeList<int2>(Allocator.Temp);

            while (treeCount > 0)
            {
                int xPos = (int)(rand.NextDouble() * Chunk.Width - 2) + 1;

                int zPos = (int)(rand.NextDouble() * Chunk.Width - 2) + 1;

                int y = Chunk.Height - 1;
                //find the ground
                while (y > 0 && blocks[Utils.to1D(xPos, y, zPos)].x == 0)
                {
                    y--;
                }
                var found = false;
                var tries = 3;
                while (tries > 0)
                {
                    if (blocks[Utils.to1D(xPos, y, zPos)].x != (sbyte)BlockType.Dirt && blocks[Utils.to1D(xPos, y, zPos)].x != (sbyte)BlockType.Grass)
                    {
                        xPos = (int)(rand.NextDouble() * Chunk.Width - 2) + 1;
                        zPos = (int)(rand.NextDouble() * Chunk.Width - 2) + 1;
                        tries--;
                    }
                    else
                    {
                        found = true;
                        tries = 0;
                    }
                }
                if (!found) break;
                y++;

                int treeHeight = 4 + (int)(rand.NextDouble() * 4);
                //build trunk
                for (int j = 0; j < treeHeight; j++)
                {
                    if (y + j < Chunk.Height)
                        blocks[Utils.to1D(xPos, y + j, zPos)].x = (sbyte)BlockType.Trunk;
                }

                int leavesWidth = 1 + (int)(rand.NextDouble() * 6);
                int leavesHeight = (int)(rand.NextDouble() * 3);
                //leaves
                int iter = 0;
                for (int m = y + treeHeight - 1; m <= y + treeHeight - 1 + treeHeight; m++)
                {
                    for (int k = xPos - (int)(leavesWidth * .5) + iter / 2; k <= xPos + (int)(leavesWidth * .5) - iter / 2; k++)
                        for (int l = zPos - (int)(leavesWidth * .5) + iter / 2; l <= zPos + (int)(leavesWidth * .5) - iter / 2; l++)
                        {
                            if (k >= 0 && k < Chunk.Width && l >= 0 && l < Chunk.Width && m >= 0 && m < Chunk.Height && rand.NextDouble() < .8f)
                                blocks[Utils.to1D(k, m, l)].x = (sbyte)BlockType.Leaves;
                        }

                    iter++;
                }
                planted.Add(new int2(xPos, zPos));
                treeCount--;
            }
            planted.Dispose();
        }
    }
}

public static class WaitFor
{
    public static IEnumerator Frames(int frameCount)
    {
        while (frameCount > 0)
        {
            frameCount--;
            yield return null;
        }
    }
}