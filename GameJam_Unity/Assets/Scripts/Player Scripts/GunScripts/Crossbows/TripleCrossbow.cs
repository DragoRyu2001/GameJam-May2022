using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleCrossbow : GunGeneral
{
    [SerializeField] GameObject bolt;
    [SerializeField, Range(0.2f, 4f)] float yOffset, xOffset;

    Vector3 a, b, c;
    // Start is called before the first frame update
    void Start()
    {
        SetBaseParameters();
    }

    // Update is called once per frame
    void Update()
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
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Shoot()
    {
        if(audioSrc.clip!=shootAudio)
            audioSrc.clip = shootAudio;
        audioSrc.volume = 0.5f;
        a = new Vector3(muzzle.position.x, muzzle.position.y + (yOffset / 2), muzzle.position.z);
        b = new Vector3(muzzle.position.x + (xOffset/2) , muzzle.position.y - (yOffset / 2), muzzle.position.z);
        c = new Vector3(muzzle.position.x - (xOffset/2) , muzzle.position.y - (yOffset / 2), muzzle.position.z);
        Instantiate(bolt, a, muzzle.rotation);
        audioSrc.Play();
        yield return new WaitForSeconds(fireRate);
        Instantiate(bolt, b, muzzle.rotation);
        audioSrc.Play();
        yield return new WaitForSeconds(fireRate);
        Instantiate(bolt, c, muzzle.rotation);
        audioSrc.Play();
        currentAmmo -= 1;
    }

}
