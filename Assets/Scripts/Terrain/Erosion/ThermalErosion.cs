using UnityEngine;

public class ThermalErosion : MonoBehaviour
{
    private float[,] _map;
    private int _mapSize;

    public int _iterations;
    public float _minAngle;
    public float c;
    
    private void Init(float[,] map, int size)
    {
        _map = map;
        _mapSize = size;
        if (_minAngle < 0)
        {
            _minAngle = 4.0f / _iterations;
        }
    }

    public float[,] ErodeHeightMap(float[,] map, int size)
    {
        Init(map, size);
        // find gradient of current cell by interpolating heights and gradients
        // if angle greater than min angle, distribute an amount until the angle is ok

        for (int it = 0; it < _iterations; it++)
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float[] neighbours = GetNeighbours(x, y);
                    float h00 = _map[x, y];
                    // float h10 = neighbours[0];
                    // float h01 = neighbours[1];
                    // float h11 = neighbours[2];

                    float dMax = 0.0f;
                    float dTotal = 0.0f;
                    int count = 0;
                    for (int i = 0; i < neighbours.Length; i++)
                    {
                        float di = h00 - neighbours[i];
                        if (di > 0.0f)
                        {
                            dTotal += di;
                            count++;
                            if (di > dMax)
                            {
                                dMax = di;
                            }
                        }
                    }

                    for (int i = 0; i < neighbours.Length; i++)
                    {
                        float di = h00 - neighbours[i];
                        if (di > 0.0f)
                        {
                            if (count == 1)
                            {
                                float amount = c * (di - _minAngle);
                                neighbours[i] += amount;
                                h00 -= amount;
                            }
                            else if (count > 1)
                            {
                                float amount = (c * (dMax - _minAngle)) * di / dTotal;
                                neighbours[i] += amount;
                                h00 -= amount;
                            }
                        }
                    }

                    // assign back 
                    _map[x, y] = h00;
                    if (x < _mapSize - 1)
                    {
                        _map[x + 1, y] = neighbours[0];
                        if (y < _mapSize - 1)
                        {
                            _map[x + 1, y + 1] = neighbours[2];
                        }
                    }
                    if (y < _mapSize - 1)
                    {
                        _map[x, y + 1] = neighbours[1];
                    }
                }
            }
        }

        return _map;
    }

    private float[] GetNeighbours(int x, int y)
    {
        float h10, h01, h11;
        h10 = h01 = h11 = _map[x, y];
        // if out of bounds, set to current cell (h00)
        if (x + 1 < _mapSize)
        {
            h10 = _map[x + 1, y];
            
            if (y + 1 < _mapSize)
                h11 = _map[x + 1, y + 1];
        }
        if (y + 1 < _mapSize)
            h01 = _map[x, y + 1];

        return new float[] {h10, h01, h11};
    }
}
