using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Perlin))]
[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] float _maxHeight;
    [SerializeField] int _mapSize;
    [SerializeField] int _seed;
    [SerializeField] /*[Range(0.0f, 1.0f)]*/ float _scale;
    [SerializeField] Vector2 _offset;
    Perlin _perlin;
    //[SerializeField] GameObject _terrainObject; 
    Mesh _mesh;
    public Terrain _terrain;

    private void OnValidate()
    {
        // ensure that offset is in range
        _offset.x = Mathf.Clamp(_offset.x, 0, _mapSize - 1);
        _offset.y = Mathf.Clamp(_offset.y, 0, _mapSize - 1);
    }

    void Awake()
    {
        //_perlin = new Perlin(octaves, persistence, 1.0f, 512);
        _perlin = gameObject.GetComponent<Perlin>();
        //_mesh = _terrainObject.GetComponent<Mesh>();
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
        //_noiseTexture = new Texture2D(_mapSize.x, _mapSize.y);
        _terrain = GetComponent<Terrain>();
    }

    void Start()
    {
        //GenerateMesh();
        GenerateTerrain();
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

    public void GenerateTerrain()
    {
        _perlin.Init(_seed, _mapSize);
        float[,] _heightMap = _perlin.GenerateHeightMap(_scale, _mapSize, _offset);
        TerrainData t = new TerrainData();
        t.heightmapResolution = _mapSize;
        t.size = new Vector3(_mapSize + 1, _maxHeight, _mapSize + 1);
        t.SetHeights(0, 0, _heightMap);
        _terrain.terrainData = t;
    }

    public void GenerateMesh()
    {
        // generate height map
        //float[,] _heightMap = _perlin.GenerateHeightMap(926, _mapSize.x);
        _perlin.Init(_seed, _mapSize);

        // set vertices data from heightmap
        Vector3[] vertices = new Vector3[(_mapSize * _mapSize) * 6];
        for (int i = 0, z = 0; z < _mapSize; z++)
        {
            for (int x = 0; x < _mapSize; x++)
            {
                //float height = _heightMap[x, y] * 10.0f;
                float xCoord = (float)x / _mapSize * _scale + _offset.x;
                float yCoord = (float)z / _mapSize * _scale + _offset.y;
                //float height = _perlin.Noise(x * _scale, z * _scale) * _maxHeight;//Mathf.Clamp(_perlin.Noise(x * _scale, y * _scale), 0.0f, 1.0f) * _maxHeight;
                float height = _perlin.Noise(xCoord, yCoord) * _maxHeight;//Mathf.Clamp(_perlin.Noise(x * _scale, y * _scale), 0.0f, 1.0f) * _maxHeight;
                vertices[i] = new Vector3(x, height, z);
                //Debug.Log(x + ", " + height + ", " + y);
                i++;
            }
        }

        // set tri data
        int[] triangles = new int[_mapSize * _mapSize * 6];
        int verts = 0;
        int tris = 0;

        for(int i = 0; i < _mapSize; i++)
        {
            for(int j = 0; j < _mapSize; j++)
            {
                triangles[tris + 0] = verts + 0;
                triangles[tris + 1] = verts + _mapSize + 1;
                triangles[tris + 2] = verts + 1;
                triangles[tris + 3] = verts + 1;
                triangles[tris + 4] = verts + _mapSize + 1;
                triangles[tris + 5] = verts + _mapSize + 2;

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

        // generate heightmap
        // apply erosion
        // create mesh using heightmap
    }
}
