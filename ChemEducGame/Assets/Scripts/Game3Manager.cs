using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game3Manager : MonoBehaviour
{
    private AudioManager audioManager;
    private void Awake() {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    void Start()
    {
        audioManager.StopAllBGMusic();
        audioManager.Play("bgGame3");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
