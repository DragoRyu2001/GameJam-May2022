using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip vampMusic;
    [SerializeField] AudioClip playMusic1;
    [SerializeField] AudioClip playMusic2;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider volumeSlider;


    [SerializeField] AudioSource audioSource;


    void Start()
    {
        volumeSlider.value = AudioListener.volume;
        musicSlider.value = 1f;
        audioSource.volume = 1f;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void PlayVampMusic()
    {
        if(!audioSource.isPlaying)
        { 
            audioSource.clip = vampMusic;
            audioSource.Play();
        }
    }

    public void PlayGameMusic()
    {
        if(!audioSource.isPlaying)
        {
            bool first = Random.Range(0, 100) > 51f;
            audioSource.clip = first?playMusic1:playMusic2;
            audioSource.volume = 0.5f;
            audioSource.Play();
        }
    }

    public void StopPlaying()
    {
        audioSource.Stop();
    }

    //Called when Slider is moved
    public void ChangeVolume(Slider val)
    {
        audioSource.volume = val.value;
    }
    public void ChangeGlobalVolume(Slider val)
    {
        AudioListener.volume = val.value;
    }
}