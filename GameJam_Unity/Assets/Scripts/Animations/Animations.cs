using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class Animations : MonoBehaviour
{
    
    [SerializeField] TwoBoneIKConstraint tbik;
    [SerializeField] MultiAimConstraint mc;
    [SerializeField] Rig rig;
    float targetWeight = 1f;
    void Start()
    {
        
    }

    
    void Update()
    {
        if(targetWeight!=tbik.weight|| targetWeight!= mc.weight)
        {
            tbik.weight = targetWeight;
            mc.weight = targetWeight;
        }
        
    }
    public void SetTBIK(float weight)
    {
        //tbik.weight = weight;
        targetWeight = weight;
    }
}
