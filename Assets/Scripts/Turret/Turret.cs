using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Norman Zhu 3/13/2024 3:27PM - laser firing sou nd effects

// Norman Zhu 3/13/2024 8:31PM - fix turret attack speed issue.
//                               fix turret targeting inactive enemies.

// Last edited - Guy Haiby 12:11 AM 3/10/2024, animations controller, shooting logic
public class Turret : MonoBehaviour
{
    //turret stats
    [SerializeField] float Range = 3.0f, FireRate = 0.4f;
    [SerializeField] GameObject bullet;
    //list of enemies within range
    private List<GameObject> enemyList = new List<GameObject>();
    private CircleCollider2D cirCol;
    private Vector2 Direction; // direction to face turret and bullet
    private float nextFire = 0;

    private GameObject target;
    
    //animation controller
    private Animator _animator;

    public Transform shootPoint; //shoot from this point
    void Start()
    {
        cirCol = GetComponent<CircleCollider2D>();
        cirCol.radius = Range;

        _animator = GetComponent<Animator>();

    }


    void Update()
    {
        nextFire += Time.deltaTime;
        //raycast for detection
        foreach (GameObject enemy in enemyList)
        {
<<<<<<< Updated upstream
            if (!enemy.activeInHierarchy)
=======
            //update target position
            Vector2 targetPos = enemy.transform.position;
            Direction = targetPos - (Vector2)transform.position;
            //make turret face target
            this.gameObject.transform.GetChild(0).right = Direction;

            nextFire += Time.deltaTime;
            if (1 / FireRate <= nextFire)
>>>>>>> Stashed changes
            {
                enemyList.Remove(enemy);
            }
        }

        if (nextFire >= FireRate && enemyList.Count > 0)
        {
            shootAnEnemy();
        }
            
    }

    void shootAnEnemy()
    {
        //update target position
        Vector2 targetPos = enemyList[0].transform.position;
        Direction = targetPos - (Vector2)transform.position;
        //make turret face target
        this.gameObject.transform.right = Direction;

        nextFire = 0f;
        shoot(enemyList[0]);
    }

    void shoot(GameObject enemy)
    {
        target = enemy;
        //shoot animation
        _animator.SetTrigger("ShootTrigger");
    }

    public void OnShootAnimationEnd(){
        
        //spawn bullet
        GameObject BulletIns = BulletPooler.Instance.GetPooledObject();
        BulletIns.transform.position = shootPoint.position;
        BulletIns.transform.rotation = Quaternion.identity;
        BulletIns.SetActive(true);
        BulletIns.GetComponent<Bullet>().enemy = target.transform;

        GameObject laserSoundSource = AudioFxManager.Instance.GetAudioObject();
        if (laserSoundSource != null)
        {
            laserSoundSource.SetActive(true);
            laserSoundSource.transform.position = transform.position;
            int audioClip = Random.Range(0, AudioFxManager.Instance.laserSounds.Length);
            laserSoundSource.GetComponent<AudioSource>().PlayOneShot(AudioFxManager.Instance.laserSounds[audioClip]);
            AudioFxManager.Instance.deactivateObjectAfterDelay(AudioFxManager.Instance.laserDuration[audioClip], laserSoundSource);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.transform.CompareTag("Enemy"))
        {
            enemyList.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.transform.CompareTag("Enemy"))
        {
            enemyList.Remove(collision.gameObject);
        }
    }
}