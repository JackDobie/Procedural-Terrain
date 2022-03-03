using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Perlin : MonoBehaviour
{
    int _mapSize;
    //int _seed;
    [SerializeField] float _scale;
    [SerializeField] int _octaves;
    [SerializeField] float _persistence;

    System.Random _rand;

    //public Perlin(int octaves, float persistence, float scale, int mapSize)
    //{
    //    _octaves = octaves;
    //    _persistence = persistence;
    //    _mapSize = mapSize;
    //    _scale = scale;
    //    //_seed = seed;
    //    //_rand = new System.Random(seed);
    //}

    public float[,] GenerateHeightMap(int seed, int mapSize)
    {
        _rand = new System.Random(seed);
        _mapSize = mapSize;

        Vector2[,] gradients = GenerateGridGradients();
        float[,] heightMap = new float[_mapSize, _mapSize];

        for (int i = 0; i < _mapSize; i++)
        {
            for(int j = 0; j < _mapSize; j++)
            {
                //float x = i / 10.0f;

                //if(x >= _chunkSize - 1)
                //{
                //    x = _chunkSize - 2.0f;
                //}
                //float y = j / 10.0f;
                //if(y >= _chunkSize - 1)
                //{
                //    y = _chunkSize - 2.0f;
                //}

                heightMap[i,j] = Mathf.Clamp(Noise(i * .3f, j * .3f, gradients), 0.0f, 1.0f);
            }
        }

        return heightMap;
    }

    Vector2[,] GenerateGridGradients()
    {
        Vector2[,] gradients = new Vector2[_mapSize + 1, _mapSize + 1];
        for (int i = 0; i < _mapSize; i++)
        {
            for (int j = 0; j < _mapSize; j++)
            {
                float x = (float)_rand.NextDouble();
                float y = (float)_rand.NextDouble();
                gradients[i, j] = new Vector2(x, y);
            }
        }
        return gradients;
    }

    float Noise(float x, float y, Vector2[,] gradients)
    {
        // find cell origin and corners
        //Vector2 cell = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
        //// corners to add to origin
        //Vector2[] corners = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };
        //for(int i = 0; i < corners.Length; i++)
        //{
        //    // find corner position from cell origin point
        //    Vector2 currentCorner = corners[i] + cell;
        //    // find vector from pos to currentcorner
        //    Vector2 offset = currentCorner - pos;
        //    // dot prod with gradient
        //    // interpolate
        //}

        // find positions of each corner
        int x0 = Mathf.FloorToInt(x);
        int x1 = x0 + 1;
        int y0 = Mathf.FloorToInt(y);
        int y1 = y0 + 1;

        // find interpolation weights
        float sx = x - (float)x0;
        float sy = y - (float)y0;

        float n0 = DotGridGradient(x0, y0, x, y, gradients);
        float n1 = DotGridGradient(x1, y0, x, y, gradients);
        float ix0 = Interpolate(n0, n1, sx);
        n0 = DotGridGradient(x0, y1, x, y, gradients);
        n1 = DotGridGradient(x1, y1, x, y, gradients);
        float ix1 = Interpolate(n0, n1, sx);
        float result = Interpolate(ix0, ix1, sy);

        return result;
	}

    float DotGridGradient(int ix, int iy, float x, float y, Vector2[,] gradients)
    {
        try
        {
            // get gradient from integer coordinates
            Vector2 gradient = gradients[ix, iy];

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
        float result = (a1 - a0) * w + a0;
        //if(result < 0.0f)
        //{
        //    return 0.0f;
        //}
        //if(result > 1.0f)
        //{
        //    return 1.0f;
        //}
        return result;
        /* // Use this cubic interpolation [[Smoothstep]] instead, for a smooth appearance:
         * return (a1 - a0) * (3.0 - w * 2.0) * w * w + a0;
         *
         * // Use [[Smootherstep]] for an even smoother result with a second derivative equal to zero on boundaries:
         * return (a1 - a0) * ((w * (w * 6.0 - 15.0) + 10.0) * w * w * w) + a0;
         */
    }

    float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }
}