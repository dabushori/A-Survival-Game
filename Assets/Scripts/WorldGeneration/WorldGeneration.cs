using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

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
    [SerializeField]
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
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, tilesParent) as GameObject;
                tile.transform.parent = gameObject.transform;
                // update the world seed
                tile.GetComponent<TileGeneration>().worldSeed = worldSeed;
                // generate the tile considering the world seed
                worldData.AddTileData(tile.GetComponent<TileGeneration>().GenerateTile(mapScale, waves), zTileIndex, xTileIndex);
            }
        }

        float distanceBetweenVertices = (float)tileDepth / (float)tileDepthInVertices;
        if (PhotonNetwork.IsMasterClient) GenerateGameObjects(tileDepthInVertices * mapDepthInTiles, tileWidthInVertices * mapWidthInTiles, mapScale, distanceBetweenVertices);
        
        GameStateController.worldDepth = tileDepthInVertices * mapDepthInTiles;
        GameStateController.worldWidth = tileWidthInVertices * mapWidthInTiles;

        gameObject.GetComponent<NavMeshSurface>().BuildNavMesh();
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
                    PhotonNetwork.Instantiate(path + random.Next(0, numOfItemsInList), hit.point, Quaternion.identity);
                }
            }
        }
    }


    public void GenerateGameObjects(int levelDepth, int levelWidth, float levelScale, float distanceBetweenVertices)
    {
        GenerateObjects(levelDepth, levelWidth, levelScale, distanceBetweenVertices, 7, GameStateController.treesPath, 12);
        GenerateObjects(levelDepth, levelWidth, levelScale, distanceBetweenVertices, 7, GameStateController.rocksPath, 6);
        GenerateObjects(levelDepth, levelWidth, levelScale, distanceBetweenVertices, 10, GameStateController.ironsPath, 6);
        GenerateObjects(levelDepth, levelWidth, levelScale, distanceBetweenVertices, 18, GameStateController.goldsPath, 6);
        GenerateObjects(levelDepth, levelWidth, levelScale, distanceBetweenVertices, 30, GameStateController.diamondsPath, 6);
    }

    public void GenerateTimeController()
    {
        // GameStateController.timeController = Instantiate(timeUnitPrefab).GetComponentInChildren<TimeController>();
        PhotonNetwork.Instantiate("Prefabs/World/TimeController", Vector3.zero, Quaternion.identity).GetComponent<TimeController>();
    }

    private GameObject player;
    public void SpawnPlayer()
    {
        // Instantiate(playerPrefab, new Vector3(GameStateController.worldDepth / 2, 5, GameStateController.worldWidth / 2), Quaternion.identity);
        Hashtable props = new Hashtable();
        Vector2 point = Random.insideUnitCircle * 50;
        player = PhotonNetwork.Instantiate("Prefabs/Player/FirstPersonPlayer", new Vector3(point.x + GameStateController.worldDepth / 2, 5, point.y + GameStateController.worldWidth / 2), Quaternion.identity) as GameObject;
        int viewID = player.GetComponent<PhotonView>().ViewID;
        props["local_player"] = viewID;
        PhotonNetwork.SetPlayerCustomProperties(props);
        //disable UI
        player.GetComponentInChildren<Canvas>().enabled = false;
        player.GetComponentInChildren<PlayerControls>().SpawnedPlayer();
    }

    public static WorldGeneration Instance;

    [SerializeField]
    TMP_Text playersText;
    [SerializeField]
    Canvas loadingScreen;
    public void SpawnedPlayer()
    {
        playersText.text = "Players Ready: " + (int)PhotonNetwork.CurrentRoom.CustomProperties["playersInGame"] + "/" + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        if ((int)PhotonNetwork.CurrentRoom.CustomProperties["playersInGame"] == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            player.GetComponentInChildren<PlayerControls>().ReadyPlayer();
        }
    }

    public void FadeScreen()
    {
        loadingScreen.GetComponentInChildren<Animation>().Play("FadeScreen");
        player.GetComponentInChildren<Canvas>().enabled = true;
        player.GetComponentInChildren<PlayerMovements>().isInLoadingScreen = false;
        GetComponentInChildren<AudioSource>().Play();
    }



    System.Random random;
    void Start()
    {
        Instance = this;
        worldSeed = ((int)PhotonNetwork.CurrentRoom.CustomProperties["seed"]) % 1000000;
        random = new System.Random(worldSeed);
        Random.InitState(worldSeed);

        if (PhotonNetwork.IsMasterClient) GenerateTimeController();
        GenerateWorld();

        SpawnPlayer();
    }
}
