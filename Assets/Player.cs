using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("enter");
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("exit");
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("col enter");
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("col exit");
    }
}
