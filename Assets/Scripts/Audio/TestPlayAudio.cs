using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayAudio : MonoBehaviour
{
    // Start is called before the first frame update
    public void PlaySmallDinoAttack()
    {
        AudioManager.PlaySound(SoundType.BigDinoBasicAttack_1);
    }
    public void PlaySmallDinoHurt()
    {
       AudioManager.PlaySound(SoundType.SmallDinoHurt); 
    }
    public void PlayBigDinoAttack()
    {

    }
    public void PlayBigDinoHurt()
    {
        
    }
    public void FireMonsterAttack()
    {
        
    }
    public void FireMonsterHurt()
    {
        
    }
}
