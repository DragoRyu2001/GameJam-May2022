using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfAnimations : MonoBehaviour
{
    [SerializeField] AudioSource audioSrc;
    [SerializeField] AudioClip swoopAudio, poundAudio;

    void Swoop()
    {
        audioSrc.clip = swoopAudio;
        audioSrc.Play();
    }
    void Pound()
    {
        audioSrc.clip = poundAudio;
        audioSrc.Play();
    }
}
