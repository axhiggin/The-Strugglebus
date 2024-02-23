using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Last edited - Norman Zhu 2:32AM 2/22/24
public class Turret : MonoBehaviour
{

    public float Range; //radius range
    public Transform Target; // position of target to shoot at 
    bool Detected = false;

    Vector2 Direction; // direction to face turret and bullets

    public GameObject bullet;

    public float FireRate;
    float nextFire = 0;
    public float Force; //speed of bullet

    public Transform shootPoint; //shoot from this point
    void Start()
    {
        
    }


    void Update()
    {
        //update target position
        Vector2 targetPos = Target.position;
        Direction = targetPos - (Vector2)transform.position;

        //raycast for detection
        RaycastHit2D rayInfo = Physics2D.Raycast(transform.position,Direction,Range);
        if (rayInfo)
        {
            if(rayInfo.collider.gameObject.tag == "Enemy") // target based on tag 
            {
                if (!Detected)
                {
                    Detected = true;
                }
            }
            else
            {
                if (Detected)
                {
                    Detected = false;
                }
            }
        }
        if (Detected)
        {
            //face target
            this.gameObject.transform.up = Direction;
            //shooting mechanism 
            if(Time.time > nextFire)
            {
                nextFire = Time.time + 1 / FireRate;
                shoot();
            }
        }
    }
    void shoot()
    {
        //spawn bullet
        GameObject BulletIns = Instantiate(bullet, shootPoint.position, Quaternion.identity);
        BulletIns.GetComponent<Rigidbody2D>().AddForce(Direction * Force);
        BulletIns.gameObject.transform.up = Direction;
    }
    //Draw range for debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}