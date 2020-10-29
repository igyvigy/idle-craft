using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Collections;
using Unity.Mathematics;

[Serializable]
public class Wall : MonoBehaviour
{
    public int4 wallPos;

    void Awake()
    {

    }
    void OnDestroy()
    {

    }

    void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position, "wall: " + wallPos);

#endif
    }

    public void BuildMesh(int2 dir)
    {
        Mesh mesh = new Mesh();
        NativeList<Vector3> verts = new NativeList<Vector3>(Allocator.Temp);
        NativeList<int> tris = new NativeList<int>(Allocator.Temp);
        List<Vector2> uvs = new List<Vector2>();

        float3 blockPos = new float3(0, 0, 0);

        int numFaces = 0;

        //front
        if (dir.y < 0)
        {
            verts.Add(blockPos + new float3(0, 0, 0));
            verts.Add(blockPos + new float3(0, 1, 0));
            verts.Add(blockPos + new float3(1, 1, 0));
            verts.Add(blockPos + new float3(1, 0, 0));
            numFaces++;

            uvs.AddRange(BlockTexture.Get(BlockType.BedRock, 0).sidePos.GetUVs());
        }

        //right
        if (dir.x > 0)
        {
            verts.Add(blockPos + new float3(1, 0, 0));
            verts.Add(blockPos + new float3(1, 1, 0));
            verts.Add(blockPos + new float3(1, 1, 1));
            verts.Add(blockPos + new float3(1, 0, 1));
            numFaces++;

            uvs.AddRange(BlockTexture.Get(BlockType.BedRock, 0).sidePos.GetUVs());
        }

        //back
        if (dir.y > 0)
        {
            verts.Add(blockPos + new float3(1, 0, 1));
            verts.Add(blockPos + new float3(1, 1, 1));
            verts.Add(blockPos + new float3(0, 1, 1));
            verts.Add(blockPos + new float3(0, 0, 1));
            numFaces++;

            uvs.AddRange(BlockTexture.Get(BlockType.BedRock, 0).sidePos.GetUVs());
        }

        //left
        if (dir.x < 0)
        {
            verts.Add(blockPos + new float3(0, 0, 1));
            verts.Add(blockPos + new float3(0, 1, 1));
            verts.Add(blockPos + new float3(0, 1, 0));
            verts.Add(blockPos + new float3(0, 0, 0));
            numFaces++;

            uvs.AddRange(BlockTexture.Get(BlockType.BedRock, 0).sidePos.GetUVs());
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

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
        verts.Dispose();
        tris.Dispose();
    }

}
