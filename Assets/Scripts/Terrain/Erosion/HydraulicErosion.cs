using UnityEngine;
using Random = UnityEngine.Random;

public class HydraulicErosion : MonoBehaviour
{
    private float[,] _map;
    private int _mapSize;

    public int _iterations;
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

    private struct Droplet
    {
        public Droplet(Vector2 position_, Vector2 direction_, float velocity_, float sediment_, float water_)
        {
            position = position_;
            direction = direction_;
            velocity = velocity_;
            sediment = sediment_;
            water = water_;
        }
        
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

    private struct HeightAndGradient
    {
        public Vector2 gradient;
        public float height;
    }
    
    private void Init(float[,] map, int size, int seed)
    {
        Random.InitState(seed);
        _map = map;
        _mapSize = size;
    }
    
    public float[,] ErodeMap(float[,] map, int size, int seed)
    {
        Init(map, size, seed);
        //Vector2 randpos = new Vector2(100 + Random.Range(0, 10), 100 + Random.Range(0, 10));
        for (int it = 0; it < _iterations; it++)
        {
            for (int i = 0; i < _particleCount; i++)
            {
                Droplet d = new Droplet(new Vector2(Random.Range(0, size), Random.Range(0, size)),
                    Vector2.zero, 0, 0, 0.5f);
                for (int j = 0; j < _lifetime; j++)
                {
                    HeightAndGradient hg = CalculateHeightAndGradient(d);

                    // calculate new direction by blending gradient and oldDir, affected by inertia
                    // dirNew = dirOld * inertia - g * (1 - inertia)
                    d.direction = (d.direction * _inertia) - (hg.gradient * (1 - _inertia));
                    d.direction = d.direction.normalized;

                    // calculate new position by adding direction to old pos
                    //d.position = oldPos + d.direction;
                    d.position += d.direction;

                    if (d.position.x < 0 || d.position.y < 0 || d.position.x >= _mapSize - 1 || d.position.y >= _mapSize - 1)
                    {
                        break;
                    }

                    //HeightAndGradient hgNew = CalculateHeightAndGradient(d);
                    float newHeight = CalculateHeight2(d);

                    // calculate height difference
                    float deltaHeight = hg.height - newHeight;

                    if (deltaHeight > 0.0f) // droplet moved uphill
                    {
                        // drop all sediment
                        d.sediment = Deposit(d, Mathf.Min(d.sediment, deltaHeight));
                    }
                    else // moved downhill
                    {
                        // calculate new carry capacity
                        float c = Mathf.Max(-deltaHeight, _minSlope) * d.velocity * d.water * _capacity;

                        // if carrying more than capacity, drop a percent of the sediment
                        if (d.sediment >= c)
                        {
                            //depositAmount = (d.sediment - c) * _depositSpeed;
                            d.sediment = Deposit(d, (d.sediment - c) * _depositSpeed);
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

        x = Mathf.Clamp(x, (int)0, _mapSize - 1);
        y = Mathf.Clamp(y, (int)0, _mapSize - 1);
        // distribute among surrounding points
        // no radius is used because would lift up pits, not fill them
        
        _map[x, y] += amount * (1 - dx) * (1 - dy); // 00
        if (x + 1 < _mapSize)
        {
            _map[x + 1, y] += amount * dx * (1 - dy); // 10
        
            if (y + 1 < _mapSize)
            {
                _map[x + 1, y + 1] += amount * dx * dy; // 11
            }
        }
        if (y + 1 < _mapSize)
        {
            _map[x, y + 1] += amount * (1 - dx) * dy; // 01
        }
        
        drop.sediment -= amount;
        return drop.sediment;
    }

    private float Erode(Droplet d, float amount)
    {
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
        for (int x = xStart; x < xEnd; x++)
        {
            for (int y = yStart; y < yEnd; y++)
            {
                float deltaX = x - d.position.x;
                float deltaY = y - d.position.y;
                float distance = Mathf.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
                float weight = Mathf.Max(0, _erosionRadius - distance);
                wSum += weight;
                weights[x - x0, y - y0] = weight;
            }
        }

        for (int x = xStart; x < xEnd; x++)
        {
            for (int y = yStart; y < yEnd; y++)
            {
                // normalise weights and remove from map
                if (wSum > 0)
                {
                    // x counts up from x0, so use the difference as the index
                    int indexX = x - x0;
                    int indexY = y - y0;
                    weights[indexX, indexY] /= wSum;
                    float erodeAmount = amount * weights[indexX, indexY];
                    _map[x, y] -= erodeAmount;
                    d.sediment += erodeAmount;
                }
            }
        }
        
        return d.sediment;
    }

    private float CalculateHeight(Droplet d)
    {
        Vector2Int pos = d.posInt;
        pos.x = Mathf.Clamp(pos.x, 0, _mapSize - 1);
        pos.y = Mathf.Clamp(pos.x, 0, _mapSize - 1);
        float xf = d.position.x - pos.x;
        float yf = d.position.y - pos.y;
        
        float h00, h10, h01, h11;
        float[] corners = GetCorners(pos);
        h00 = corners[0];
        h10 = corners[1];
        h01 = corners[2];
        h11 = corners[3];
        
        return CalculateHeight(h00, h10, h01, h11, xf, yf);
    }
    
    private float CalculateHeight(float h00, float h10, float h01, float h11, float xf, float yf)
    {
        return h01 * (1 - xf) * (1 - yf) + h01 * xf * (1 - yf) + h10 * (1 - xf) * yf + h11 * xf * yf;
    }

    private float CalculateHeight2(Vector2 posdif, float[] corners)
    {
        float h00 = corners[0];
        float h10 = corners[1];
        float h01 = corners[2];
        float h11 = corners[3];

        float l = (1 - posdif.y) * h00 + posdif.y * h01;
        float r = (1 - posdif.y) * h10 + posdif.y * h11;
        
        return (1 - posdif.x) * l + posdif.x * r;
    }
    
    private float CalculateHeight2(Droplet d)
    {
        Vector2 posf = d.position;
        Vector2Int posi = d.posInt;
        Vector2 posdif = posf - posi;
        
        float[] corners = GetCorners(posi);

        return CalculateHeight2(posdif, corners);
    }
    
    private HeightAndGradient CalculateHeightAndGradient(Droplet d)
    {
        Vector2Int posi = d.posInt;

        posi.x = Mathf.Clamp(posi.x, 0, _mapSize - 1);
        posi.y = Mathf.Clamp(posi.y, 0, _mapSize - 1);

        Vector2 posdif = d.position - posi;
        
        // calc heights of the four corners
        float[] corners = GetCorners(posi);
        float h00 = corners[0];
        float h10 = corners[1];
        float h01 = corners[2];
        float h11 = corners[3];

        // calc droplet direction with bilinear interpolation
        float gradX = (h00 - h10) * (1 - posdif.y) + (h01 - h11) * posdif.y;
        float gradY = (h00 - h01) * (1 - posdif.x) + (h10 - h11) * posdif.x;
        Vector2 grad = new Vector2(gradX, gradY);
        //Vector2 grad = gradX + gradY;

        // calc height
        float height = CalculateHeight2(posdif, corners);

        return new HeightAndGradient() {height = height, gradient = grad};
    }

    private float[] GetCorners(int x, int y)
    {
        float h00, h10, h01, h11;
        h00 = h10 = h01 = h11 = _map[x, y];
        // if out of bounds, set to current cell (h00)
        if (x + 1 < _mapSize)
        {
            h10 = _map[x + 1, y];
            
            if (y + 1 < _mapSize)
                h11 = _map[x + 1, y + 1];
        }
        if (y + 1 < _mapSize)
            h01 = _map[x, y + 1];

        return new float[] {h00, h10, h01, h11};
    }
    
    private float[] GetCorners(Vector2Int pos)
    {
        return GetCorners(pos.x, pos.y);
    }
}
