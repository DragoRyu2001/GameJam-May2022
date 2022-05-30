using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimations : MonoBehaviour
{
    [SerializeField] AudioSource audioSrc;
    [SerializeField] AudioClip[] stepClips;
    [SerializeField] AudioClip swordClip, arrowClip, poundClip;
    [SerializeField] Animator anim;
    float blend;
    void Step()
    {
        blend = anim.GetFloat("Blend");
        if(blend>0.75)
        {
            audioSrc.clip = stepClips[Random.Range(0, stepClips.Length)];
            audioSrc.Play();
        }
    }
    void Swoop()
    {
        audioSrc.clip = swordClip;
        audioSrc.Play();
    }
    void Shoot()
    {
        audioSrc.clip = arrowClip;
        audioSrc.Play();
    }
    void Pound()
    {
        audioSrc.clip = poundClip;
        audioSrc.Play();
    }
}
