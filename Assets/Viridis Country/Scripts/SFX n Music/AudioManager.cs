using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;
using GameEventSystem;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public Sound[] sounds;

    public enum ConstructionAudioTypes
    {
        Wood,
        Stone,
        Metal
    }

    void Awake ()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        foreach (Sound s in sounds){

            s.source = gameObject .AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

        }
    }

    public void Play (string name){

        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();

    }

    private void PlayWhenConstructionRemoved(ConstructionAudioTypes cType)
    {
        Sound demolish = null;
        foreach (Sound s in sounds)
        {
            if(s.name == "Demolish") 
                demolish = s;
        }
    }

    private void OnEnable()
    {
        GameEvents.Construction_Removed += PlayWhenConstructionRemoved;
    }

    private void OnDisable()
    {
        GameEvents.Construction_Removed -= PlayWhenConstructionRemoved;
    }

}
