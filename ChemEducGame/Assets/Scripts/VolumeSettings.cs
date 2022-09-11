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
        musicSlider.value = PlayerPrefs.GetFloat("musicvolume", 1);
        sfxSlider.onValueChanged.AddListener(audioManager.SfxVolume);
        sfxSlider.value = PlayerPrefs.GetFloat("sfxvolume", 1);
    }

}
