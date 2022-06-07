using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsHandler : MonoBehaviour
{
    public static void CreateFloatingPoints(GameObject floatingPointsPrefab, Vector3 position, string text)
    {
        TextMesh textMesh = Instantiate(floatingPointsPrefab, position, Quaternion.identity).GetComponent<TextMesh>();
        textMesh.text = text;
    }

    void Start()
    {
        Destroy(gameObject, 1f);
    }

    private void Update()
    {
        transform.position += Vector3.up * Time.deltaTime * 5f;
    }

    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }
}
