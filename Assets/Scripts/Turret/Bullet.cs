using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 20.0f, bulletDamage = 5.0f, bulletDuration = 1.0f;
    public Transform enemy;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        StartCoroutine(DestroyBullet());
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = enemy.position - transform.position;
        rb.velocity = direction.normalized * bulletSpeed;
        this.gameObject.transform.right = direction;
        if(this.rb.velocity.magnitude == 0)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D Col){
        if (Col.gameObject.transform.CompareTag("Enemy"))
        {
            Col.GetComponent<EnemyBasic>().damageEnemy(bulletDamage);
            // Debug.Log("hit enemy for: " + bulletDamage);
            this.gameObject.SetActive(false);
        }
    }

    private IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(bulletDuration);
        this.gameObject.SetActive(false);
    }
}
