using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AmbienceTrigger : MonoBehaviour
{
    
    public string ambTag;
    public AudioSource ambSource;
    public AudioClip[] ambClips;

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
            PlayClip(0);
            Debug.Log("Se supone que " + ambTag);
         }  
          if (other.tag == "Player" && ambTag == "Snow")
         {
            PlayClip(1);
            Debug.Log("Se supone que " + ambTag);
         } 
          if (other.tag == "Player" && ambTag == "House")
         {
            PlayClip(2);
            Debug.Log("Se supone que " + ambTag);
         }  
          else if (other.tag == "Player" && ambTag == "Water")
         {
            PlayClip(3);
            Debug.Log("Se supone que " + ambTag);
         }  
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.tag =="Player")
        {
            StopClip();
            Debug.Log("Se supone que no mas " + ambTag);
        }    
    }

    void PlayClip (int index)
    {
        AudioClip selectedAmb = ambClips[index];
        ambSource.clip = selectedAmb;
        ambSource.Play();
    }
    
    void StopClip()
    {
        ambSource.Stop(); 
    }
}
