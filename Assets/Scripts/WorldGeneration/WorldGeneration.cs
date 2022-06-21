using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

/*
 * A class responsible for generating the world in the beginning.
 * The world creation algorithm uses the TileGeneration class to generate the tiles (surface of the world),
 * and it also uses the NoiseMapGeneration to create noise maps for world objects generation (trees, rocks, ...)
 */
public class WorldGeneration : MonoBehaviour
{
    // Map dimenstions in units of tiles (50x50 by default)
    [SerializeField]
    int mapWidthInTiles, mapDepthInTiles;

    // Map scale (used for the noise map calculation, 5 by default)
    [SerializeField]
    int mapScale = 5;

    // The tile prefab (used to generate the world tiles)
    [SerializeField]
    GameObject tilePrefab;

    // The walls object, which contains the world's limits
    [SerializeField]
    GameObject Walls;

    // The world's seed
    [SerializeField]
    int worldSeed;

    // The waves that will be used to generate noise maps
    [SerializeField]
    NoiseMapGeneration.Wave[] waves;
    
    // The transform which the tiles will be instanciated as children of
    [SerializeField]
    Transform tilesParent;

    /*
     * Create the world's tiles (surface), objects (trees, rocks, ...) and walls (invisible limits)
     */
    void GenerateWorld()
    {
        // Calculate the number of vertices of the tile in each axis using its mesh
        // (It is a square of vertices so in order to get its dimenstions we need to take the square root of the number of vertices)
        int tileDepthInVertices = (int)Mathf.Sqrt(tilePrefab.GetComponent<MeshFilter>().sharedMesh.vertices.Length);
        
        // Get the tile dimensions from the tile Prefab
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
        int tileWidth = (int)tileSize.x;
        int tileDepth = (int)tileSize.z;

        // For each Tile, instantiate a Tile in the correct position
        for (int xTileIndex = 0; xTileIndex < mapWidthInTiles; xTileIndex++)
        {
            for (int zTileIndex = 0; zTileIndex < mapDepthInTiles; zTileIndex++)
            {
                // Calculate the tile position based on the X and Z indices
                Vector3 tilePosition = new Vector3(gameObject.transform.position.x + xTileIndex * tileWidth,
                  gameObject.transform.position.y,
                  gameObject.transform.position.z + zTileIndex * tileDepth);

                // Instantiate a new Tile
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, tilesParent);
                
                // Generate the tile considering the world seed
                tile.GetComponent<TileGeneration>().GenerateTile(mapScale, waves, worldSeed);
            }
        }

        // Save the world dimensions
        GameStateController.worldDepth = 10 * mapDepthInTiles;
        GameStateController.worldWidth = 10 * mapWidthInTiles;

        float distanceBetweenVertices = (float)tileDepth / (float)tileDepthInVertices;
        // If the player is the master client, generate the world objects
        if (PhotonNetwork.IsMasterClient) GenerateGameObjects(GameStateController.worldDepth, GameStateController.worldWidth, mapScale, distanceBetweenVertices);
        
        // Generate the world's limits
        Instantiate(Walls, Vector3.zero, Quaternion.identity, tilesParent);

