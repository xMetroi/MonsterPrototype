using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AmbienceTrigger : MonoBehaviour
{
    
    //Ambience Sounds
    public string ambTag;
    public AudioSource ambSource;
    public AudioClip[] ambClips;

    //Fade In/Out

    public float fadeInDuration = 1f;
    public float fadeOutDuration = 1f;
    private bool isFading = false;
    private float targetVolume = 0f;
    private float currentVolume = 0f;
    private float fadeTime = 0f;

    private void Start() 
    {
        // GameObject objectTag = GetComponent<GameObject>();
        ambTag = gameObject.tag;
        ambSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
         if (other.tag == "Player" && ambTag == "Forest")
         {
            FadeIn();
            PlayClip(0);
            Debug.Log("Se supone que " + ambTag);
         }  
          if (other.tag == "Player" && ambTag == "Snow")
         {
            PlayClip(1);
            FadeIn();
            Debug.Log("Se supone que " + ambTag);
         } 
          if (other.tag == "Player" && ambTag == "House")
         {
            PlayClip(2);
            FadeIn();
            Debug.Log("Se supone que " + ambTag);
         }  
          else if (other.tag == "Player" && ambTag == "Water")
         {
            FadeOut();
            StopClip();
            Debug.Log("Se supone que " + ambTag);
         }  
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.tag =="Player")
        {
            FadeOut();
            StopClip();
            Debug.Log("Se supone que no mas " + ambTag);
        }    
    }

    public void PlayClip (int index)
    {
        AudioClip selectedAmb = ambClips[index];
        ambSource.clip = selectedAmb;
        ambSource.Play();
    }
    
    public void StopClip()
    {
        if (currentVolume == 0)
        ambSource.Stop(); 
    }

    //Fade In/Out Behaviour 

    public void FadeIn()
    {
        if (!isFading && ambSource.volume == 0f)
        {
            isFading = true;
            targetVolume = 1f;
            fadeTime = 0f;
        }
    }

    public void FadeOut()
    {
        if (!isFading && ambSource.volume > 0f)
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
            ambSource.volume = currentVolume;

            if (Mathf.Abs(currentVolume - targetVolume) < 0.01f)
            {
                isFading = false;
                ambSource.volume = targetVolume;
            }
        }
    }

}
