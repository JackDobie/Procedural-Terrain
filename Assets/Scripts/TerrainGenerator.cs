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
        _perlin = gameObject.GetComponent<Perlin>();
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
        _terrain = GetComponent<Terrain>();
    }

    void Start()
    {
        GenerateMesh();
        //GenerateTerrain();
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
        _perlin.Init(_seed, _mapSize);
        float[,] _heightMap = _perlin.GenerateHeightMap(_scale, _maxHeight, _offset);
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        for(int i = 0; i < _mapSize; i++)
        {
            for(int j = 0; j < _mapSize; j++)
            {
                // add a new vertex using the heightmap data for Y
                verts.Add(new Vector3(i, _heightMap[i, j], j));

                if (i == 0 || j == 0) continue;

                // adds the indexes of three verts in order to make up each of two triangles
                tris.Add(_mapSize * i + j); // TR
                tris.Add(_mapSize * i + j - 1); // BR
                tris.Add(_mapSize * (i - 1) + j - 1); // BL
                tris.Add(_mapSize * (i - 1) + j - 1); // BL
                tris.Add(_mapSize * (i - 1) + j); // TL
                tris.Add(_mapSize * i + j); // TR
            }
        }

        // update mesh
        _mesh.Clear();
        _mesh.vertices = verts.ToArray();
        _mesh.triangles = tris.ToArray();
        _mesh.RecalculateNormals();

        // generate heightmap
        // apply erosion
        // create mesh using heightmap
    }
}
