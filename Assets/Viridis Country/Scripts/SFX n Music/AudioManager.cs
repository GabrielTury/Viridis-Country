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
        Metal,
        Construction_Removed
    }
    public enum SoundEffects
    {
        Click,
        Select,
        OneStar,
        TwoStar,
        ThreeStar
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
        Sound Construction_Removed = null;
        foreach (Sound s in sounds)
        {
            if(s.name == "Construction_Removed")
                Construction_Removed = s;
        }
        Construction_Removed.source.Play();
    }


    private void PlayWhenZoom(SoundEffects sfx)
    {
        Sound Planet_Zoom = null;
        foreach (Sound s in sounds)
        {
            if (s.name == "Planet_Zoom")
                Planet_Zoom = s;
        }
        Planet_Zoom.source.Play();
    }

    private void PlayWhenClick(SoundEffects sfx)
    {
        Sound Click = null;
        foreach (Sound s in sounds)
        {
            if (s.name == "Click")
                Click = s;
        }
        Click.source.Play();
    }

    private void PlayWhenSelect(SoundEffects sfx)
    {
        Sound Select = null;
        foreach (Sound s in sounds)
        {
            if (s.name == "Select")
                Select = s;
        }
        Select.source.Play();
    }

    private void PlayWhenOneStar(SoundEffects sfx)
    {
        Sound OneStar = null;
        foreach (Sound s in sounds)
        {
            if (s.name == "OneStar")
                OneStar = s;
        }
        OneStar.source.Play(); 
    }

    private void PlayWhenTwoStar(SoundEffects sfx)
    {
        Sound TwoStar = null;
        foreach (Sound s in sounds)
        {
            if (s.name == "TwoStar")
                TwoStar = s;
        }
        TwoStar.source.Play();
    }

    private void PlayWhenThreeStar(SoundEffects sfx)
    {
        Sound ThreeStar = null;
        foreach (Sound s in sounds)
        {
            if (s.name == "ThreeStar")
                ThreeStar = s;
        }
        ThreeStar.source.Play();
    }

    private void OnEnable()
    {
        GameEvents.Construction_Removed += PlayWhenConstructionRemoved;
        GameEvents.Click += PlayWhenClick;
        GameEvents.Select_Construction += PlayWhenSelect;
        GameEvents.OneStar += PlayWhenOneStar;
        GameEvents.TwoStar += PlayWhenTwoStar;
        GameEvents.ThreeStar += PlayWhenThreeStar;
        GameEvents.Planet_Zoom += PlayWhenZoom;

    }

    private void OnDisable()
    {
        GameEvents.Construction_Removed -= PlayWhenConstructionRemoved;
        GameEvents.Click -= PlayWhenClick;
        GameEvents.Select_Construction -= PlayWhenSelect;
        GameEvents.OneStar -= PlayWhenOneStar;
        GameEvents.TwoStar -= PlayWhenTwoStar;
        GameEvents.ThreeStar -= PlayWhenThreeStar;
        GameEvents.Planet_Zoom -= PlayWhenZoom;
    }

}