        // Build the nav mesh (which will update the surface and objects data to future NavMeshAgents)
        gameObject.GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    /*
     * Populate the world with world objects from the given path using a noise map
     * The world objects' prefabs are <path>/0.prefab, <path>/1.prefab, ..., <path>/(numOfItemsInList - 1).prefab
     */
    public void GenerateObjects(int levelDepth, int levelWidth, float levelScale, float distanceBetweenVertices, int neighborRadius, string path, int numOfItemsInList)
    {
        // Generate a noise map using Perlin Noise
        float[,] objectsMap = NoiseMapGeneration.GenerateNoiseMap(levelDepth, levelWidth, levelScale, 0, 0, waves, random.Next(0, 1000000));

        int iterationDelta = 2 * neighborRadius + 1;
        // For each square of iterationDelta x iterationDelta (noise map entries), find the heighest object in the noise map and place there an object
        for (int zIndex = 0; zIndex < levelDepth; zIndex += iterationDelta)
        {
            for (int xIndex = 0; xIndex < levelWidth; xIndex += iterationDelta)
            {
                // Find the limits of the current sqaure
                int neighborZBegin = (int)Mathf.Max(0, zIndex - neighborRadius);
                int neighborXBegin = (int)Mathf.Max(0, xIndex - neighborRadius);
                int neighborZEnd = (int)Mathf.Min(levelDepth - 1, zIndex + neighborRadius);
                int neighborXEnd = (int)Mathf.Min(levelWidth - 1, xIndex + neighborRadius);

                // Find the maximum noise value in the current square
                float maxValue = 0f;
                int maxZ = zIndex, maxX = xIndex;
                for (int neighborZ = neighborZBegin; neighborZ <= neighborZEnd; ++neighborZ)
                {
                    for (int neighborX = neighborXBegin; neighborX <= neighborXEnd; ++neighborX)
                    {
                        float neighborValue = objectsMap[neighborZ, neighborX];
                        // Save the maximum noise value in the current range
                        if (neighborValue >= maxValue)
                        {
                            maxZ = neighborZ;
                            maxX = neighborX;
                            maxValue = neighborValue;
                        }
                    }
                }

                // Find the object's future location using raycast
                Vector3 position = new Vector3(maxX, 1000, maxZ);
                if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, float.MaxValue, GameStateController.surfaceLayer)) 
                {
                    PhotonNetwork.Instantiate(path + random.Next(0, numOfItemsInList), hit.point, Quaternion.identity);
                }
            }
        }
    }


    /*
     * Generate all the world objects 
     */
    public void GenerateGameObjects(int levelDepth, int levelWidth, float levelScale, float distanceBetweenVertices)
    {
        // Generate trees
        GenerateObjects(levelDepth, levelWidth, levelScale, distanceBetweenVertices, 7, GameStateController.treesPath, 12);

        // Generate rocks
        GenerateObjects(levelDepth, levelWidth, levelScale, distanceBetweenVertices, 7, GameStateController.rocksPath, 6);

        // Generate mushrooms
        GenerateObjects(levelDepth, levelWidth, levelScale, distanceBetweenVertices, 10, GameStateController.mushroomPath, 1);

        // Generate iron
        GenerateObjects(levelDepth, levelWidth, levelScale, distanceBetweenVertices, 10, GameStateController.ironsPath, 6);

        // Generate gold
        GenerateObjects(levelDepth, levelWidth, levelScale, distanceBetweenVertices, 18, GameStateController.goldsPath, 6);

        // Generate diamond
        GenerateObjects(levelDepth, levelWidth, levelScale, distanceBetweenVertices, 30, GameStateController.diamondsPath, 6);
    }

    /*
     * Instantiate the time controller
     */
    public void GenerateTimeController()
    {
        PhotonNetwork.Instantiate("Prefabs/World/TimeController", Vector3.zero, Quaternion.identity);
    }

    GameObject player; // the player

    /*
     * The function spawn the player and notify everyone that the player joined the game
     */
    public void SpawnPlayer()
    {
        // spawn the player
        player = PhotonNetwork.Instantiate("Prefabs/Player/FirstPersonPlayer", new Vector3(GameStateController.worldDepth / 2, 5, GameStateController.worldWidth / 2), Quaternion.identity) as GameObject;
        //disable UI because we want everyone to wait
        player.GetComponentInChildren<Canvas>().enabled = false;
        // call spawnedplayer (rpc) in player controls that sends rpcs to notify all that this player joined the game
        player.GetComponentInChildren<PlayerControls>().SpawnedPlayer();
    }

    public static WorldGeneration Instance; // singleton

    [SerializeField]
    TMP_Text playersText; // text for the amount of players
    [SerializeField]
    Canvas loadingScreen; // the loading screen

    /*
     * The function shows how many players are ready to their knowing
     */
    public void SpawnedPlayer()
    {
        // show ammount of ready players
        playersText.text = "Players Ready: " + (int)PhotonNetwork.CurrentRoom.CustomProperties["playersInGame"] + "/" + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        // if all the players in the room are in the game
        if ((int)PhotonNetwork.CurrentRoom.CustomProperties["playersInGame"] == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            // call readyplayer (rpc) that notify that everyone is ready
            player.GetComponentInChildren<PlayerControls>().ReadyPlayer();
        }
    }

    /*
     * the function calls the player
     */
    public void StartGame()
    {
        // fade the loading screen
        loadingScreen.GetComponentInChildren<Animation>().Play("FadeScreen");
        Invoke(nameof(RemoveScreen), 2f); // remove it
        // allow the player to move and interact with the UI
        player.GetComponentInChildren<Canvas>().enabled = true;
        player.GetComponentInChildren<PlayerMovements>().isInLoadingScreen = false;
        ShowDayCounter(); // show the day counter
        GetComponentInChildren<AudioSource>().Play(); // play the music
    }

    /*
     * A function to remove the loading screen
     */
    void RemoveScreen()
    {
        loadingScreen.GetComponentInChildren<Canvas>().enabled = false;
    }

    /*
     * A function to show the current day
     */
    void ShowDayCounter()
    {
        if (GameStateController.timeController == null)
        {
            Invoke(nameof(ShowDayCounter), 1f);
        } 
        else
        {
            GameStateController.timeController.ShowDay();
        }
    }

    System.Random random;
    void Start()
    {
        Instance = this;
        // Get the seed from the worlds' shared properties
        worldSeed = ((int)PhotonNetwork.CurrentRoom.CustomProperties["seed"]) % 1000000;
        random = new System.Random(worldSeed);
        Random.InitState(worldSeed);

        // Generate the time controller only at master client's execution
        if (PhotonNetwork.IsMasterClient) GenerateTimeController();
        
        // Generate the world
        GenerateWorld();

        // Spawn the player
        SpawnPlayer();
    }
}
