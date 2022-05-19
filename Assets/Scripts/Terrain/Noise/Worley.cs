using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Worley : MonoBehaviour
{
    public int _pointsCount;
    public int _pointDistance;

    public float scale;

    private void OnValidate()
    {
        _pointsCount = Mathf.Clamp(_pointsCount, 0, int.MaxValue); // pointscount cannot be less than 0
        _pointDistance = Mathf.Clamp(_pointDistance, 1, _pointsCount);
    }

    // gets the distance of the 'pointDistance' closest point.
    // eg. pointDistance = 2 gets 2nd closest point
    public float[,] GenerateHeightMap(int seed, int size)
    {
        Random.InitState(seed);

        Vector2[] points = new Vector2[_pointsCount];

        for (int i = 0; i < _pointsCount; i++)
        {
            int xPoint = Random.Range(0, size);
            int yPoint = Random.Range(0, size);
            points[i] = new Vector2(xPoint, yPoint);
        }
        
        float[,] map = new float[size, size];

        // loop for each pixel
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            { 
                Vector2 pixelPoint = new Vector2(i, j);
                List<float> distances = new List<float>();
        
                for (int k = 0; k < _pointsCount; k++)
                {
                    float distance = Vector2.Distance(pixelPoint, points[k]);
                    distances.Add(distance);
                }
                distances.Sort();
                float result = distances[_pointDistance - 1] * scale;
                map[i, j] = result;
            }
        }

        return map;
    }
}
