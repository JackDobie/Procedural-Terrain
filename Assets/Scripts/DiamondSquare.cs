using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondSquare : MonoBehaviour
{
    private int _mapSize;
    
    public void Init(int seed, int mapSize)
    {
        _mapSize = mapSize;
    }
    
    public float[,] GenerateHeightMap(float scale, float maxHeight, Vector2 offset)
    {
        float[,] heightMap = new float[_mapSize, _mapSize];

        for (int i = 0; i < _mapSize; i++)
        {
            for(int j = 0; j < _mapSize; j++)
            {
                float xCoord = (float)i / _mapSize * scale + offset.x;
                float yCoord = (float)j / _mapSize * scale + offset.y;

                heightMap[i,j] = Noise(xCoord, yCoord) * maxHeight;
            }
        }

        return heightMap;
    }

    public float Noise(float x, float y)
    {
        return 0;
    }
}
