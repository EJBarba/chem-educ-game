using UnityEngine.Audio;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }

    public void MusicVolume (float volume)
    {
        string[] musicArray = { "bgmusic1", "bgmusicmainmenu", "bgmusicdefeat", "bgmusicvictory"};
         foreach (Sound sound in sounds)
         {
             foreach (var music in musicArray)
             {
                if (sound.name == music)
                {
                    sound.source.volume = volume;
                }
             }
         }
    }
     public void SfxVolume (float volume)
    {
         string[] sfxArray = { "wordcorrect", "wordwrong", "tilesfx"};
         foreach (Sound sound in sounds)
         {
             foreach (var sfx in sfxArray)
             {
                if (sound.name == sfx)
                {
                    sound.source.volume = volume;
                }
             }
         }

    }



}
