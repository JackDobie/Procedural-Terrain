using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Perlin : MonoBehaviour
{
    int _mapSize;
    [SerializeField] int _octaves;
    [SerializeField] float _persistence;

    Vector2[,] _gradients;

    public void Init(int seed, int mapSize)
    {
        _mapSize = mapSize;
        _gradients = GenerateGridGradients(seed);
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

    Vector2[,] GenerateGridGradients(int seed)
    {
        System.Random rand = new System.Random(seed);
        _gradients = new Vector2[_mapSize, _mapSize];
        for (int i = 0; i < _mapSize; i++)
        {
            for (int j = 0; j < _mapSize; j++)
            {
                float x = (float)rand.NextDouble();
                float y = (float)rand.NextDouble();
                _gradients[i, j] = new Vector2(x, y);
            }
        }
        return _gradients;
    }

    public float Noise(float x, float y)
    {
        // find positions of each corner
        int x0 = Mathf.FloorToInt(x);
        int x1 = x0 + 1;
        int y0 = Mathf.FloorToInt(y);
        int y1 = y0 + 1;

        // find interpolation weights
        float sx = Fade(x - (float)x0);
        float sy = Fade(y - (float)y0);

        float n0 = DotGridGradient(x0, y0, x, y);
        float n1 = DotGridGradient(x1, y0, x, y);
        float ix0 = Interpolate(n0, n1, sx);
        n0 = DotGridGradient(x0, y1, x, y);
        n1 = DotGridGradient(x1, y1, x, y);
        float ix1 = Interpolate(n0, n1, sx);
        float result = Interpolate(ix0, ix1, sy);

        return result;
	}

    float DotGridGradient(int ix, int iy, float x, float y)
    {
        try
        {
            // get gradient from integer coordinates
            Vector2 gradient = _gradients[ix, iy];

            // find the offset vector
            float dx = x - (float)ix;
            float dy = y - (float)iy;

            // find the dot product
            return (dx * gradient.x + dy * gradient.y);
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
            Debug.LogError("ix: " + ix + ", iy: " + iy + " x: " + x + " y: " + y);
            return 0;
        }
    }

    float Interpolate(float a0, float a1, float w)
    {
        /* // You may want clamping by inserting:
         * if (0.0 > w) return a0;
         * if (1.0 < w) return a1;
         */
        if (0.0 > w) return a0;
        if (1.0 < w) return a1;
        return (a1 - a0) * w + a0;
        /* // Use this cubic interpolation [[Smoothstep]] instead, for a smooth appearance:
         * return (a1 - a0) * (3.0 - w * 2.0) * w * w + a0;
         *
         * // Use [[Smootherstep]] for an even smoother result with a second derivative equal to zero on boundaries:
         * return (a1 - a0) * ((w * (w * 6.0 - 15.0) + 10.0) * w * w * w) + a0;
         */
    }

    // Fade function defined by Ken Perlin. Eases coordinate values so that they will ease towards integral values. This smooths the final output
    float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }
}
