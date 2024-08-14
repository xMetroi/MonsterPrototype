using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private bool isPaused = false; 
    public GameObject pauseMenuUI;

    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);  
    }

    public void Pause()
    {
        isPaused = !isPaused; 

        Time.timeScale = isPaused ? 0f : 1f; 
        pauseMenuUI.SetActive(isPaused); 
    }

}
