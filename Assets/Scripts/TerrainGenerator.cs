using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Perlin), typeof(DiamondSquare))]
[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private float _maxHeight;
    [SerializeField] private int _mapSize;
    [SerializeField] private int _seed;
    [SerializeField] private float _scale;
    [SerializeField] private Vector2 _offset;
    public int _activeNoise;
    private Perlin _perlin;
    private DiamondSquare _diamondSquare;
    //[SerializeField] GameObject _terrainObject; 
    private Mesh _mesh;
    public Terrain _terrain;
    private float[,] _heightMap;

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
        // initialise noise and generate height map
        switch (_activeNoise)
        {
            case 0:
                _perlin.Init(_seed, _mapSize);
                _heightMap = _perlin.GenerateHeightMap(_scale, _maxHeight, _offset);
                break;
            case 1:
                _diamondSquare.Init(_seed, _mapSize);
                _heightMap = _diamondSquare.GenerateHeightMap(_scale, _maxHeight, _offset);
                break;
            default: // use perlin as default
                _perlin.Init(_seed, _mapSize);
                _heightMap = _perlin.GenerateHeightMap(_scale, _maxHeight, _offset);
                break;
        }
        
        GenerateMesh();
        //GenerateTerrain();
    }

    public void GenerateTerrain()
    {
        TerrainData t = new TerrainData();
        t.heightmapResolution = _mapSize;
        t.size = new Vector3(_mapSize + 1, _maxHeight, _mapSize + 1);
        t.SetHeights(0, 0, heightMap);
        _terrain.terrainData = t;
    }

    public void GenerateMesh()
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        for(int i = 0; i < _mapSize; i++)
        {
            for(int j = 0; j < _mapSize; j++)
            {
                // add a new vertex using the heightmap data for Y
                verts.Add(new Vector3(i, heightMap[i, j], j));

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
