using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Last edited - Norman Zhu 2:32AM 2/22/24
public class Turret : MonoBehaviour
{
    //turret stats
    [SerializeField] float Range = 3.0f, FireRate = 1.0f, Force = 100.0f;
    [SerializeField] GameObject bullet;
    //list of enemies within range
    private List<GameObject> enemyList = new List<GameObject>();
    private CircleCollider2D cirCol;
    private Vector2 Direction; // direction to face turret and bullet
    private float nextFire = 0;
    

    public Transform shootPoint; //shoot from this point
    void Start()
    {
        cirCol = GetComponent<CircleCollider2D>();
        cirCol.radius = Range;
    }


    void Update()
    {
        //raycast for detection
        foreach (GameObject enemy in enemyList)
        {
            //update target position
            Vector2 targetPos = enemy.transform.position;
            Direction = targetPos - (Vector2)transform.position;
            Debug.Log(enemy.name);

            this.gameObject.transform.up = Direction;

            Debug.Log(nextFire);
            nextFire += Time.deltaTime;
            if (1 / FireRate <= nextFire)
            {
                nextFire = 0f;
                shoot(enemy);
            }
        }
    }

    void shoot(GameObject enemy)
    {
        //spawn bullet
        GameObject BulletIns = Instantiate(bullet, shootPoint.position, Quaternion.identity);
        BulletIns.GetComponent<Bullet>().enemy = enemy.transform;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.transform.CompareTag("Enemy"))
        {
            enemyList.Add(collision.gameObject);
            Debug.Log(enemyList.Count);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.transform.CompareTag("Enemy"))
        {
            enemyList.Remove(collision.gameObject);
            Debug.Log(enemyList.Count);
        }
    }
}