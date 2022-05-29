using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float speed;
    [SerializeField] bool isPlayer;
    [SerializeField] float maxDist;
    Vector3 startPos;
    bool canMove;

    private PlayerScript playerComponent;
    private Enemy enemyComponent;


    void Start()
    {
        if(!isPlayer)
        {
            damage = GameManager.instance.CrossbowDamageUpdate();
        }
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
            if (other.TryGetComponent(out enemyComponent))
            {
                enemyComponent.TakeDamage(damage, isPlayer);
            }
            transform.parent = other.transform;
            canMove = false;
            Destroy(gameObject, 5f);
        }
        else if (!isPlayer && other.transform.CompareTag("Player"))
        {
            if(other.transform.root.TryGetComponent(out playerComponent))
            {
                playerComponent.TakeDamage(damage);
            }
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
