using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
                tile.transform.parent = gameObject.transform;
                // update the world seed
                tile.GetComponent<TileGeneration>().worldSeed = worldSeed;
                // generate the tile considering the world seed
                worldData.AddTileData(tile.GetComponent<TileGeneration>().GenerateTile(mapScale, waves), zTileIndex, xTileIndex);
            }
        }

        float distanceBetweenVertices = (float)tileDepth / (float)tileDepthInVertices;
        GenerateGameObjects(tileDepthInVertices * mapDepthInTiles, tileWidthInVertices * mapWidthInTiles, mapScale, distanceBetweenVertices);
        gameObject.GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    public void GenerateObjects(int levelDepth, int levelWidth, float levelScale, float distanceBetweenVertices, int neighborRadius, GameObject[] objectsList)
    {
        // generate a noise map using Perlin Noise
        float[,] objectsMap = NoiseMapGeneration.GenerateNoiseMap(levelDepth, levelWidth, levelScale, 0, 0, this.waves, random.Next(0, 1000000));
        for (int zIndex = 0; zIndex < levelDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < levelWidth; xIndex++)
            {
                // convert from Level Coordinate System to Tile Coordinate System and retrieve the corresponding TileData
                TileCoordinate tileCoordinate = worldData.ConvertToTileCoordinate(zIndex, xIndex);
                TileGeneration.TileData tileData = worldData.tilesData[tileCoordinate.tileZIndex, tileCoordinate.tileXIndex];

                float objectValue = objectsMap[zIndex, xIndex];
                // compares the current noise value to the neighbor ones
                int neighborZBegin = (int)Mathf.Max(0, zIndex - neighborRadius);
                int neighborZEnd = (int)Mathf.Min(levelDepth - 1, zIndex + neighborRadius);
                int neighborXBegin = (int)Mathf.Max(0, xIndex - neighborRadius);
                int neighborXEnd = (int)Mathf.Min(levelWidth - 1, xIndex + neighborRadius);
                float maxValue = 0f;
                for (int neighborZ = neighborZBegin; neighborZ <= neighborZEnd; neighborZ++)
                {
                    for (int neighborX = neighborXBegin; neighborX <= neighborXEnd; neighborX++)
                    {
                        float neighborValue = objectsMap[neighborZ, neighborX];
                        // saves the maximum noise value in the radius
                        if (neighborValue >= maxValue)
                        {
                            maxValue = neighborValue;
                        }
                    }
                }
                // if the current object noise value is the maximum one, place an object in this location
                if (objectValue == maxValue)
                {
                    Vector3 position = new Vector3(xIndex * distanceBetweenVertices, 1000, zIndex * distanceBetweenVertices); // finding the object's future location using raycast

                    if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, float.MaxValue, (1 << 6))) // 6 is the surface layer, so here we will only find hits with the surface layer
                    {
                        GameObject obj = Instantiate(objectsList[random.Next(0, objectsList.Length)], hit.point, Quaternion.identity);
                        obj.transform.localScale = Vector3.one * 0.5f;
                    }
                }
            }
        }
    }



    [SerializeField]
    private GameObject[] treePrefabs;

    [SerializeField]
    private GameObject[] rockPrefabs;

    [SerializeField]
    private GameObject[] ironPrefabs;

    [SerializeField]
    private GameObject[] goldPrefabs;

    [SerializeField]
    private GameObject[] diamondPrefabs;


    public void GenerateGameObjects(int levelDepth, int levelWidth, float levelScale, float distanceBetweenVertices)
    {
        GenerateObjects(levelDepth, levelWidth, levelScale, distanceBetweenVertices, 3, treePrefabs);
        GenerateObjects(levelDepth, levelWidth, levelScale, distanceBetweenVertices, 3, rockPrefabs);
        GenerateObjects(levelDepth, levelWidth, levelScale, distanceBetweenVertices, 10, ironPrefabs);
        GenerateObjects(levelDepth, levelWidth, levelScale, distanceBetweenVertices, 20, goldPrefabs);
        GenerateObjects(levelDepth, levelWidth, levelScale, distanceBetweenVertices, 30, diamondPrefabs);
    }

    System.Random random;
    void Start()
    {
        worldSeed = GameStateController.Seed % 1000000;
        random = new System.Random(worldSeed);
        GenerateWorld();
    }
}
