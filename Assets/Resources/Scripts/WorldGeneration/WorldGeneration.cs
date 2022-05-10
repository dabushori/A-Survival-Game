using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class WorldGeneration : MonoBehaviourPunCallbacks
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
    }

    [SerializeField]
    private WorldData worldData; // the world data (all its tiles)
    Transform tilesParent;
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
                // GameObject tile = PhotonNetwork.Instantiate("Prefabs/World/Tile", tilePosition, Quaternion.identity) as GameObject;
                
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, tilesParent) as GameObject;
                // update the world seed
                tile.GetComponent<TileGeneration>().worldSeed = worldSeed;


                // generate the tile considering the world seed
                worldData.AddTileData(tile.GetComponent<TileGeneration>().GenerateTile(mapScale, waves), zTileIndex, xTileIndex);
                
                // GameStateController.WorldObjectsManagement.CreateObject(tile);
            }
        }

        gameObject.GetComponent<NavMeshSurface>().BuildNavMesh();

        float distanceBetweenVertices = (float)tileDepth / (float)tileDepthInVertices;
        if (PhotonNetwork.IsMasterClient) GenerateGameObjects(tileDepthInVertices * mapDepthInTiles, tileWidthInVertices * mapWidthInTiles, mapScale, distanceBetweenVertices);
        
        GameStateController.worldDepth = tileDepthInVertices * mapDepthInTiles;
        GameStateController.worldWidth = tileWidthInVertices * mapWidthInTiles;

    }

    public void GenerateObjects(int levelDepth, int levelWidth, float levelScale, float distanceBetweenVertices, int neighborRadius, string path, int numOfItemsInList)
    {
        // generate a noise map using Perlin Noise
        float[,] objectsMap = NoiseMapGeneration.GenerateNoiseMap(levelDepth, levelWidth, levelScale, 0, 0, this.waves, random.Next(0, 1000000));

        int iterationDelta = 2 * neighborRadius + 1;
        for (int zIndex = 0; zIndex < levelDepth; zIndex += iterationDelta)
        {
            for (int xIndex = 0; xIndex < levelWidth; xIndex += iterationDelta)
            {
                int neighborZBegin = (int)Mathf.Max(0, zIndex - neighborRadius);
                int neighborXBegin = (int)Mathf.Max(0, xIndex - neighborRadius);
                int neighborZEnd = (int)Mathf.Min(levelDepth - 1, zIndex + neighborRadius);
                int neighborXEnd = (int)Mathf.Min(levelWidth - 1, xIndex + neighborRadius);
                float maxValue = 0f;
                int maxZ = zIndex, maxX = xIndex;
                for (int neighborZ = neighborZBegin; neighborZ <= neighborZEnd; ++neighborZ)
                {
                    for (int neighborX = neighborXBegin; neighborX <= neighborXEnd; ++neighborX)
                    {
                        float neighborValue = objectsMap[neighborZ, neighborX];
                        // saves the maximum noise value in the radius
                        if (neighborValue >= maxValue)
                        {
                            maxZ = neighborZ;
                            maxX = neighborX;
                            maxValue = neighborValue;
                        }
                    }
                }

                Vector3 position = new Vector3(maxX * distanceBetweenVertices, 1000, maxZ * distanceBetweenVertices); // finding the object's future location using raycast

                if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, float.MaxValue, (1 << 6))) // 6 is the surface layer, so here we will only find hits with the surface layer
                {
                    GameObject obj = PhotonNetwork.InstantiateRoomObject(path + random.Next(0, numOfItemsInList), hit.point, Quaternion.identity);
                    obj.transform.localScale = Vector3.one * 0.5f;
                }
            }
        }
    }


    public void GenerateGameObjects(int levelDepth, int levelWidth, float levelScale, float distanceBetweenVertices)
    {
        GenerateObjects(levelDepth, levelWidth, levelScale, distanceBetweenVertices, 7, "World/Trees/", 12);
        GenerateObjects(levelDepth, levelWidth, levelScale, distanceBetweenVertices, 7, "World/Rocks/", 6);
        GenerateObjects(levelDepth, levelWidth, levelScale, distanceBetweenVertices, 10, "World/Iron/", 6);
        GenerateObjects(levelDepth, levelWidth, levelScale, distanceBetweenVertices, 18, "World/Gold/", 6);
        GenerateObjects(levelDepth, levelWidth, levelScale, distanceBetweenVertices, 30, "World/Diamond/", 6);
    }

    [SerializeField]
    GameObject playerPrefab;

    [PunRPC]
    public void SpawnPlayer()
    {
        PhotonNetwork.Instantiate("Prefabs/Player/PhotonPlayer", Vector3.zero, Quaternion.identity);
    }

    [SerializeField]
    GameObject worldObjectsManagementPrefab;

    System.Random random;
    void Start()
    {
        worldSeed = ((int)PhotonNetwork.CurrentRoom.CustomProperties["seed"]) % 1000000;
        random = new System.Random(worldSeed);
        Random.InitState(worldSeed);

        GameStateController.timeController = GetComponentInChildren<TimeController>();

        GenerateWorld();
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView.Get(this).RPC(nameof(SpawnPlayer), RpcTarget.All);
        }

    }

    void SyncWorld_RPC(object worldObjects, object tiles)
    {
        Instantiate((GameObject)worldObjects, transform);
        Instantiate((GameObject)tiles, transform);
    }
}
