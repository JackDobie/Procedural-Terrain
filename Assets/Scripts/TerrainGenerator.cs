using System.Collections.Generic;
using MyBox;
using UnityEngine;

[RequireComponent(typeof(Perlin), typeof(DiamondSquare))]
[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private Transform _pivotPoint;
    [Header("Noise properties")]
    [SerializeField] private int _seed;
    [SerializeField] private int _mapSize;
    private int oldSize = 128;
    [SerializeField] private float _maxHeight;
    
    [Space] public NoiseType _activeNoise;
    private Perlin _perlin;
    private DiamondSquare _diamondSquare;
    private Mesh _mesh;
    private Terrain _terrain;
    
    [Space] [SerializeField] private bool _useTerrain;
    [ConditionalField(nameof(_useTerrain))]
    [SerializeField] private Material _terrainMat;
    
    [Space] [SerializeField] private bool _rotate;
    [ConditionalField(nameof(_rotate))]
    [SerializeField] private float _rotateSpeed;

    public enum NoiseType
    {
        Perlin,
        DiamondSquare
    }

    private void OnValidate()
    {
        // if mapsize changed
        if (oldSize != _mapSize)
        {
            // check if mapsize is power of 2
            if ((_mapSize & (_mapSize - 1)) == 0)
            {
                oldSize = _mapSize;
            }
            else
            {
                _mapSize = oldSize;
            }
        }
    }

    private void Awake()
    {
        _perlin = gameObject.GetComponent<Perlin>();
        _diamondSquare = gameObject.GetComponent<DiamondSquare>();
        _mesh = new Mesh
        {
            // allows creating mesh with up to 2^32 verts rather than 2^16 at the cost of memory. can create mesh larger than 128x128
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };
        GetComponent<MeshFilter>().mesh = _mesh;
        _terrain = GetComponent<Terrain>();
    }

    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        float[,] heightMap;
        // initialise noise and generate height map
        switch (_activeNoise)
        {
            case NoiseType.Perlin:
                _perlin.Init(_seed, _mapSize);
                heightMap = _perlin.GenerateHeightMap();
                break;
            case NoiseType.DiamondSquare:
                heightMap = _diamondSquare.GenerateHeightMap(_seed, _mapSize);
                break;
            default: // use perlin as default
                _perlin.Init(_seed, _mapSize);
                heightMap = _perlin.GenerateHeightMap();
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
            GenerateTerrain(heightMap);   
        }
        else
        {
            _terrain.enabled = false;
            GenerateMesh(heightMap);
        }
        
        // reset rotation
        transform.rotation = Quaternion.identity;
        if(_rotate)
        {
            // subtract position by half mapsize to make it rotate around the centre
            Vector3 position = transform.position;
            transform.position =
                new Vector3(position.x - (_mapSize * 0.5f), position.y, position.z - (_mapSize * 0.5f));
        }
    }

    private void GenerateTerrain(float[,] map)
    {
        int size = (int)Mathf.Sqrt(map.Length);
        TerrainData t = new TerrainData
        {
            heightmapResolution = size,
            size = new Vector3(size, _maxHeight, size),
        };
        t.SetHeights(0, 0, map);
        _terrain.terrainData = t;
        _terrain.materialTemplate = _terrainMat;
    }

    private void GenerateMesh(float[,] map)
    {
        int size = (int)Mathf.Sqrt(map.Length);
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                // add a new vertex using the heightmap data for Y
                verts.Add(new Vector3(i, map[i, j] * _maxHeight, j));

                if (i == 0 || j == 0) continue;

                // adds the indexes of three verts in order to make up each of two triangles
                tris.Add(size * i + j); // TR
                tris.Add(size * i + j - 1); // BR
                tris.Add(size * (i - 1) + j - 1); // BL
                tris.Add(size * (i - 1) + j - 1); // BL
                tris.Add(size * (i - 1) + j); // TL
                tris.Add(size * i + j); // TR
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

    private void Update()
    {
        if(_rotate)
        {
            _pivotPoint.rotation *= Quaternion.Euler(Vector3.up * (_rotateSpeed * Time.deltaTime));
        }
    }
}
