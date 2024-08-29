using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISounds : MonoBehaviour
{
    public AudioSource uiAudio;
    public AudioClip [] menuAudios;
    public AudioClip textClip;
    void Start()
    {
        uiAudio = GetComponent<AudioSource>();
    }
    
    public void PlayMenuSFX()
    {
        uiAudio.clip = menuAudios[Random.Range(0, menuAudios.Length)];
        uiAudio.PlayOneShot(uiAudio.clip);
    }
    public void PlayTextClip()
    {
        uiAudio.PlayOneShot(textClip);
    }
}
