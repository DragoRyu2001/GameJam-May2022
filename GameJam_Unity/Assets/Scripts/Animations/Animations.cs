using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class Animations : MonoBehaviour
{
    
    [SerializeField] TwoBoneIKConstraint rifleTbik, shotgunTbik, crossbowTbik;
    [SerializeField] Animator anim;
    [SerializeField] MultiAimConstraint mac, smac;
    [SerializeField] AudioSource audioSrc;
    [SerializeField] AudioClip[] stepClips;
    [SerializeField] Rig rig;
    TwoBoneIKConstraint activeTBIK, prevTBIK;
    float move;
    float targetWeight = 1f, stargetWeight = 0.5f;
    void Start()
    {
        prevTBIK = rifleTbik;
        activeTBIK = rifleTbik;
    }

    
    void Update()
    {
        if(activeTBIK!=prevTBIK)
        {
            rifleTbik.weight = 0;
            shotgunTbik.weight = 0;
            crossbowTbik.weight =0;
            activeTBIK.weight = 1;
            prevTBIK = activeTBIK;
            Debug.Log(activeTBIK);
        }
        if(targetWeight!=activeTBIK.weight|| targetWeight!= mac.weight)
        {
            activeTBIK.weight = targetWeight;
            mac.weight = targetWeight;
        }
        if(stargetWeight!=smac.weight)
        {
            smac.weight = stargetWeight;
        }
        
    }
    public void SetWeights(float weight)
    {
        targetWeight = weight;
    }
    public void SetWeapon(int weapon)
    {
        switch(weapon)
        {
            case 0:
                activeTBIK = rifleTbik;
                break;
            case 1:
                activeTBIK = shotgunTbik;
                break;
            case 2:
                activeTBIK = crossbowTbik;
                break;
            default:
                Debug.Log("Invalid Case provided in SetWeapon() in Animations.cs");
                break;
        }
    }
    public void SetSpine(float weight)
    {
        stargetWeight = weight;
    }
    void Step()
    {
        move = anim.GetFloat("Move");
        if(move<0.95f||move>1.05f)
        {
            Debug.Log("This is Playing");
            audioSrc.clip = stepClips[Random.Range(0, stepClips.Length)];
            audioSrc.Play();
        }
    }
}
