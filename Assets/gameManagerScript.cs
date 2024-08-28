using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManagerScript : MonoBehaviour
{
    public GameObject pauseMenu;

    // Método para pausar el juego
    public void pauseButton()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    // Método para reanudar el juego
    public void playButton()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            pauseButton();
        }
    }
}
