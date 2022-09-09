using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    AudioManager audioManager;
    void Awake()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        musicSlider.onValueChanged.AddListener(audioManager.MusicVolume);
        sfxSlider.onValueChanged.AddListener(audioManager.SfxVolume);
    }

}
