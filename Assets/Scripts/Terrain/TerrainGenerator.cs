using System.Collections.Generic;
using MyBox;
using UnityEngine;

[RequireComponent(typeof(Perlin), typeof(DiamondSquare), typeof(Worley))]
[RequireComponent(typeof(HydraulicErosion), typeof(ThermalErosion))]
[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [Space]
    [SerializeField] private Transform _pivotPoint;
    [Header("Noise properties")]
    public int _seed;
    public int _mapSize;
    private int oldSize = 128;
    public float _maxHeight;
    
    [Space]
    public NoiseType _activeNoise;
    private Perlin _perlin;
    private DiamondSquare _diamondSquare;
    private Worley _worley;
    public ErosionType _activeErosion;
    private HydraulicErosion _hydraulic;
    private ThermalErosion _thermal;
    [Space]
    private Mesh _mesh;
    private Terrain _terrain;
    
    [Space]
    [SerializeField] private bool _useTerrain;
    [ConditionalField(nameof(_useTerrain))]
    [SerializeField] private Material _terrainMat;
    
    [Space]
    public bool _rotate;
    [ConditionalField(nameof(_rotate))]
    public float _rotateSpeed;

    [Space]
    public bool _ridged = true;

    public enum NoiseType
    {
        Perlin = 0,
        DiamondSquare,
        Worley
    }
    
    public enum ErosionType
    {
        None = 0,
        Hydraulic,
        Thermal
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
        if ((_mapSize & (_mapSize - 1)) == 0)
        {
            oldSize = _mapSize;
        }
        else
        {
            _mapSize = oldSize;
        }
        
        _perlin = gameObject.GetComponent<Perlin>();
        _diamondSquare = gameObject.GetComponent<DiamondSquare>();
        _worley = gameObject.GetComponent<Worley>();
        _hydraulic = gameObject.GetComponent<HydraulicErosion>();
        _thermal = gameObject.GetComponent<ThermalErosion>();
        
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
            case NoiseType.Worley:
                heightMap = _worley.GenerateHeightMap(_seed, _mapSize);
                break;
            default: // use perlin as default
                _perlin.Init(_seed, _mapSize);
                heightMap = _perlin.GenerateHeightMap();
                break;
        }
        for (int i = 0; i < _mapSize; i++)
        {
            for (int j = 0; j < _mapSize; j++)
            {
                heightMap[i, j] *= _maxHeight;
            }
        }

        float[,] erodedMap;
        switch (_activeErosion)
        {
            case ErosionType.None:
                erodedMap = heightMap;
                break;
            case ErosionType.Hydraulic:
                erodedMap = _hydraulic.ErodeMap(heightMap, _mapSize, _seed);
                break;
            case ErosionType.Thermal:
                erodedMap = _thermal.ErodeHeightMap(heightMap, _mapSize);
                break;
            default:
                // use none as default
                erodedMap = heightMap;
                break;
        }
        //float[,] erodedMap = _hydraulic.ErodeMap(heightMap, _mapSize, _seed);
        // for (int i = 0; i < _mapSize; i++)
        // {
        //     for (int j = 0; j < _mapSize; j++)
        //     {
        //         erodedMap[i, j] *= _maxHeight;
        //     }
        // }
        
        float[] result = IsMapOk(erodedMap);
        switch (result[0])
        {
            case -1:
                //Debug.Log("Mesh is ok");
                for (int i = 0; i < _mapSize; i++)
                {
                    for (int j = 0; j < _mapSize; j++)
                    {
                        if (_ridged)
                        {
                            // make all points positive to give a ridge
                            erodedMap[i, j] = Mathf.Abs(erodedMap[i, j]);
                            // invert values. points previously lower than 0 will make a ridge as the highest values
                            erodedMap[i, j] *= -1;
                            // add the height of the terrain to make up for the inverted height
                            erodedMap[i, j] += (erodedMap[i, j] / _maxHeight);
                        }
                    }
                }
                
                if (_useTerrain)
                {
                    _mesh.Clear();
                    _terrain.enabled = true;
                    GenerateTerrain(erodedMap);
                }
                else
                {
                    _terrain.enabled = false;
                    GenerateMesh(erodedMap);
                }
                break;
            default:
                Debug.Log("Mesh not ok - broke at " + result[0] + "," + result[1]);
                break;
        }
        
        // reset rotation to properly move objects
        transform.position = _pivotPoint.transform.position;
        Quaternion prevRotation = _pivotPoint.rotation;
        _pivotPoint.rotation = Quaternion.identity;
        
        // subtract position by half mapsize to make it rotate around 0,0
        Vector3 position = transform.position;
        transform.position = new Vector3(position.x - (_mapSize * 0.5f), position.y, position.z - (_mapSize * 0.5f));

        // set camera pos
        _camera.transform.position = new Vector3(-(_mapSize * 0.5f) - (_mapSize * 0.2f), _camera.transform.position.y, 0.0f);
        
        // reset rotation back to previous state
        _pivotPoint.rotation = prevRotation;
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
                float y = map[i, j];
                verts.Add(new Vector3(i, y/* * _maxHeight*/, j));

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

    public int SetMapSize(int size)
    {
        if (size != oldSize)
        {
            // check if mapsize is power of 2
            if ((size & (size - 1)) == 0)
            {
                oldSize = size;
                _mapSize = size;
            }
            else
            {
                _mapSize = oldSize;
            }
        }
        return _mapSize;
    }
    
    private float[] IsMapOk(float[,] map)
    {
        for (int i = 0; i < _mapSize; i++)
        {
            for (int j = 0; j < _mapSize; j++)
            {
                if (!IsFinite(map[i, j]))
                {
                    return new float[]{i, j};
                }
            }
        }
        // foreach (Vector3 v in m.vertices)
        // {
        //     if (!IsFinite(v.x) || !IsFinite(v.y) || !IsFinite(v.z))
        //     {
        //         return false;
        //     }
        // }

        return new float[] {-1};
    }

    public static bool IsFinite(float f)
    {
        return !float.IsInfinity(f) && !float.IsNaN(f);
    }
}
