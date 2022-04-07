using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class HydraulicErosion : MonoBehaviour
{
    public uint _iterations;
    private float[,] _map;
    private int _mapSize;

    public int _particleCount = 1000;
    public int _lifetime = 30;
    public int _gravity = 4;
    public int _erosionRadius = 2;
    [Range(0, 1)] public float _inertia = 0.1f;
    public float _capacity = 10.0f;
    [Range(0, 1)] public float _evaporationSpeed = 0.1f;
    [Range(0, 1)] public float _erosionSpeed = 0.1f;
    [Range(0, 1)] public float _depositSpeed = 1.0f;
    public float _minSlope = 0.001f;
    //public uint _maxPath;

    //private List<Droplet> _droplets;

    struct Droplet
    {
        public Vector2 position;
        public Vector2 direction;
        public float velocity;
        public float sediment;
        public float water;

        public Vector2Int posInt
        {
            get
            {
                int x = Mathf.FloorToInt(position.x);
                int y = Mathf.FloorToInt(position.y);
                return new Vector2Int(x, y);
            }
            set
            {
                position = value;
            }
        }
    }

    struct HeightAndGradient
    {
        public Vector2 gradient;
        public float height;
    }
    
    private void Init(float[,] map, int size, int seed)
    {
        Random.InitState(seed);
        _map = map;
        _mapSize = size;
        //_droplets = new List<Droplet>();
    }
    
    public float[,] ErodeMap(float[,] map, int size, int seed)
    {
        Init(map, size, seed);
        for (int it = 0; it < _iterations; it++)
        {
            for (int i = 0; i < _particleCount; i++)
            {
                Droplet d;
                d.position = new Vector2(Random.Range(0, size), Random.Range(0, size));
                d.direction = Vector2.zero;
                d.velocity = 0;
                d.sediment = 0;
                d.water = 0;
                //_droplets.Add(d);
                for (int j = 0; j < _lifetime; j++)
                {
                    Vector2 oldPos = d.position;
                    HeightAndGradient hg = CalculateHeightAndGradient(map, d);

                    // calculate new direction by blending gradient and oldDir, affected by inertia
                    // dirNew = dirOld * inertia - g * (1 - inertia)
                    Vector2 oldDir = d.direction;
                    d.direction = new Vector2(oldDir.x * _inertia - hg.gradient.x * (1 - _inertia),
                        oldDir.y * _inertia - hg.gradient.y * (1 - _inertia));
                    d.direction = d.direction.normalized;

                    // calculate new position by adding direction to old pos
                    //d.position = oldPos + d.direction;
                    d.position += d.direction;

                    if (d.position.x < 0 || d.position.y < 0 || d.position.x >= _mapSize || d.position.y >= _mapSize)
                    {
                        break;
                    }

                    HeightAndGradient hgNew = CalculateHeightAndGradient(map, d);

                    // calculate height difference
                    float deltaHeight = hg.height - hgNew.height;

                    if (deltaHeight > 0.0f) // droplet moved uphill
                    {
                        // drop all sediment
                        //depositAmount = d.sediment;
                        float amount = Mathf.Min(d.sediment, deltaHeight);
                        d.sediment = Deposit(d, amount);
                    }
                    else // moved downhill
                    {
                        // calculate new carry capacity
                        float c = Mathf.Max(-deltaHeight, _minSlope) * d.velocity * d.water * _capacity;

                        // if carrying more than capacity, drop a percent of the sediment. prevents spikes
                        if (d.sediment >= c)
                        {
                            //depositAmount = (d.sediment - c) * _depositSpeed;
                            d.sediment = Deposit(d, (d.sediment - c) * _depositSpeed);
                            // alt:
                            // depositAmount = Mathf.Min(deltaHeight, sediment);
                        }
                        else
                        {
                            // carrying under capacity, can erode up to deltaheight
                            //erodeAmount = Mathf.Min((c - d.sediment) * _erosionSpeed, -deltaHeight);
                            d.sediment = Erode(d, Mathf.Min((c - d.sediment) * _erosionSpeed, -deltaHeight));
                        }
                    }

                    // adjust speed and evaporate water
                    d.velocity = Mathf.Sqrt((d.velocity * d.velocity) + deltaHeight * _gravity);
                    d.water *= 1 - _evaporationSpeed;

                    // for (int x = 0; x < _mapSize; x++)
                    // {
                    //     for (int y = 0; y < _mapSize; y++)
                    //     {
                    //         if (!TerrainGenerator.IsFinite(_map[x,y]))
                    //         {
                    //             Debug.Log("error! iteration: " + it + ", particle: " + i + ", life: " + j);
                    //         }
                    //     }
                    // }
                }
            }
        }

        return _map;
    }

    private float Deposit(Droplet drop, float amount)
    {
        int x = drop.posInt.x;
        int y = drop.posInt.y;
        float dx = drop.position.x - x;
        float dy = drop.position.y - y;
        // int xStart = Mathf.Max(x - 1, 0);
        // int yStart = Mathf.Max(y - 1, 0);
        // int xEnd = Mathf.Min(x + 1 + 1, _mapSize);
        // int yEnd = Mathf.Min(y + 1 + 1, _mapSize);

        x = Mathf.Clamp(x, (int)0, _mapSize - 1);
        y = Mathf.Clamp(y, (int)0, _mapSize - 1);
        // distribute among surrounding points
        // no radius is used because would lift up pits, not fill them
        if (x - 1 >= 0)
        {
            // -1 0
            _map[x - 1, y] += amount * (1 - dx) * (1 - dy);
        }
        if (x + 1 < _mapSize)
        {
            // 1 0
            _map[x + 1, y] += amount * dx * (1 - dy);
        }
        if (y - 1 >= 0)
        {
            // 0 1
            _map[x, y - 1] += amount * (1 - dx) * dy;
        }
        if (y + 1 < _mapSize)
        {
            // 1 0
            _map[x, y + 1] += amount * dx * dy;
        }
        _map[x, y] += amount;

        // float u = _map[x, y + 1];
        // float d = _map[x, y - 1];
        // float l = _map[x - 1, y];
        // float r = _map[x + 1, y];
        // for (int i = xStart; i < xEnd; i++)
        // {
        //     for (int j = yStart; j < yEnd; j++)
        //     {
        //         if (i == 0 || j == 0)
        //         {
        //             _map[x, y] += amount;
        //         }
        //     }
        // }
        
        drop.sediment -= amount;
        return drop.sediment;
    }

    private float Erode(Droplet d, float amount)
    {
        //float halfRadius = _erosionRadius * 0.5f;
        
        // use max to prevent trying to access out of bounds
        // float fxStart = Mathf.Max(d.position.x - halfRadius, 0);
        // float fyStart = Mathf.Max(d.position.y - halfRadius, 0);
        // float fxEnd = Mathf.Min(d.position.x + halfRadius, _mapSize - 1);
        // float fyEnd = Mathf.Min(d.position.y + halfRadius, _mapSize - 1);
        // int ixStart = Mathf.FloorToInt(fxStart);
        // int iyStart = Mathf.FloorToInt(fyStart);
        // int ixEnd = Mathf.FloorToInt(fxEnd);
        // int iyEnd = Mathf.FloorToInt(fyEnd);

        Vector2Int pos = d.posInt;
        int x0 = pos.x - _erosionRadius;
        int y0 = pos.y - _erosionRadius;
        // use max to prevent trying to access out of bounds
        int xStart = Mathf.Max(x0, 0);
        int yStart = Mathf.Max(y0, 0);
        int xEnd = Mathf.Min(pos.x + _erosionRadius, _mapSize);
        int yEnd = Mathf.Min(pos.y + _erosionRadius, _mapSize);

        float[,] weights = new float[(_erosionRadius * 2) + 1, (_erosionRadius * 2) + 1];
        float wSum = 0;
        for (int y = yStart; y < yEnd; y++)
        {
            for (int x = xStart; x < xEnd; x++)
            {
                float deltaX = x - d.position.x;
                float deltaY = y - d.position.y;
                float distance = Mathf.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
                float weight = Mathf.Max(0, _erosionRadius - distance);
                wSum += weight;
                weights[x - x0, y - y0] = weight;
            }
        }

        for (int y = yStart; y < yEnd; y++)
        {
            for (int x = xStart; x < xEnd; x++)
            {
                // normalise weights and remove from map
                if (wSum > 0)
                {
                    float prev = _map[x - x0, y - y0];
                    weights[x - x0, y - y0] /= wSum;
                    _map[x - x0, y - y0] -= amount * (weights[x - x0, y - y0] /* * _erosionFactor*/);
                }
            }
        }
        
        d.sediment += amount;
        // _map[d.position.x, d.position.y] -= amount;
        return d.sediment;
    }
    
    private HeightAndGradient CalculateHeightAndGradient(float[,] map, Droplet d)
    {
        Vector2Int iPos = d.posInt;

        if (iPos.x < 0) iPos.x = 0;
        if (iPos.y < 0) iPos.y = 0;
        if (iPos.x >= _mapSize) iPos.x = _mapSize - 1;
        if (iPos.y >= _mapSize) iPos.y = _mapSize - 1;
        
        float xf = d.position.x - iPos.x;
        float yf = d.position.y - iPos.y;
        
        // calc heights of the four neighbours
        // float h00 = map[iPos.x, iPos.y];
        // float h10 = map[iPos.x + 1, iPos.y];
        // float h01 = map[iPos.x, iPos.y + 1];
        // float h11 = map[iPos.x + 1, iPos.y + 1];

        float h00 = map[iPos.x, iPos.y];
        float h10, h01, h11;
        // if out of bounds, set to current cell (h00)
        if (iPos.x + 1 < _mapSize)
        {
            h10 = map[iPos.x + 1, iPos.y];
            h11 = iPos.y + 1 < _mapSize ? map[iPos.x + 1, iPos.y + 1] : h00;
        }
        else
        {
            h10 = h00;
            h11 = h00;
        }
        h01 = iPos.y + 1 < _mapSize ? map[iPos.x, iPos.y + 1] : h00;
        
        // calc gradients
        Vector2 g00 = new Vector2(h10 - h00, h01 - h00);
        Vector2 g10 = new Vector2(h10 - h00, h11 - h10);
        Vector2 g01 = new Vector2(h11 - h01, h01 - h00);
        Vector2 g11 = new Vector2(h11 - h01, h11 - h10);

        // calc droplet direction with bilinear interpolation
        // float gradX = (h10 - h00) * (1 - yf) + (h11 - h01) * yf;
        // float gradY = (h01 - h00) * (1 - xf) + (h11 - h10) * xf;
        Vector2 gradX = (g10 - g00) * (1 - yf) + (g11 - g01) * yf;
        Vector2 gradY = (g01 - g00) * (1 - xf) + (g11 - g10) * xf;

        // calc height
        float height = h01 * (1 - xf) * (1 - yf) + h01 * xf * (1 - yf) + h10 * (1 - xf) * yf + h11 * xf * yf;
        //float l = (1 - yf) * h00 + 

        return new HeightAndGradient() {height = height, gradient = gradX + gradY /*new Vector2(gradX, gradY)*/};
    }
}
