using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // load the next scene in the scene loader
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
