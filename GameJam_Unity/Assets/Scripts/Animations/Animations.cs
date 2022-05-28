using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class Animations : MonoBehaviour
{
    
    [SerializeField] TwoBoneIKConstraint rifleTbik, shotgunTbik, crossbowTbik;
    [SerializeField] MultiAimConstraint mac;
    [SerializeField] Rig rig;
    TwoBoneIKConstraint activeTBIK, prevTBIK;
    float targetWeight = 1f;
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
}
