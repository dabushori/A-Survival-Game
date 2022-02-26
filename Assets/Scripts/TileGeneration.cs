using UnityEngine;

public class TileGeneration : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public float seed;
        public float frequency;
        public float amplitude;
    }

    [SerializeField]
    private MeshRenderer tileRenderer;
    
    [SerializeField]
    private MeshFilter meshFilter;
    
    [SerializeField]
    private MeshCollider meshCollider;
    
    [SerializeField]
    private float mapScale;
    public float worldSeed;

    [SerializeField]
    private Wave[] waves;

    [SerializeField]
    private float heightMultiplier;

    [SerializeField]
    private Texture2D colorMap;

    public float[,] GenerateNoiseMap(int mapDepth, int mapWidth, float scale, float offsetX, float offsetZ, Wave[] waves)
    {
        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapDepth, mapWidth];
        for (int zIndex = 0; zIndex < mapDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < mapWidth; xIndex++)
            {
                // calculate sample indices based on the coordinates, the scale and the offset
                float sampleX = (xIndex + offsetX) / scale;
                float sampleZ = (zIndex + offsetZ) / scale;
                float noise = 0f;
                float normalization = 0f;
                foreach (Wave wave in waves)
                {
                    // generate noise value using PerlinNoise for a given Wave
                    noise += wave.amplitude * Mathf.PerlinNoise(sampleX * wave.frequency + wave.seed + worldSeed, sampleZ * wave.frequency + wave.seed + worldSeed);
                    normalization += wave.amplitude;
                }
                // normalize the noise value so that it is within 0 and 1
                noiseMap[zIndex, xIndex] = noise / normalization;
            }
        }
        return noiseMap;
    }

    private Color GetColor(float height)
    {
        // move the height to [0,50] range
        int h = Mathf.RoundToInt(height * 50f);
        // choose the corresponding pixel from the biome texture
        return colorMap.GetPixel(h, 50 - h);
    }

    private Texture2D BuildTexture(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);
        Color[] colorMap = new Color[tileDepth * tileWidth];
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                // transform the 2D map index is an Array index
                int colorIndex = zIndex * tileWidth + xIndex;
                float height = heightMap[zIndex, xIndex];
                // assign the color according to the terrain type
                colorMap[colorIndex] = GetColor(height);
            }
        }
        // create a new texture and set its pixel colors
        Texture2D tileTexture = new Texture2D(tileWidth, tileDepth);
        tileTexture.wrapMode = TextureWrapMode.Clamp;
        tileTexture.SetPixels(colorMap);
        tileTexture.Apply();
        return tileTexture;
    }

    private void UpdateMeshVertices(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);
        Vector3[] meshVertices = meshFilter.mesh.vertices;
        // iterate through all the heightMap coordinates, updating the vertex index
        int vertexIndex = 0;
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                float height = heightMap[zIndex, xIndex];
                Vector3 vertex = meshVertices[vertexIndex];
                // change the vertex Y coordinate, proportional to the height value
                meshVertices[vertexIndex] = new Vector3(vertex.x, height * heightMultiplier, vertex.z);
                vertexIndex++;
            }
        }
        // update the vertices in the mesh and update its properties
        meshFilter.mesh.vertices = meshVertices;
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();
        // update the mesh collider
        meshCollider.sharedMesh = meshFilter.mesh;
    }

    public void GenerateTile()
    {
        // calculate tile depth and width based on the mesh vertices
        Vector3[] meshVertices = meshFilter.mesh.vertices;
        int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);
        int tileWidth = tileDepth;
        // calculate the offsets based on the tile position
        float offsetX = -gameObject.transform.position.x;
        float offsetZ = -gameObject.transform.position.z;
        // generate a heightMap using noise
        float[,] heightMap = GenerateNoiseMap(tileDepth, tileWidth, mapScale, offsetX, offsetZ, waves);

        // build a Texture2D from the height map
        tileRenderer.material.mainTexture = BuildTexture(heightMap);

        // update the tile mesh vertices according to the height map
        UpdateMeshVertices(heightMap);
    }
}
