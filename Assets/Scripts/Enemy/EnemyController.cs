using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    EnemyBrain references;
    Slider monsterSliderLife;

    private void Start()
    {
        references = GetComponent<EnemyBrain>();
        monsterSliderLife = GameObject.Find("SliderIAHP").GetComponent<Slider>();
        monsterSliderLife.maxValue = references.GetHP();
        monsterSliderLife.value = references.GetHP();
    }

    private void Update()
    {
        if (monsterSliderLife.value != references.GetHP())
        {
            monsterSliderLife.value = references.GetHP();
        }
    }
}
