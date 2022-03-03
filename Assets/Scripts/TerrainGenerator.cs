using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Perlin))]
[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] float _maxHeight;
    [SerializeField] Vector2Int _mapSize;
    [SerializeField] int _seed;
    [SerializeField] float _scale;
    //[SerializeField] Material _noiseMat;
    //float[,] _heightMap;
    Perlin _perlin;
    [SerializeField] GameObject _terrainObject;
    //MeshFilter _meshFilter;
    Mesh _mesh;

    void Awake()
    {
        //_perlin = new Perlin(octaves, persistence, 1.0f, 512);
        _perlin = gameObject.GetComponent<Perlin>();
        //_mesh = _terrainObject.GetComponent<Mesh>();
        _mesh = new Mesh();
        _terrainObject.GetComponent<MeshFilter>().mesh = _mesh;
        //_noiseTexture = new Texture2D(_mapSize.x, _mapSize.y);
    }

    void Start()
    {
        GenerateMesh();
    }

    void GenerateTexture()
    {
        //_heightMap = _perlin.GenerateHeightMap(926, _mapSize.x);
        //Texture2D tex = new Texture2D(1000, 1000);
        //for(int i = 0; i < 100; i++)
        //{
        //    for (int j = 0; j < 100; j++)
        //    {
        //        Color c = new Color(_heightMap[i, j], _heightMap[i, j], _heightMap[i, j], 1.0f);
        //        tex.SetPixel(i, j, c);
        //    }
        //}
        //if (tex)
        //{
        //    _noiseMat.SetTexture("_MainTex", tex);
        //    //_noiseMat.mainTexture = tex;
            
        //}
        //else
        //{
        //    Debug.LogError("TEX!!!!");
        //}

        //_noiseMat.SetTexture("_MainTex", tex);
        //_heightMap = _perlin.GenerateHeightMap();
    }

    void GenerateMesh()
    {
        // generate height map
        float[,] _heightMap = _perlin.GenerateHeightMap(926, _mapSize.x);

        // set vertices data from heightmap
        Vector3[] vertices = new Vector3[(_mapSize.x + 1) * (_mapSize.y + 1)];
        int i = 0, y = 0, x = 0;
        try
        {
            for (i = 0, y = 0; y < _mapSize.y; y++)
            {
                for (x = 0; x < _mapSize.x; x++)
                {
                    float height = _heightMap[x, y] * 10.0f;
                    vertices[i] = new Vector3(x, height, y);
                    //Debug.Log(x + ", " + height + ", " + y);
                    i++;
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Out of range: i=" + i + ", y=" + y + ", x=" + x);
        }

        // set tri data
        int[] triangles = new int[_mapSize.x * _mapSize.y * 6];
        int verts = 0;
        int tris = 0;

        for(i = 0; i < _mapSize.y; i++)
        {
            for(int j = 0; j < _mapSize.x; j++)
            {
                triangles[tris + 0] = verts + 0;
                triangles[tris + 1] = verts + _mapSize.x + 1;
                triangles[tris + 2] = verts + 1;
                triangles[tris + 3] = verts + 1;
                triangles[tris + 4] = verts + _mapSize.x + 1;
                triangles[tris + 5] = verts + _mapSize.x + 2;

                verts++;
                tris += 6;
            }
            verts++;
        }

        // update mesh
        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();
        //_mesh.RecalculateBounds();
        //MeshCollider meshCollider = _terrainObject.GetComponent<MeshCollider>();
        //meshCollider.sharedMesh = _mesh;

        // generate heightmap
        // apply erosion
        // create mesh using heightmap
    }
}
