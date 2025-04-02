using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPause : MonoBehaviour
{
    public GameObject pauseImage;

    void Start()
    {
        if (pauseImage != null)
        {
            pauseImage.SetActive(false); 
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerMovement.isPaused = !PlayerMovement.isPaused;
            Time.timeScale = PlayerMovement.isPaused ? 0 : 1;

            pauseImage.SetActive(PlayerMovement.isPaused);

        }
    }
}
