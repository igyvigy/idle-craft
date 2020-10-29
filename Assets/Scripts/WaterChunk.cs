using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class WaterChunk : MonoBehaviour
{
    //chunk size
    public const int waterHeight = (Chunk.Height / 2) - 4;

    //0 = air, 1 = land
    public int[,] locs = new int[Chunk.Width, Chunk.Width];

    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = new Vector3(-1, waterHeight, -1);
    }

    public void SetLocs(int3[] blocks)
    {
        int y;

        for (int x = 0; x < Chunk.Width; x++)
        {
            for (int z = 0; z < Chunk.Width; z++)
            {
                locs[x, z] = 0;

                y = Chunk.Height - 1;

                //find the ground
                var index = Utils.to1D(x, y, z);
                while (y > 0 && (BlockType)blocks[index].x == BlockType.Air)
                {
                    y--;
                }
                if (y + 1 < waterHeight)
                    locs[x, z] = 1;
            }
        }
    }

    Vector2[] uvpat = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };

    public void BuildMesh()
    {
        Mesh mesh = new Mesh();

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int x = 0; x < Chunk.Width; x++)
            for (int z = 0; z < Chunk.Width; z++)
            {
                if (locs[x, z] == 1)
                {
                    verts.Add(new Vector3(x, 0, z));
                    verts.Add(new Vector3(x, 0, z + 1));
                    verts.Add(new Vector3(x + 1, 0, z + 1));
                    verts.Add(new Vector3(x + 1, 0, z));

                    verts.Add(new Vector3(x, 0, z));
                    verts.Add(new Vector3(x, 0, z + 1));
                    verts.Add(new Vector3(x + 1, 0, z + 1));
                    verts.Add(new Vector3(x + 1, 0, z));

                    uvs.AddRange(uvpat);
                    uvs.AddRange(uvpat);
                    int tl = verts.Count - 8;
                    tris.AddRange(new int[] { tl, tl + 1, tl + 2, tl, tl + 2, tl + 3,
                        tl+3+4,tl+2+4,tl+4,tl+2+4,tl+1+4,tl+4});
                }
            }

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}
