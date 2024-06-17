using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Slider _musicSlider_, _sfxSlider_;
    public SoundManager soundManager;

    public void ToggleMusic()
    {
        //SoundManager.Instance.ToggleMusic();
        soundManager.ToggleMusic();
    }

    public void ToggleSFX()
    {
        //SoundManager.Instance.ToggleSfx();
        soundManager.ToggleSfx();
    }

    public void MusicVolume()
    {
        //SoundManager.Instance.MusicVolume(_musicSlider_.value);
        soundManager.MusicVolume(_musicSlider_.value);
    }
    public void SFXVolume()
    {
        //SoundManager.Instance.SFXVolume(_musicSlider_.value);
        soundManager.SFXVolume(_sfxSlider_.value);
    }
}
