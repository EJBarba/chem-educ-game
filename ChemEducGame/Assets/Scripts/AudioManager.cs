using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start() {
        Play("bgmusic1");
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
        Sound s = Array.Find(sounds, sound => sound.name == "bgmusic1");
        // check for playerprefs

        s.source.volume = volume;
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
