using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Worley : MonoBehaviour
{
    public int _pointsCount;
    public int _pointDistance;

    private void OnValidate()
    {
        _pointDistance = Mathf.Clamp(_pointDistance, 1, _pointsCount);
    }

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
                //List<KeyValuePair<float, Vector2>> orderedPoints = new List<KeyValuePair<float, Vector2>>();
                List<float> distances = new List<float>();
        
                for (int k = 0; k < _pointsCount; k++)
                {
                    //orderedPoints.Add(points[i]);
            
                    float distance = Vector2.Distance(pixelPoint, points[k]);
                    distances.Add(distance);
                    //orderedPoints.Add(new KeyValuePair<float, Vector2>(distance, points[k]));
                }
                //orderedPoints = orderedPoints.OrderBy(x => x.Key).ToList();
                //map[i, j] = orderedPoints[_pointDistance].Key;
                map[i, j] = distances[_pointDistance];
            }
        }

        return map;
    }

    // gets the distance of the 'pointDistance' closest point.
    // eg. pointDistance = 2 gets 2nd closest point
    public float Noise(int seed, int size, int x, int y)
    {
        Random.InitState(seed);

        Vector2[] points = new Vector2[_pointsCount];

        for (int i = 0; i < _pointsCount; i++)
        {
            int xPoint = Random.Range(0, size);
            int yPoint = Random.Range(0, size);
            points[i] = new Vector2(xPoint, yPoint);
        }
        
        Vector2 pixelPoint = new Vector2(x, y);
        List<float> distances = new List<float>();
        
        for (int i = 0; i < _pointsCount; i++)
        {
            float distance = Vector2.Distance(pixelPoint, points[i]);
            distances.Add(distance);
        }

        return distances[_pointDistance];
    }
}
