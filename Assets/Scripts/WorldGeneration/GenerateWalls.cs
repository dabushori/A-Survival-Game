using UnityEngine;

/*
 * Create the invisible world limits
 */
public class GenerateWalls : MonoBehaviour
{
    // The width (and height) of a tile
    [SerializeField]
    int tileWidth;

    // Generate the invisible world limits so players won't fall of the world
    void Awake()
    {
        // Move the (empty) object to the middle of the map
        transform.position = new Vector3((GameStateController.worldDepth - tileWidth) / 2, 0, (GameStateController.worldWidth - tileWidth) / 2);

        // Get the invisible walls objects and modify them to wrap the world
        BoxCollider[] walls = GetComponentsInChildren<BoxCollider>();

        walls[0].gameObject.transform.position = transform.position + new Vector3(0, 25, GameStateController.worldDepth / 2);
        walls[0].gameObject.transform.localScale = new Vector3(GameStateController.worldWidth, 50, 0);

        walls[1].gameObject.transform.position = transform.position + new Vector3(0, 25, -GameStateController.worldDepth / 2);
        walls[1].gameObject.transform.localScale = new Vector3(GameStateController.worldWidth, 50, 0);
 
        walls[2].gameObject.transform.position = transform.position + new Vector3(-GameStateController.worldWidth / 2, 25, 0);
        walls[2].gameObject.transform.localScale = new Vector3(GameStateController.worldDepth, 50, 0);

        walls[3].gameObject.transform.position = transform.position + new Vector3(GameStateController.worldWidth / 2, 25, 0);
        walls[3].gameObject.transform.localScale = new Vector3(GameStateController.worldDepth, 50, 0);

    }
}
