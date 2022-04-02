using UnityEngine;

public class HydraulicErosion : MonoBehaviour
{
    public float _rain;
    public float _solubility;
    public float _evaporation;
    public float _capacity;
    public int _iterations;
    
    public float[,] ErodeHeightMap(float[,] heightMap, int size, float maxHeight)
    {
        float[,] h = new float[size,size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                h[i, j] = heightMap[i, j];
            }
        }
        
        float Kr = _rain;
        float Ks = _solubility;
        float Ke = _evaporation;
        float Kc = _capacity;
        // water map
        float[,] w = new float[size, size];
        // sediment map
        float[,] m = new float[size, size];
        for (int iteration = 0; iteration < _iterations; iteration++)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    // add rain
                    w[i, j] += Kr;
                    
                    // move solid ground to sediment
                    h[i, j] -= Ks * w[i, j];
                    m[i, j] += Ks * w[i, j];
                    // return excess to heightmap
                    // if (M[i, j] > Kc)
                    // {
                    //     float excess = Kc - M[i, j];
                    //     M[i, j] -= excess;
                    //     heightMap[i, j] += excess;
                    // }
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
                    
                    float a = h[i, j] + w[i, j];
                    
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
                            float ai = h[i + x, j + y] + w[i + x, j + y];
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
                            float ai = h[i + x, j + y] + w[i + x, j + y];
                            float di = a - ai;

                            if (dTotal != 0)
                            {
                                // the amount of water moved to neighbour
                                float deltaW = Mathf.Min(w[i, j], deltaA) * di / dTotal;

                                if (w[i, j] != 0)
                                {
                                    // transport sediment to neighbouring cells
                                    m[i + x, j + y] += m[i, j] * (deltaW / w[i, j]);
                                    m[i, j] -= m[i, j] * (deltaW / w[i, j]);
                                }
                            }
                        }
                    }

                    w[i, j] *= (1 - Ke);
                    float mMax = Kc * w[i, j];
                    float prevM = m[i, j];
                    float deltaM = Mathf.Max(0, m[i, j] - mMax);
                    // if (float.IsNaN(deltaM) || float.IsInfinity(deltaM))
                    // {
                    //     deltaM = 0;
                    // }
                    m[i, j] -= deltaM;
                    float prev = h[i, j];
                    h[i, j] += deltaM;
                    if (float.IsNaN(h[i, j]) || float.IsInfinity(h[i, j]) || h[i, j] > maxHeight)
                    {
                        h[i, j] = heightMap[i, j];
                        //Debug.Log("bruh " + i + ", " + j + ", prev: " + prev + ", " + prevM);
                    }
                }
            }
        }
        
        return h;
    }
}
