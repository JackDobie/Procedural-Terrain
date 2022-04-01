using UnityEngine;

public class HydraulicErosion : MonoBehaviour
{
    public float _rain;
    public float _solubility;
    public float _evaporation;
    public float _capacity;
    public int _iterations;
    
    public float[,] ErodeHeightMap(float[,] heightMap, int size, int seed)
    {
        float[,] waterMap = new float[size, size];
        float[,] sedimentMap = new float[size, size];
        for (int iteration = 0; iteration < _iterations; iteration++)
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    // add rain
                    waterMap[x, y] += _rain;
                    
                    // move solid ground to sediment
                    sedimentMap[x, y] += _solubility * waterMap[x, y];
                    heightMap[x, y] -= _solubility * waterMap[x, y];
                    // return excess to heightmap
                    if (sedimentMap[x, y] > _capacity)
                    {
                        float excess = _capacity - sedimentMap[x, y];
                        sedimentMap[x, y] -= excess;
                        heightMap[x, y] += excess;
                    }
                }
            }
            
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    // transport sediment

                    // find difference in altitude from neighbours
                    float neighbourAvg = 0.0f;
                    float dTotal = 0.0f;
                    
                    float a = heightMap[i, j] + waterMap[i, j];
                    
                    // average neighbour heights and add up differences
                    int count = 0;
                    for (int x = (i > 0 ? -1 : 0); x < (i < size - 1 ? 2 : 1); x++)
                    {
                        for (int y = (j > 0 ? -1 : 0); y < (j < size - 1 ? 2 : 1); y++)
                        {
                            if (x == 0 && y == 0)
                            {
                                continue;
                            }
                            //aEverage += (waterMap[i + x - 1, j + y - 1] + heightMap[i + x - 1, j + y - 1]) / 9.0f;
                            float ai = heightMap[i + x, j + y] + waterMap[i + x, j + y];
                            neighbourAvg += ai;
                            count++;
                            float di = a - ai;
                            if (di > 0)
                            {
                                dTotal += di;
                            }
                        }
                    }
                    neighbourAvg /= count;

                    // the height of current cell minus average total height
                    float deltaA = a - neighbourAvg;
                    
                    for (int x = (i > 0 ? -1 : 0); x < (i < size - 1 ? 2 : 1); x++)
                    {
                        for (int y = (j > 0 ? -1 : 0); y < (j < size - 1 ? 2 : 1); y++)
                        {
                            if (x == 0 && y == 0)
                            {
                                continue;
                            }
                            float ai = heightMap[i + x, j + y] + waterMap[i + x, j + y];
                            float di = a - ai;

                            // the amount of water moved to neighbour
                            float deltaW = 0;
                            if (dTotal != 0)
                            {
                                deltaW = Mathf.Min(waterMap[i, j], deltaA) * di / dTotal;

                                if (waterMap[i, j] != 0)
                                {
                                    // transport sediment to neighbouring cells
                                    sedimentMap[i + x, j + y] += sedimentMap[i, j] * (deltaW / waterMap[i, j]);
                                    sedimentMap[i, j] -= sedimentMap[i, j] * (deltaW / waterMap[i, j]);
                                }
                            }
                        }
                    }

                    waterMap[i, j] *= (1 - _evaporation);
                    float mMax = _capacity * waterMap[i, j];
                    float deltaM = Mathf.Max(0, sedimentMap[i, j] - mMax);
                    sedimentMap[i, j] -= deltaM;
                    heightMap[i, j] += deltaM;
                }
            }
        }
        
        return heightMap;
    }
}
