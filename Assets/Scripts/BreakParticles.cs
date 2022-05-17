using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakParticles : MonoBehaviour
{
    public static void CreateBreakParticles(GameObject BreakParticlesPrefab, Vector3 position, Transform mobTransform)
    {
        ParticleSystem ps = Instantiate(BreakParticlesPrefab, position, Quaternion.identity, mobTransform).GetComponent<ParticleSystem>();
        ps.Play();
    }

    void Start()
    {
        Destroy(gameObject, 2f);
    }
}