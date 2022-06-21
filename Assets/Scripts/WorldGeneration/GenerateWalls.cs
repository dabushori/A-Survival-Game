using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateWalls : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    int tileWidth;
    void Awake()
    {
        transform.position = new Vector3(GameStateController.worldDepth / 2 - tileWidth / 2, 0, GameStateController.worldWidth / 2 - tileWidth / 2);
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
