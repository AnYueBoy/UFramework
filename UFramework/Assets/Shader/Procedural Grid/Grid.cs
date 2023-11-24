using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour
{
    public int xSize, ySize;
    private Vector3[] vertices;
    private Mesh mesh;

    private void Awake()
    {
        GenerateVertices();
    }

    private void GenerateVertices()
    {
        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, y = 0; y < ySize + 1; y++)
        {
            for (int x = 0; x < xSize + 1; x++, i++)
            {
                // 生成网格点位置
                vertices[i] = new Vector3(x, y);
                // 将网格点坐标映射到uv坐标
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                tangents[i] = tangent;
            }
        }

        var meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh = new Mesh();
        meshFilter.mesh.name = "Procedural Grid";
        mesh.vertices = vertices;

        int[] triangles = new int[xSize * ySize * 6];
     
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.uv = uv;
        mesh.tangents = tangents;
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }

        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}