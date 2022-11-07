using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private AudioManager audioManager;
    private void Awake() {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }
    void Start()
    {
       audioManager.StopAllBGMusic();
        audioManager.Play("tutorialMusic");          
    }

    public void StopTutorialMusic()
    {
        audioManager.Stop("tutorialMusic");
    }
}
