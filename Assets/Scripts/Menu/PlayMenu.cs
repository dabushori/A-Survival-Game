using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayMenu : MonoBehaviour
{
    int seed;
    bool isRNG;
    public void SetSeed(string seed)
    {
        if (!int.TryParse(seed, out int num))
        {
            this.seed = 0;
        } else
        {
            this .seed = num;
        }
    }

    public void IsRNG(bool rng)
    {
        isRNG = rng;
    }
    public void PlayGame()
    {
        if (isRNG)
        {
            seed = Random.Range(-1000000, 1000000);
        }
        GameStateController.Seed = seed;
        // load the next scene in the scene loader
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
