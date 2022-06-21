using UnityEngine;

/*
 * A class to handle particles logic
 */
public class BreakParticles : MonoBehaviour
{
    /*
     * Create (locally) the given particles in the given position
     */
    public static void CreateBreakParticles(GameObject BreakParticlesPrefab, Vector3 position, Transform transform)
    {
        ParticleSystem ps = Instantiate(BreakParticlesPrefab, position, Quaternion.identity, transform).GetComponent<ParticleSystem>();
        ps.Play();
    }

    /*
     * Destory the particles object after 2 seconds
     */
    void Start()
    {
        Destroy(gameObject, 2f);
    }
}