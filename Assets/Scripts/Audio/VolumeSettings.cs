using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
   [SerializeField] private AudioMixer audioMixer;
   [SerializeField] private Slider mainSlider;
   [SerializeField] private Slider sfxSlider;
   [SerializeField] private Slider musicSlider;


    public void SetMasterVolume()
    {
        float volume = mainSlider.value;
        audioMixer.SetFloat("MasterVol", Mathf.Log10(volume)*20);
    }
    public void SetSfxVolume()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat("SfxVol", Mathf.Log10(volume)*20);
    }
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("MusicVol", Mathf.Log10(volume)*20);
    }
}
