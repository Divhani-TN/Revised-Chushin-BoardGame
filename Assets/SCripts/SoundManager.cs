using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public Volume[] musicSounds, sfxSounds;
   
    public AudioSource musicSource, sfxSource;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        PlayMusic("Theme");
    }
    public void PlayMusic(string name)
    {
        Volume S = Array.Find(musicSounds, X => X.name == name);
        if (S == null)
        {
            Debug.Log("Sound unfound");
        }
        else
        {
            musicSource.clip = S.clip;
            musicSource.Play();
        }
    }
    public void PlaySFX(string name)
    {
       Volume S = Array.Find(sfxSounds, X => X.name == name);
        if (S == null)
        {
            Debug.Log("Sound unfound");
        }
        else
        {
            sfxSource.clip = S.clip;
            musicSource.Play();
        }
    }
    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }
    public void ToggleSfx()
    {
        sfxSource.mute = !sfxSource.mute;
    }
     public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }
    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
