using MyBox;
using UnityEngine;

public class Perlin : MonoBehaviour
{
    private int _mapSize = 0;
    public int _octaves;
    [ConditionalField(nameof(_octaves), true, 0)]
    public float _persistence;
    [Space]
    public Vector2 _offset;
    public float _scale;

    private Vector2[,] _gradients;
    
    private void OnValidate()
    {
        float maxSize = _mapSize > 0 ? (_mapSize - _scale - 1) : 0;
        // clamp offset to 0 and map size
        _offset.x = Mathf.Clamp(_offset.x, 0, maxSize);
        _offset.y = Mathf.Clamp(_offset.y, 0, maxSize);
    }
    
    public void Init(int seed, int mapSize)
    {
        _mapSize = mapSize;
        _gradients = GenerateGridGradients(seed);
    }

    public float[,] GenerateHeightMap()
    {
        float[,] heightMap = new float[_mapSize, _mapSize];
        
        //todo: add octaves
        for (int i = 0; i < _mapSize; i++)
        {
            for(int j = 0; j < _mapSize; j++)
            {
                float xCoord = (float)i / _mapSize * _scale + _offset.x;
                float yCoord = (float)j / _mapSize * _scale + _offset.y;
                
                if(_octaves > 0)
                {
                    heightMap[i, j] = OctaveNoise(xCoord, yCoord);
                }
                else
                {
                    heightMap[i, j] = Noise(xCoord, yCoord);
                }
            }
        }

        return heightMap;
    }

    private Vector2[,] GenerateGridGradients(int seed)
    {
        Random.InitState(seed);
        _gradients = new Vector2[_mapSize, _mapSize];
        for (int i = 0; i < _mapSize; i++)
        {
            for (int j = 0; j < _mapSize; j++)
            {
                float x = Random.value;
                float y = Random.value;
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
        float sx = Fade(x - x0);
        float sy = Fade(y - y0);

        float n0 = DotGridGradient(x0, y0, x, y);
        float n1 = DotGridGradient(x1, y0, x, y);
        float ix0 = Interpolate(n0, n1, sx);
        n0 = DotGridGradient(x0, y1, x, y);
        n1 = DotGridGradient(x1, y1, x, y);
        float ix1 = Interpolate(n0, n1, sx);
        float result = Interpolate(ix0, ix1, sy);

        return result;
	}

    public float OctaveNoise(float x, float y)
    {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;
        for (int i = 0; i < _octaves; i++)
        {
            total += Noise(x * frequency, y * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= _persistence;
            frequency *= 2;
        }

        if (maxValue == 0) return total;
        return total / maxValue;
    }

    private float DotGridGradient(int ix, int iy, float x, float y)
    {
        // get gradient from integer coordinates
        Vector2 gradient = _gradients[ix, iy];

        // find the offset vector
        float dx = x - ix;
        float dy = y - iy;

        // find the dot product
        return (dx * gradient.x + dy * gradient.y);
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
