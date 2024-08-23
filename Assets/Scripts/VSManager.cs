using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VSManager : MonoBehaviour
{
    [SerializeField] private Image imagePlayer;
    [SerializeField] private Image imageEnemy;
    [SerializeField] private TextMeshProUGUI enemyName;

    public GameObject panelStartBattle;

    public void SetImagePlayer(SpriteRenderer image)
    {
        imagePlayer.sprite = image.sprite;
    }

    public void SetImageEnemy(SpriteRenderer image)
    {
        imageEnemy.sprite = image.sprite;
    }

    public void SetEnemyName(string name)
    {
        enemyName.text = name;
    }

    private void Start()
    {
        panelStartBattle.SetActive(false);
    }

    void Update()
    {
        if (GameManager.Instance.IsBattle())
        {
            StartCoroutine(StarAnimation());
        }
    }

    IEnumerator StarAnimation()
    {
        panelStartBattle.SetActive(true);
        yield return new WaitForSeconds(10f);
        panelStartBattle.SetActive(false);
    }

    public void DestroyPanel() 
    {
        Destroy(gameObject);
    }
}
