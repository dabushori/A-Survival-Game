using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakParticles : MonoBehaviour
{
    public static void CreateBreakParticles(GameObject BreakParticlesPrefab, Vector3 position, Transform transform)
    {
        ParticleSystem ps = Instantiate(BreakParticlesPrefab, position, Quaternion.identity, transform).GetComponent<ParticleSystem>();
        ps.Play();
    }

    void Start()
    {
        Destroy(gameObject, 2f);
    }
}