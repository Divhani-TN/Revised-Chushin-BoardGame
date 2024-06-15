using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public Volume[] musicSounds, sfxSounds;

    public AudioSource musicSource, sfxSource;
    private static bool isInitialized = false;

    private void Awake()
    {
        if (isInitialized)
        {
            // Avoid duplicates but don't destroy the object
            if (Instance != this)
            {
                return;
            }
        }

        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
            isInitialized = true;
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
            sfxSource.Play();
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
        musicSource.volume = Mathf.Pow(volume, 2);
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Pow(volume, 2);
    }
}