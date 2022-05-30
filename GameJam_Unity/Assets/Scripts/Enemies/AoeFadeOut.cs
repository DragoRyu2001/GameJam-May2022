using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeFadeOut : MonoBehaviour
{
    [SerializeField] float timer;
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject[] particles;
    void Start()
    {
        StartCoroutine(AOE());
    }
    IEnumerator AOE()
    {
        yield return new WaitForSeconds(timer);
        foreach(GameObject objs in particles)
        {
            objs.SetActive(false);
        }
        GameObject obj = Instantiate(explosion, transform.position, explosion.transform.rotation);
        Destroy(obj, 1f);
        Destroy(this.gameObject, 1.5f);
    }
}
