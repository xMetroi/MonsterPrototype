using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LoadNewScene(int num)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(num);
    }

    IEnumerator SkipIntro()
    {
        yield return new WaitForSeconds(33);
        LoadNewScene(2);
    }

    private void Start() 
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            StartCoroutine(SkipIntro());
        }    
    }
}
