using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    AudioManager audioManager;
    void Awake()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }
    void Start()
    {
        audioManager.Stop("bgmusic1");
        audioManager.Play("bgmusicmainmenu");
    }

}
