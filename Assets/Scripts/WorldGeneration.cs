using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField]
    private int mapWidthInTiles;
    [SerializeField]
    private int mapDepthInTiles;
    [SerializeField]
    private int mapScale = 5;

    [SerializeField]
    private GameObject tilePrefab;

    public int worldSeed;

    [SerializeField]
    private NoiseMapGeneration.Wave[] waves;

    [SerializeField]
    private GameObject[] treePrefabs;

    [SerializeField]
    private GameObject[] rockPrefabs;

    public class TileCoordinate
    {
        public int tileZIndex;
        public int tileXIndex;
        public int coordinateZIndex;
        public int coordinateXIndex;
        public TileCoordinate(int tileZIndex, int tileXIndex, int coordinateZIndex, int coordinateXIndex)
        {
            this.tileZIndex = tileZIndex;
            this.tileXIndex = tileXIndex;
            this.coordinateZIndex = coordinateZIndex;
            this.coordinateXIndex = coordinateXIndex;
        }
    }

    public class WorldData
    {
        private int tileDepthInVertices, tileWidthInVertices;
        public TileGeneration.TileData[,] tilesData;
        public WorldData(int tileDepthInVertices, int tileWidthInVertices, int mapDepthInTiles, int mapWidthInTiles)
        {
            // build the tilesData matrix based on the level depth and width
            tilesData = new TileGeneration.TileData[tileDepthInVertices * mapDepthInTiles, tileWidthInVertices * mapWidthInTiles];
            this.tileDepthInVertices = tileDepthInVertices;
            this.tileWidthInVertices = tileWidthInVertices;
        }
        public void AddTileData(TileGeneration.TileData tileData, int tileZIndex, int tileXIndex)
        {
            // save the TileData in the corresponding coordinate
            tilesData[tileZIndex, tileXIndex] = tileData;
        }

        public TileCoordinate ConvertToTileCoordinate(int zIndex, int xIndex)
        {
            // the tile index is calculated by dividing the index by the number of tiles in that axis
            int tileZIndex = (int)Mathf.Floor((float)zIndex / (float)tileDepthInVertices);
            int tileXIndex = (int)Mathf.Floor((float)xIndex / (float)tileWidthInVertices);
            // the coordinate index is calculated by getting the remainder of the division above
            // we also need to translate the origin to the bottom left corner
            int coordinateZIndex = tileDepthInVertices - (zIndex % tileDepthInVertices) - 1;
            int coordinateXIndex = tileWidthInVertices - (xIndex % tileDepthInVertices) - 1;
            TileCoordinate tileCoordinate = new TileCoordinate(tileZIndex, tileXIndex, coordinateZIndex, coordinateXIndex);
            return tileCoordinate;
        }
    }

    [SerializeField]
    private WorldData worldData; // the world data (all its tiles)
    void GenerateWorld()
    {
        // calculate the number of vertices of the tile in each axis using its mesh
        Vector3[] tileMeshVertices = tilePrefab.GetComponent<MeshFilter>().sharedMesh.vertices;
        int tileDepthInVertices = (int)Mathf.Sqrt(tileMeshVertices.Length);
        int tileWidthInVertices = tileDepthInVertices;
        // creating a new world data object
        worldData = new WorldData(tileDepthInVertices, tileWidthInVertices, mapDepthInTiles, mapWidthInTiles);

        // get the tile dimensions from the tile Prefab
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
        int tileWidth = (int)tileSize.x;
        int tileDepth = (int)tileSize.z;
        // for each Tile, instantiate a Tile in the correct position
        for (int xTileIndex = 0; xTileIndex < mapWidthInTiles; xTileIndex++)
        {
            for (int zTileIndex = 0; zTileIndex < mapDepthInTiles; zTileIndex++)
            {
                // calculate the tile position based on the X and Z indices
                Vector3 tilePosition = new Vector3(gameObject.transform.position.x + xTileIndex * tileWidth,
                  gameObject.transform.position.y,
                  gameObject.transform.position.z + zTileIndex * tileDepth);
                // instantiate a new Tile
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                // update the world seed
                tile.GetComponent<TileGeneration>().worldSeed = worldSeed;
                // generate the tile considering the world seed
                worldData.AddTileData(tile.GetComponent<TileGeneration>().GenerateTile(mapScale, waves), zTileIndex, xTileIndex);
            }
        }

        float distanceBetweenVertices = (float)tileDepth / (float)tileDepthInVertices;
        GenerateTrees(tileDepthInVertices * mapDepthInTiles, tileWidthInVertices * mapWidthInTiles, mapScale, distanceBetweenVertices);
        GenerateRocks(tileDepthInVertices * mapDepthInTiles, tileWidthInVertices * mapWidthInTiles, mapScale, distanceBetweenVertices);
    }

    /* void GenerateRandomObjects(float distanceBetweenVertices)
     {
         System.Random random = new System.Random(worldSeed);
         for (int zIndex = 0; zIndex < mapDepthInTiles; zIndex++)
         {
             for (int xIndex = 0; xIndex < mapWidthInTiles; xIndex++)
             {
                 TileCoordinate tileCoordinate = worldData.ConvertToTileCoordinate(zIndex, xIndex);
                 TileGeneration.TileData tileData = worldData.tilesData[tileCoordinate.tileZIndex, tileCoordinate.tileXIndex];
                 int tileWidth = tileData.heightMap.GetLength(1);

                 Vector3[] meshVertices = tileData.mesh.vertices;
                 int vertexIndex = tileCoordinate.coordinateZIndex * tileWidth + tileCoordinate.coordinateXIndex;

                 Vector3 treePosition = new Vector3(xIndex * distanceBetweenVertices, meshVertices[vertexIndex].y, zIndex * distanceBetweenVertices);
                 GameObject tree = Instantiate(treePrefabs[random.Next(0, treePrefabs.Length)], treePosition, Quaternion.identity) as GameObject;
                 tree.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
             }
         }

     }

     void GenerateObjects()
     {
         foreach (GameObject mo in mapObjects)
         {
             GenerateRandomObjects(mo);
         }
     }*/

    System.Random random;

    public void GenerateTrees(int levelDepth, int levelWidth, float levelScale, float distanceBetweenVertices, int neighborRadius = 2)
    {
        // generate a tree noise map using Perlin Noise
        float[,] treeMap = NoiseMapGeneration.GenerateNoiseMap(levelDepth, levelWidth, levelScale, 0, 0, this.waves, random.Next(0, 1000000));
        for (int zIndex = 0; zIndex < levelDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < levelWidth; xIndex++)
            {
                // convert from Level Coordinate System to Tile Coordinate System and retrieve the corresponding TileData
                TileCoordinate tileCoordinate = worldData.ConvertToTileCoordinate(zIndex, xIndex);
                TileGeneration.TileData tileData = worldData.tilesData[tileCoordinate.tileZIndex, tileCoordinate.tileXIndex];

                float treeValue = treeMap[zIndex, xIndex];
                // compares the current tree noise value to the neighbor ones
                int neighborZBegin = (int)Mathf.Max(0, zIndex - neighborRadius);
                int neighborZEnd = (int)Mathf.Min(levelDepth - 1, zIndex + neighborRadius);
                int neighborXBegin = (int)Mathf.Max(0, xIndex - neighborRadius);
                int neighborXEnd = (int)Mathf.Min(levelWidth - 1, xIndex + neighborRadius);
                float maxValue = 0f;
                for (int neighborZ = neighborZBegin; neighborZ <= neighborZEnd; neighborZ++)
                {
                    for (int neighborX = neighborXBegin; neighborX <= neighborXEnd; neighborX++)
                    {
                        float neighborValue = treeMap[neighborZ, neighborX];
                        // saves the maximum tree noise value in the radius
                        if (neighborValue >= maxValue)
                        {
                            maxValue = neighborValue;
                        }
                    }
                }
                // if the current tree noise value is the maximum one, place a tree in this location
                if (treeValue == maxValue)
                {
                    Vector3 position = new Vector3(xIndex * distanceBetweenVertices, 1000, zIndex * distanceBetweenVertices); // finding the tree's future location using raycast

                    if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, float.MaxValue, (1 << 6))) // 6 is the surface layer, so here we will only find hits with the surface layer
                    {
                        GameObject tree = Instantiate(treePrefabs[random.Next(0, treePrefabs.Length)], hit.point, Quaternion.identity) as GameObject;
                        tree.transform.localScale = Vector3.one * 0.5f;
                    }
                }
            }
        }
    }

    public void GenerateRocks(int levelDepth, int levelWidth, float levelScale, float distanceBetweenVertices, int neighborRadius = 2)
    {
        // generate a rock noise map using Perlin Noise
        float[,] rocksMap = NoiseMapGeneration.GenerateNoiseMap(levelDepth, levelWidth, levelScale, 0, 0, this.waves, random.Next(0, 1000000));
        for (int zIndex = 0; zIndex < levelDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < levelWidth; xIndex++)
            {
                // convert from Level Coordinate System to Tile Coordinate System and retrieve the corresponding TileData
                TileCoordinate tileCoordinate = worldData.ConvertToTileCoordinate(zIndex, xIndex);
                TileGeneration.TileData tileData = worldData.tilesData[tileCoordinate.tileZIndex, tileCoordinate.tileXIndex];

                float rockValue = rocksMap[zIndex, xIndex];
                // compares the current rock noise value to the neighbor ones
                int neighborZBegin = (int)Mathf.Max(0, zIndex - neighborRadius);
                int neighborZEnd = (int)Mathf.Min(levelDepth - 1, zIndex + neighborRadius);
                int neighborXBegin = (int)Mathf.Max(0, xIndex - neighborRadius);
                int neighborXEnd = (int)Mathf.Min(levelWidth - 1, xIndex + neighborRadius);
                float maxValue = 0f;
                for (int neighborZ = neighborZBegin; neighborZ <= neighborZEnd; neighborZ++)
                {
                    for (int neighborX = neighborXBegin; neighborX <= neighborXEnd; neighborX++)
                    {
                        float neighborValue = rocksMap[neighborZ, neighborX];
                        // saves the maximum tree noise value in the radius
                        if (neighborValue >= maxValue)
                        {
                            maxValue = neighborValue;
                        }
                    }
                }
                // if the current rock noise value is the maximum one, place a rock in this location
                if (rockValue == maxValue)
                {
                    Vector3 position = new Vector3(xIndex * distanceBetweenVertices, 1000, zIndex * distanceBetweenVertices); // finding the rock's future location using raycast

                    if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, float.MaxValue, (1 << 6))) // 6 is the surface layer, so here we will only find hits with the surface layer
                    {
                        GameObject rock = Instantiate(rockPrefabs[random.Next(0, rockPrefabs.Length)], hit.point, Quaternion.identity) as GameObject;
                        rock.transform.localScale = Vector3.one * 0.5f;
                    }
                }
            }
        }
    }

    [SerializeField]
    private GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        worldSeed = GameStateController.Seed;
        worldSeed %= 1000000;
        random = new System.Random(worldSeed);
        GenerateWorld();
        // Instantiate(playerPrefab, new Vector3(0,100,0), Quaternion.identity);
    }
}
