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
}
