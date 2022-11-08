using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherySettings : MonoBehaviour
{
   
    AudioManager audioManager;
    void Awake()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }
    public void StopSpaceMusic()
    {
        audioManager.Stop("laserMusic");
    }

    public void PlaySpaceMusic()
    {
       audioManager.StopAllBGMusic();
       audioManager.Play("laserMusic"); 
    }

}
