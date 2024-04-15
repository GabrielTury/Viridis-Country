using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource audiosource;
    public AudioClip [] songs;
    public float volume;
    [SerializeField] private float trackTimer;
    [SerializeField] private float songsPlayed;
    [SerializeField] private bool [] beenPlayed;
    void Start()
    {
        audiosource = GetComponent<AudioSource>();

        beenPlayed = new bool[songs.Length];

        if (!audiosource.isPlaying)
        {
            ChangeSong(Random.Range(0, songs.Length));

        }

    }

    // Update is called once per frame
    void Update()
    {
        audiosource.volume = volume;

        if (audiosource.isPlaying)
        {
            trackTimer += 1 * Time.deltaTime;
        }

        if(!audiosource.isPlaying || trackTimer >= audiosource.clip.length) { 
            ChangeSong(Random.Range(0, songs.Length));

        }
        if(songsPlayed == songs.Length)
        {
            songsPlayed = 0;
            for(int i =0;i <songs.Length; i++)
            {
                if (i == songs.Length)
                    break;
                else
                    beenPlayed[i] = false;
            }
        }
    }

    public void ChangeSong(int songPicked)
    {
        if (!beenPlayed[songPicked])
        {
            trackTimer = 0;
            songsPlayed++;
            beenPlayed[songPicked] = true;
            audiosource.clip = songs[songPicked];
            audiosource.Play();
        }
       else
            audiosource.Stop();
    }
}
