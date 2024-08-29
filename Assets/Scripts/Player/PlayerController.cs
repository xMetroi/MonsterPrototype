using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    PlayerCombat references;
    Slider monsterSliderLife;

    private void Start()
    {
        /*references = GetComponent<PlayerCombat>();
        monsterSliderLife = GameObject.Find("SliderPlayerHP").GetComponent<Slider>();
        monsterSliderLife.maxValue = references.GetHP();
        monsterSliderLife.value = references.GetHP();*/
    }

    private void Update()
    {
        //if(monsterSliderLife.value != references.GetHP()) 
        //{
            //monsterSliderLife.value = references.GetHP();
        //}
    }

    /*public void ChangeHP()
    {
        monsterSliderLife.value = references.GetHP();
    }*/
}
