using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Collections;
using Unity.Mathematics;

// [ExecuteAlways]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

[Serializable]
public class Origin : MonoBehaviour
{

    public int Width = 16;
    public int Height = 30;

    int to1DBlocks(int3 coords)
    {
        return (coords.y * Width * Width) + (coords.z * Width) + coords.x;
    }
    int3 to3DBlocks(int idx)
    {
        int y = idx / (Width * Width);
        idx -= (y * Width * Width);
        int z = idx / Width;
        int x = idx % Width;
        return new int3(x, y, z);
    }

    void Awake()
    {
        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        for (int y = 0; y > -Height; y--)
            for (int x = 0; x < Width; x++)
                for (int z = 0; z < Width; z++)
                {
     
                    int3 block = GetBlock(x, y, z);
                    if (block.x != 0) // check if air type
                    {

                        float3 blockPos = new float3(x, y, z);
                        int numFaces = 0;
                     
                        verts.Add(blockPos + new float3(0, 1, 0));
                        verts.Add(blockPos + new float3(0, 1, 1));
                        verts.Add(blockPos + new float3(1, 1, 1));
                        verts.Add(blockPos + new float3(1, 1, 0));
                        numFaces++;

                        uvs.AddRange(BlockTexture.Get((BlockType)block.x, 1).topPos.GetUVs());
                     
                        //bottom
                      
                        verts.Add(blockPos + new float3(0, 0, 0));
                        verts.Add(blockPos + new float3(1, 0, 0));
                        verts.Add(blockPos + new float3(1, 0, 1));
                        verts.Add(blockPos + new float3(0, 0, 1));
                        numFaces++;

                        uvs.AddRange(BlockTexture.Get((BlockType)block.x, 1).bottomPos.GetUVs());
                      
                        //front
                    
                        verts.Add(blockPos + new float3(0, 0, 0));
                        verts.Add(blockPos + new float3(0, 1, 0));
                        verts.Add(blockPos + new float3(1, 1, 0));
                        verts.Add(blockPos + new float3(1, 0, 0));
                        numFaces++;

                        uvs.AddRange(BlockTexture.Get((BlockType)block.x, 1).sidePos.GetUVs());
                       
                        //right
                       
                        verts.Add(blockPos + new float3(1, 0, 0));
                        verts.Add(blockPos + new float3(1, 1, 0));
                        verts.Add(blockPos + new float3(1, 1, 1));
                        verts.Add(blockPos + new float3(1, 0, 1));
                        numFaces++;

                        uvs.AddRange(BlockTexture.Get((BlockType)block.x, 1).sidePos.GetUVs());
                       
                        //back
                       
                        verts.Add(blockPos + new float3(1, 0, 1));
                        verts.Add(blockPos + new float3(1, 1, 1));
                        verts.Add(blockPos + new float3(0, 1, 1));
                        verts.Add(blockPos + new float3(0, 0, 1));
                        numFaces++;

                        uvs.AddRange(BlockTexture.Get((BlockType)block.x, 1).sidePos.GetUVs());
                   
                        //left
           
                        verts.Add(blockPos + new float3(0, 0, 1));
                        verts.Add(blockPos + new float3(0, 1, 1));
                        verts.Add(blockPos + new float3(0, 1, 0));
                        verts.Add(blockPos + new float3(0, 0, 0));
                        numFaces++;

                        uvs.AddRange(BlockTexture.Get((BlockType)block.x, 1).sidePos.GetUVs());

                        int tl = verts.Count - 4 * numFaces;
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

    }

    int3 GetBlock(int x, int y, int z)
    {
        var noise = new FastNoise(1);

        float wider = 1.2f;

        float simplex1 = noise.GetSimplex(x * .8f * wider, z * .8f * wider) * 10;
        float simplex2 = noise.GetSimplex(x * 3f * wider, z * 3f * wider) * 10 * (noise.GetSimplex(x * .3f * wider, z * .3f * wider) + .5f);

        float heightMap = simplex1 + simplex2;

        float baseLandHeight = Height * .5f + heightMap; //-30 + heightMap;//

        BlockType blockType = BlockType.Air;
        int level = 0;
        if (y <= baseLandHeight)
        {
            if (y > baseLandHeight - 1)
                blockType = BlockType.BedRock;
        }
        var block = new int3((sbyte)blockType, level, 0);
        return block;
    }

    static public Mesh MakeMesh()
    {
        Vector3 size = new Vector3(1f, 1f, 1f);

        Mesh mesh = new Mesh();

        float length = size.x;//1f;
        float width = size.y;//1f;
        float height = size.z;//1f;

        #region Vertices
        Vector3 p0 = new Vector3(-length * .5f, -width * .5f, height * .5f);
        Vector3 p1 = new Vector3(length * .5f, -width * .5f, height * .5f);
        Vector3 p2 = new Vector3(length * .5f, -width * .5f, -height * .5f);
        Vector3 p3 = new Vector3(-length * .5f, -width * .5f, -height * .5f);

        Vector3 p4 = new Vector3(-length * .5f, width * .5f, height * .5f);
        Vector3 p5 = new Vector3(length * .5f, width * .5f, height * .5f);
        Vector3 p6 = new Vector3(length * .5f, width * .5f, -height * .5f);
        Vector3 p7 = new Vector3(-length * .5f, width * .5f, -height * .5f);

        Vector3[] vertices = new Vector3[]
        {
            // Bottom
            p0, p1, p2, p3,
        
            // Left
            p7, p4, p0, p3,
        
            // Front
            p4, p5, p1, p0,
        
            // Back
            p6, p7, p3, p2,
        
            // Right
            p5, p6, p2, p1,
        
            // Top
            p7, p6, p5, p4
        };
        #endregion

        #region Normales
        Vector3 up = Vector3.up;
        Vector3 down = Vector3.down;
        Vector3 front = Vector3.forward;
        Vector3 back = Vector3.back;
        Vector3 left = Vector3.left;
        Vector3 right = Vector3.right;

        Vector3[] normales = new Vector3[]
        {
            // Bottom
            down, down, down, down,
        
            // Left
            left, left, left, left,
        
            // Front
            front, front, front, front,
        
            // Back
            back, back, back, back,
        
            // Right
            right, right, right, right,
        
            // Top
            up, up, up, up
        };
        #endregion

        #region UVs
        Vector2 _00 = new Vector2(0f, 0f);
        Vector2 _10 = new Vector2(1f, 0f);
        Vector2 _01 = new Vector2(0f, 1f);
        Vector2 _11 = new Vector2(1f, 1f);

        Vector2[] uvs = new Vector2[]
        {
            // Bottom
            _11, _01, _00, _10,
        
            // Left
            _11, _01, _00, _10,
        
            // Front
            _11, _01, _00, _10,
        
            // Back
            _11, _01, _00, _10,
        
            // Right
            _11, _01, _00, _10,
        
            // Top
            _11, _01, _00, _10,
        };
        #endregion

        #region Triangles
        int[] triangles = new int[]
        {
            // Bottom
            3, 1, 0,
            3, 2, 1,			
        
            // Left
            3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
            3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
        
            // Front
            3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
            3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
        
            // Back
            3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
            3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
        
            // Right
            3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
            3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
        
            // Top
            3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
            3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,

        };
        #endregion

        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        mesh.Optimize();

        return mesh;
    }

}