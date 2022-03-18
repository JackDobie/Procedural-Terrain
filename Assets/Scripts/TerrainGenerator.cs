using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Perlin), typeof(DiamondSquare))]
[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator : MonoBehaviour
{
    [Header("Noise properties")]
    [SerializeField] private int _seed;
    /// <summary> if generating mesh, limit this to 254 because meshes are limited to 65000 verts</summary>
    [SerializeField] private int _mapSize;
    [SerializeField] private float _maxHeight;
    [SerializeField] private Vector2 _offset;
    [Space] public int _activeNoise;
    private Perlin _perlin;
    private DiamondSquare _diamondSquare;
    private Mesh _mesh;
    private Terrain _terrain;
    private float[,] _heightMap;
    [Space] [SerializeField] private bool _useTerrain;
    [SerializeField] private Material _terrainMat;

    private void OnValidate()
    {
        // ensure that offset is in range
        _offset.x = Mathf.Clamp(_offset.x, 0, _mapSize - 1);
        _offset.y = Mathf.Clamp(_offset.y, 0, _mapSize - 1);
    }

    private void Awake()
    {
        _perlin = gameObject.GetComponent<Perlin>();
        _diamondSquare = gameObject.GetComponent<DiamondSquare>();
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
        _terrain = GetComponent<Terrain>();
    }

    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        // initialise noise and generate height map
        switch (_activeNoise)
        {
            case 0:
                _perlin.Init(_seed, _mapSize);
                _heightMap = _perlin.GenerateHeightMap(_offset);
                break;
            case 1:
                _heightMap = _diamondSquare.GenerateHeightMap(_seed, _mapSize);
                break;
            default: // use perlin as default
                _perlin.Init(_seed, _mapSize);
                _heightMap = _perlin.GenerateHeightMap(_offset);
                break;
        }
        
        // float[,] scaledHeights = new float[_mapSize, _mapSize];
        // for (int i = 0; i < _mapSize; i++)
        // {
        //     for (int j = 0; j < _mapSize; j++)
        //     {
        //         scaledHeights[i, j] = _heightMap[(int)(i / (float)_mapSize * _scale), (int)(j / (float)_mapSize * _scale)];
        //     }
        // }

        if (_useTerrain)
        {
            _mesh.Clear();
            _terrain.enabled = true;
            GenerateTerrain(_heightMap);   
        }
        else
        {
            _terrain.enabled = false;
            GenerateMesh(_heightMap);
        }
    }

    private void GenerateTerrain(float[,] map)
    {
        TerrainData t = new TerrainData
        {
            heightmapResolution = _mapSize,
            size = new Vector3(_mapSize, _maxHeight, _mapSize),
        };
        t.SetHeights(0, 0, map);
        _terrain.terrainData = t;
        _terrain.materialTemplate = _terrainMat;
    }

    private void GenerateMesh(float[,] map)
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        for(int i = 0; i < _mapSize; i++)
        {
            for(int j = 0; j < _mapSize; j++)
            {
                // add a new vertex using the heightmap data for Y
                verts.Add(new Vector3(i, map[i, j] * _maxHeight, j));

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
