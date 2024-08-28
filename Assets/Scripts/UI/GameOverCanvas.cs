using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverCanvas : MonoBehaviour
{
    [SerializeField] private GameObject gameOverHolder;
    [SerializeField] private GameObject winHolder;
    [SerializeField] private GameObject looseHolder;

    public void ShowWinHolder()
    {
        gameOverHolder.SetActive(true);
        winHolder.SetActive(true);
    }

    public void ShowLooseHolder()
    {
        gameOverHolder.SetActive(true);
        looseHolder.SetActive(true);
    }

}
