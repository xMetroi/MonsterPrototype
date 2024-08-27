using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Properties")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

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
}