using UnityEngine;
using Random = UnityEngine.Random;

public class DiamondSquare : MonoBehaviour
{
    public float _offsetRange;
    public float _smoothness;
    
    public float[,] GenerateHeightMap(int seed, int size, float scale, Vector2 offset)
    {
        // check that the size is a power of 2
        if ((size & (size - 1)) != 0)
        {
            return new float[0, 0];
        }
        
        float range = _offsetRange;
        
        // set seed for random number generator
        Random.InitState(seed);
        
        //todo: create island option by setting these to min height
        float[,] heightMap = new float[size, size];
        heightMap[0, 0] = Random.Range(0.0f, 1.0f); // top left
        heightMap[0, size - 1] = Random.Range(0.0f, 1.0f); // top right
        heightMap[size - 1, 0] = Random.Range(0.0f, 1.0f); // bottom left
        heightMap[size - 1, size - 1] = Random.Range(0.0f, 1.0f); // bottom right
        
        // half sidelength while the length of the side is greater than 1
        for (int sideLength = size; sideLength > 1; sideLength /= 2)
        {
            int halfSide = sideLength / 2;
            
            // square step
            for (int x = 0; x + sideLength < size; x += sideLength)
            {
                for (int y = 0; y + sideLength < size; y += sideLength)
                {
                    // finds the average of the corners
                    float average = heightMap[x, y]; // TL
                    average += heightMap[x + sideLength - 1, y]; // TR
                    average += heightMap[x, y + sideLength - 1]; // BL
                    average += heightMap[x + sideLength - 1, y + sideLength - 1]; // BR
                    average *= 0.25f; // divide by 4
                    
                    // add a random offset to the centre of the points
                    average += Random.Range(0, range);

                    //average = Mathf.Clamp(average, 0.0f, 1.0f);
                    
                    heightMap[x + halfSide, y + halfSide] = average;
                }
            }
            
            // diamond step
            for (int x = 0; x < size; x += halfSide)
            {
                for (int y = (x + halfSide) % sideLength; y < size; y += sideLength)
                {
                    float average = heightMap[(x - halfSide + size) % size, y];
                    average += heightMap[(x + halfSide) % size, y];
                    average += heightMap[x, (y + halfSide) % size];
                    average += heightMap[x, (y - halfSide + size) % size];
                    average *= 0.25f;
                    
                    //add random offset
                    average += Random.Range(0, range);

                    //average = Mathf.Clamp(average, 0.0f, 1.0f);
                    
                    heightMap[x, y] = average;
                    
                    //todo: add edge case
                    if (x == 0)
                    {
                        heightMap[size - 1, y] = average;
                    }

                    if (y == 0)
                    {
                        heightMap[x, size - 1] = average;
                    }
                }
            }
            // lower the random range
            range -= range * 0.5f * _smoothness;
        }

        return heightMap;
    }

    private float RandomRange(float min, float max, System.Random rand)
    {
        return min + (float)rand.NextDouble() * (max - min);
    }
}
