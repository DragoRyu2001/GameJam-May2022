using UnityEngine;
using System.Collections;

public class Crossbow : GunGeneral
{
    [SerializeField] GameObject bolt;

    private void Start()
    {
        SetBaseParameters();
    }

    private void Update()
    {
        OrientMuzzle();
        CanShootCheck();
        ReadInput();
    }

    private void ReadInput()
    {
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            anim.SetTrigger("Attack");
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot()
    {
        if(audioSrc.clip!=shootAudio)
            audioSrc.clip = shootAudio;
        audioSrc.volume = 1f;
        audioSrc.Play();
        Instantiate(bolt, muzzle.position, muzzle.rotation);
        currentAmmo -= 1;
        yield return new WaitForSeconds(0.15f);
        StartCoroutine(Reload());
    }
}
