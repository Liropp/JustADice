using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private AudioSource[] musics;
    public Sound[] sounds;
    void Awake()
    {
        StartVolume();

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = Mathf.Clamp(PlayerPrefs.GetFloat("G_volume") - s.volumeDown, 0, 1);
            s.source.pitch = s.pitch;
            s.source.playOnAwake = false;
        }

        UpdateVolume();
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

    public void StartVolume()
    {
        if (PlayerPrefs.GetInt("FIRSTTIMEOPENING", 1) == 1)
        {
            Debug.Log("First Time Opening");

            PlayerPrefs.SetFloat("G_volume", slider.value);

            //Set first time opening to false
            PlayerPrefs.SetInt("FIRSTTIMEOPENING", 0);

            //Do your stuff here
            PlayerPrefs.SetString("FirstLaunch", "true");
        }
        else
        {
            Debug.Log("NOT First Time Opening");

            //Do your stuff here
            slider.value = PlayerPrefs.GetFloat("G_volume");

            //Debug.Log("volumeSlider = " + slider.value);
            //Debug.Log("volumePP = " + PlayerPrefs.GetFloat("G_volume"));
        }
    }

    public void SetVolume()
    {
        PlayerPrefs.SetFloat("G_volume", slider.value);
    }

    public void UpdateVolume()
    {
        foreach (Sound s in sounds)
        {
            s.source.volume = Mathf.Clamp(PlayerPrefs.GetFloat("G_volume") - s.volumeDown, 0, 1);
            //Debug.Log("volume = " + s.source.volume);
        }

        foreach (AudioSource m in musics)
        {
            m.volume = PlayerPrefs.GetFloat("G_volume");
            //Debug.Log("volume = " + m.volume);
        }
    }

    public void QuitSettings()
    {
        FindObjectOfType<AudioManager>().UpdateVolume();
    }
}
