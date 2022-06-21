using UnityEngine;

/*
 * Easily generate a noise map using the given these parameters:
 * mapDepth and mapWidth - dimensions of the map
 * scale - the scale that is assigned to the noise function inputs
 * offsetX and offsetZ - offsets of the current noise map from the origin
 * waves - some waves to make the values interesting (in our algorithm this parameter is constant)
 * worldSeed - the seed of the world, used to make differences between different maps, but the same maps for the same worldSeed
 * 
 * The noise map sizes are [mapDepth, mapWidth] and its values are calculates as can be seen in lines 39-40
 */
public class NoiseMapGeneration
{
    [System.Serializable]
    public class Wave
    {
        public float seed;
        public float frequency;
        public float amplitude;
    }

    public static float[,] GenerateNoiseMap(int mapDepth, int mapWidth, float scale, float offsetX, float offsetZ, Wave[] waves, int worldSeed)
    {
        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapDepth, mapWidth];
        for (int zIndex = 0; zIndex < mapDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < mapWidth; xIndex++)
            {
                // Calculate sample indices based on the coordinates, the scale and the offset
                float sampleX = (xIndex + offsetX) / scale;
                float sampleZ = (zIndex + offsetZ) / scale;
                float noise = 0f;
                float normalization = 0f;
                foreach (Wave wave in waves)
                {
                    // Generate noise value using PerlinNoise for a given Wave
                    noise += wave.amplitude * Mathf.PerlinNoise(sampleX * wave.frequency + wave.seed + worldSeed, sampleZ * wave.frequency + wave.seed + worldSeed);
                    normalization += wave.amplitude;
                }
                // Normalize the noise value so that it is within 0 and 1
                noiseMap[zIndex, xIndex] = noise / normalization;
            }
        }
        return noiseMap;
    }
}
