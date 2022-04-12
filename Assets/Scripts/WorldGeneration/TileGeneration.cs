using UnityEngine;

public class TileGeneration : MonoBehaviour
{

    [SerializeField]
    private MeshRenderer tileRenderer;
    
    [SerializeField]
    private MeshFilter meshFilter;
    
    [SerializeField]
    private MeshCollider meshCollider;
    
    [SerializeField]
    private float mapScale;

    public int worldSeed;

    [SerializeField]
    private float heightMultiplier;

    [SerializeField]
    private Texture2D colorMap;

    public class TileData
    {
        public float[,] heightMap;
        public Mesh mesh;
        public TileData(float[,] heightMap, Mesh mesh)
        {
            this.heightMap = heightMap;
            this.mesh = mesh;
        }
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
                meshVertices[vertexIndex] = new Vector3(vertex.x, height, vertex.z);
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

    public TileData GenerateTile(int mapScale, NoiseMapGeneration.Wave[] waves)
    {
        this.mapScale = mapScale;
        // calculate tile depth and width based on the mesh vertices
        Vector3[] meshVertices = meshFilter.mesh.vertices;
        int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);
        int tileWidth = tileDepth;
        // calculate the offsets based on the tile position
        float offsetX = -gameObject.transform.position.x;
        float offsetZ = -gameObject.transform.position.z;
        // generate a heightMap using noise
        float[,] heightMap = NoiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, mapScale, offsetX, offsetZ, waves, worldSeed);

        // build a Texture2D from the height map
        tileRenderer.material.mainTexture = BuildTexture(heightMap);

        for (int i = 0; i < heightMap.GetLength(0); ++i)
        {
            for (int j = 0; j < heightMap.GetLength(1); ++j)
            {
                heightMap[i, j] *= heightMultiplier;
            }
        }

        // update the tile mesh vertices according to the height map
        UpdateMeshVertices(heightMap);

        return new TileData(heightMap, meshFilter.mesh);
    }
}
