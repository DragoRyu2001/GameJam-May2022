using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float speed;
    [SerializeField] bool isPlayer;
    [SerializeField] float maxDist;
    [SerializeField] PlayerScript playerScript;
    Vector3 startPos;
    bool canMove;

    void Start()
    {
        startPos = transform.position;
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, startPos) > maxDist)
        {
            Destroy(gameObject);
        }
        if (canMove)
        {
            transform.Translate(speed * Time.deltaTime * Vector3.forward);
        }

    }
    void OnTriggerEnter(Collider other)
    {
        if (isPlayer && other.transform.CompareTag("Enemy"))
        {
            other.transform.gameObject.GetComponent<Enemy>().TakeDamage(damage, isPlayer, playerScript.InBerserk);
            Destroy(gameObject, 5f);
        }
        else if (!isPlayer && other.transform.tag == "Player")
        {
            other.transform.gameObject.GetComponent<PlayerScript>().TakeDamage(damage);
            Debug.Log("Player Shot");
            Destroy(gameObject);
        }
        else if (other.transform.CompareTag("Level"))
        {
            canMove = false;
            Destroy(gameObject, 5f);
        }

    }
    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

}
