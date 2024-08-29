using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Properties")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Fade In/Out Properties")]

    public float fadeInDuration = 1f;
    public float fadeOutDuration = 1f;
    private bool isFading = false;
    private float targetVolume = 0f;
    private float currentVolume = 0f;
    private float fadeTime = 0f;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Play a audio clip in the Music Channel
    /// </summary>
    /// <param name="clip"></param>
    public void PlayMusicClip(AudioClip clip)
    {
        musicSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Play a audio clip in the SFX Channel
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySfxClip(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

     //Fade In/Out Behaviour 

    public void FadeIn()
    {
        if (!isFading && musicSource.volume == 0f)
        {
            isFading = true;
            targetVolume = 1f;
            fadeTime = 0f;
        }
    }

    public void FadeOut()
    {
        if (!isFading && musicSource.volume > 0f)
        {
            isFading = true;
            targetVolume = 0f;
            fadeTime = 0f;
        }
    }
    
    public void Update()
    {
        if (isFading)
        {
            fadeTime += Time.deltaTime;

            if(targetVolume == 0f)
            {
                currentVolume = Mathf.Lerp(currentVolume, targetVolume, fadeTime/fadeOutDuration);
            }
             else
            {
                currentVolume = Mathf.Lerp(currentVolume, targetVolume, fadeTime / fadeInDuration);
            }
            musicSource.volume = currentVolume;

            if (Mathf.Abs(currentVolume - targetVolume) < 0.01f)
            {
                isFading = false;
                musicSource.volume = targetVolume;
            }
        }
    }
}