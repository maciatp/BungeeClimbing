using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClip[] songs;
    AudioSource musicPlayer;
    bool mustPlay = false;

    public bool SetMustPlay
    {
        set { mustPlay = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        musicPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!musicPlayer.isPlaying && mustPlay == true)
        {
            PlayMusic();
        }
    }

    public void PlayMusic()
    {
        mustPlay = true;
        int songNumber = Random.Range(0, songs.Length);
        SetSong(songNumber);
    }

    private void SetSong(int songNumber_)
    {
        Debug.Log("seteo song " +songs[songNumber_].name);
        musicPlayer.clip = songs[songNumber_];
        musicPlayer.Play();
    }

    public void PauseMusic()
    {
        musicPlayer.Pause();
    }
    public void ReturnPlayingMusic()
    {
        musicPlayer.Play();
    }
}

