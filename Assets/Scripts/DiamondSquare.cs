using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class DiamondSquare : MonoBehaviour
{
    public float _offsetRange;
    
    public float[,] GenerateHeightMap(int seed, int size, float scale, Vector2 offset)
    {
        Random.InitState(seed);
        
        // ensure that the size is divisible by 2
        if (size % 2 != 0)
        {
            return new float[0, 0];
        }
        
        //todo: create island option by setting these to min height
        float[,] values = new float[size, size];
        values[0, 0] = Random.Range(0.0f, 1.0f); // top left
        values[0, size - 1] = Random.Range(0.0f, 1.0f); // top right
        values[size - 1, 0] = Random.Range(0.0f, 1.0f); // bottom left
        values[size - 1, size - 1] = Random.Range(0.0f, 1.0f); // bottom right
        
        // half sidelength while the length of the side is greater than 1
        for (int sideLength = size; sideLength > 1; sideLength /= 2)
        {
            int halfSide = sideLength / 2;
            
            // diamond step
            for (int i = 0; i + sideLength < size; i += sideLength)
            {
                for (int j = 0; j + sideLength < size; j += sideLength)
                {
                    // finds the average of the corners
                    float average = values[i, j]; // TL
                    average += values[i + sideLength - 1, j]; // TR
                    average += values[i, j + sideLength - 1]; // BL
                    average += values[i + sideLength - 1, j + sideLength - 1]; // BR
                    average *= 0.25f; // divide by 4
                    
                    // add a random offset to the centre of the points
                    average += Random.Range(0, _offsetRange);

                    //average = Mathf.Clamp(average, 0.0f, 1.0f);
                    
                    values[i + halfSide, j + halfSide] = average;
                }
            }
            
            // square step
            for (int i = 0; i < size; i += halfSide)
            {
                for (int j = (i + halfSide) % sideLength; j < size; j += sideLength)
                {
                    float average = values[(i - halfSide + size - 1) % (size), j];
                    average += values[(i + halfSide) % (size), j];
                    average += values[i, (j + halfSide) % (size)];
                    average += values[i, (j - halfSide + size - 1) % (size)];
                    average *= 0.25f;
                    
                    //add random offset
                    average += Random.Range(0, _offsetRange);

                    //average = Mathf.Clamp(average, 0.0f, 1.0f);
                    
                    values[i, j] = average;
                    
                    //todo: add edge case
                    if (i == 0)
                    {
                        values[size - 1, j] = average;
                    }

                    if (j == 0)
                    {
                        values[i, size - 1] = average;
                    }
                }
            }
            // half the random range
            //randRange -= randRange * 0.5f;
        }

        return values;
    }

    private float RandomRange(float min, float max, System.Random rand)
    {
        return min + (float)rand.NextDouble() * (max - min);
    }
}
