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
    [SerializeField] Animator animator;

    public GameObject panelStartBattle;
    public GameObject dialogueSystem;

    public void SetImagePlayer(SpriteRenderer image)
    {
        imagePlayer.sprite = image.sprite;
    }

    public void SetImageEnemy(SpriteRenderer image)
    {
        if (image != null && image.sprite != null)
        {
            imageEnemy.sprite = image.sprite;
        }
    }

    public void SetEnemyName(string name)
    {
        enemyName.text = name;
    }

    private void Start()
    {
        panelStartBattle.SetActive(false);
        //Event subscribtion
        GameManager.instance.BattleStarted += OnBattleStarted;
    }

    private void OnDestroy()
    {
        //Event desubscribtion
        GameManager.instance.BattleStarted -= OnBattleStarted;
    }

    public void OnBattleStarted(TrainerEnemyController trainerEnemyController)
    {
        StartCoroutine(StartAnimation());
    }

    IEnumerator StartAnimation()
    {
        dialogueSystem.transform.localScale = new Vector3(0, 1, 0);
        panelStartBattle.SetActive(true);

        // Configurar el Animator para que funcione en tiempo real
        if (animator != null)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime; // Animación en tiempo real
        }

        // Pausar el tiempo del juego
        Time.timeScale = 0;

        // Esperar en tiempo real para que la animación se ejecute completamente
        yield return new WaitForSecondsRealtime(3f);

        // Reanudar el tiempo del juego
        Time.timeScale = 1;
        panelStartBattle.SetActive(false);
    }



}
