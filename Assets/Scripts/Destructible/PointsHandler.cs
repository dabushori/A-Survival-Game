using UnityEngine;

/*
 * A class to handle damage view logic
 */
public class PointsHandler : MonoBehaviour
{
    /*
     * A static function to (locally) create a floating point message in the given position containing the given text
     */
    public static void CreateFloatingPoints(GameObject floatingPointsPrefab, Vector3 position, string text)
    {
        TextMesh textMesh = Instantiate(floatingPointsPrefab, position, Quaternion.identity).GetComponent<TextMesh>();
        textMesh.text = text;
    }

    /*
     * Destory the message after 1 second
     */
    void Start()
    {
        Destroy(gameObject, 1f);
    }

    /*
     * Make the message floating by lifting it in a constant speed (5 unity units per second)
     */
    private void Update()
    {
        transform.position += Vector3.up * Time.deltaTime * 5f;
    }

    /*
     * Make the points always face the camera
     */
    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }
}
