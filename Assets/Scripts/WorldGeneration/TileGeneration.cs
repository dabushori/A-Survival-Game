using UnityEngine;

/*
 * Generation of a specific tile given its position in the world
 */
public class TileGeneration : MonoBehaviour
{
    // The MeshRenderer of the current tile
    [SerializeField]
    MeshRenderer tileRenderer;

    // The MeshFilter of the current tile
    [SerializeField]
    MeshFilter meshFilter;

    // The MeshColliderthe current tile
    [SerializeField]
    MeshCollider meshCollider;

    // The height multiplier (noise values are between 0 to 1, the height multiplier sets the maximum height possible)
    [SerializeField]
    float heightMultiplier;

    // The color map to color different heights
    [SerializeField]
    Texture2D colorMap;

    // Get the color given the height using the color map
    private Color GetColor(float height)
    {
        // Move the height to [0,50] range
        int h = Mathf.RoundToInt(height * 50f);
        // Choose the corresponding pixel from the biome texture
        return colorMap.GetPixel(h, 50 - h);
    }

    /*
     * Color the tile by the heights using the color map (yellow-brown for lower places and green for higher places)
     */
    private Texture2D BuildTexture(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);
        Color[] colorMap = new Color[tileDepth * tileWidth];
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                // Transform the 2D map index is an Array index
                int colorIndex = zIndex * tileWidth + xIndex;
                float height = heightMap[zIndex, xIndex];
                // Assign the color according to the height
                colorMap[colorIndex] = GetColor(height);
            }
        }
        // Create a new texture and set its pixel colors
        Texture2D tileTexture = new Texture2D(tileWidth, tileDepth)
        {
            wrapMode = TextureWrapMode.Clamp
        };
        tileTexture.SetPixels(colorMap);
        tileTexture.Apply();
        return tileTexture;
    }

    /*
     * Update the current tile's mesh vertices to the given heights
     */
    void UpdateMeshVertices(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);
        Vector3[] meshVertices = meshFilter.mesh.vertices;
        
        int vertexIndex = 0;
        // Iterate through all the heightMap coordinates, updating the vertex index
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                // vertexIndex = zIndex * tileDepth + xIndex
                // A little optimization is to increment it by 1 in every iteration rether than calculating it
                
                float height = heightMap[zIndex, xIndex];
                Vector3 vertex = meshVertices[vertexIndex];
                
                // Change the vertex Y coordinate, proportional to the height value
                meshVertices[vertexIndex++] = new Vector3(vertex.x, height, vertex.z);
            }
        }
        // Update the vertices in the mesh and update its properties
        meshFilter.mesh.vertices = meshVertices;
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();
        // Update the mesh collider
        meshCollider.sharedMesh = meshFilter.mesh;
    }

    /*
     * Generate the current tile
     */
    public void GenerateTile(int mapScale, NoiseMapGeneration.Wave[] waves, int worldSeed)
    {
        // Calculate tile depth and width based on the mesh vertices
        Vector3[] meshVertices = meshFilter.mesh.vertices;
        int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);
        int tileWidth = tileDepth;
        
        // Calculate the offsets based on the tile position
        float offsetX = -gameObject.transform.position.x;
        float offsetZ = -gameObject.transform.position.z;

        // Generate a heightMap using the NoiseMapGeneration class and the offsets
        float[,] heightMap = NoiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, mapScale, offsetX, offsetZ, waves, worldSeed);

        // Build a Texture2D from the height map
        tileRenderer.material.mainTexture = BuildTexture(heightMap);

        // Set the heights of the vertices of the tiles
        for (int i = 0; i < heightMap.GetLength(0); ++i)
        {
            for (int j = 0; j < heightMap.GetLength(1); ++j)
            {
                heightMap[i, j] *= heightMultiplier;
            }
        }

        // Update the tile mesh vertices according to the height map
        UpdateMeshVertices(heightMap);
    }
}
