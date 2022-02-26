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

    /*
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
                // choose a terrain type according to the height value
                TerrainType terrainType = ChooseTerrainType(height);
                // assign the color according to the terrain type
                colorMap[colorIndex] = terrainType.color;
            }
        }
        // create a new texture and set its pixel colors
        Texture2D tileTexture = new Texture2D(tileWidth, tileDepth);
        tileTexture.wrapMode = TextureWrapMode.Clamp;
        tileTexture.SetPixels(colorMap);
        tileTexture.Apply();
        return tileTexture;
    }
    */

    private void UpdateMeshVertices(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);
        Vector3[] meshVertices = this.meshFilter.mesh.vertices;
        // iterate through all the heightMap coordinates, updating the vertex index
        int vertexIndex = 0;
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                float height = heightMap[zIndex, xIndex];
                Vector3 vertex = meshVertices[vertexIndex];
                // change the vertex Y coordinate, proportional to the height value
                meshVertices[vertexIndex] = new Vector3(vertex.x, height * this.heightMultiplier, vertex.z);
                vertexIndex++;
            }
        }
        // update the vertices in the mesh and update its properties
        this.meshFilter.mesh.vertices = meshVertices;
        this.meshFilter.mesh.RecalculateBounds();
        this.meshFilter.mesh.RecalculateNormals();
        // update the mesh collider
        this.meshCollider.sharedMesh = this.meshFilter.mesh;
    }

    public void GenerateTile()
    {
        // calculate tile depth and width based on the mesh vertices
        Vector3[] meshVertices = this.meshFilter.mesh.vertices;
        int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);
        int tileWidth = tileDepth;
        // calculate the offsets based on the tile position
        float offsetX = -this.gameObject.transform.position.x;
        float offsetZ = -this.gameObject.transform.position.z;
        // generate a heightMap using noise
        float[,] heightMap = this.GenerateNoiseMap(tileDepth, tileWidth, this.mapScale, offsetX, offsetZ, waves);

        // build a Texture2D from the height map
        // Texture2D tileTexture = BuildTexture(heightMap);
        // this.tileRenderer.material.mainTexture = tileTexture;

        // update the tile mesh vertices according to the height map
        UpdateMeshVertices(heightMap);
    }
}
