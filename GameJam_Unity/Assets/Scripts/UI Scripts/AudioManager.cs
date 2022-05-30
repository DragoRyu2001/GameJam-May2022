using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip vampMusic;
    [SerializeField] AudioClip playMusic1;
    [SerializeField] AudioClip playMusic2;
    [SerializeField] Slider volumeSlider;


    [SerializeField] AudioSource audioSource;


    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Use this for initialization


    void Start()
    {

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
            audioSource.volume = first ? 0.05f : 0.03f;
            audioSource.Play();
        }
    }

    public void StopPlaying()
    {
        audioSource.Stop();
    }

    void OnEnable()
    {
        //Register Slider Events
        volumeSlider.onValueChanged.AddListener(delegate { ChangeVolume(volumeSlider.value); });
    }

    //Called when Slider is moved
    void ChangeVolume(float sliderValue)
    {
        audioSource.volume = sliderValue;
    }
}