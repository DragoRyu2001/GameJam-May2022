using UnityEngine;

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
            Shoot();
            StartCoroutine(Reload());
        }
    }

    private void Shoot()
    {
        if(audioSrc.clip!=shootAudio)
            audioSrc.clip = shootAudio;
        audioSrc.volume = 0.5f;
        audioSrc.Play();
        Instantiate(bolt, muzzle.position, muzzle.rotation);
        currentAmmo -= 1;

    }
}
