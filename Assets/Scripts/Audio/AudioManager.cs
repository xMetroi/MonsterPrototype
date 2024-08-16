using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum SoundType
{
 BigDinoBasicAttack_1,
 BigDinoBasicAttack_2,
 BigDinoSpecialAttack,
 BigDinoHurt,
 SmallDinoBasicAttack_1,
 SmallDinoBasicAttack_2,
 SmallDinoSpecialAttack,
 SmallDinoHurt,
 FireMonsterBasicAttack_1,
 FireMonsterBasicAttack_2,
 FireMonsterSpecialAttack,
 FireMonsterHurt,
 Dialogues

}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class AudioManager : MonoBehaviour
{
    [SerializeField] private SoundList [] soundList;
    private static AudioManager Instance;
    private AudioSource soundEffects;
    

    private void Awake() 
    {
       Instance = this;
    }

    private void Start() 
    {
        soundEffects = GetComponent<AudioSource>();    
    }
   
    public static void PlaySound(SoundType sound, float volume = 1)
    {
       AudioClip[] clips = Instance.soundList[(int)sound].Sounds;
       AudioClip randomClip = clips [UnityEngine.Random.Range(0, clips.Length)];
       Instance.soundEffects.PlayOneShot(randomClip, volume);
    }

    #if UNITY_EDITOR
    private void OnEnable() 
    {
        string [] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length);
        for (int i = 0; i < soundList.Length; i++)
        {
            soundList[i].name = names[i];
        }
    }
    #endif
}

[Serializable]
public struct SoundList 
{
    public AudioClip[] Sounds {get => sounds;}
    [HideInInspector] public string name;
    [SerializeField] private AudioClip [] sounds;
}