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
        if(enemy.gameObject.activeSelf)
        {
            Vector2 direction = enemy.position - transform.position;
            rb.velocity = direction.normalized * bulletSpeed;
            this.gameObject.transform.right = direction;
        }
    }

    void OnTriggerEnter2D(Collider2D Col){
        if (Col.gameObject.transform.CompareTag("Enemy"))
        {
            Col.GetComponent<EnemyBasic>().damageEnemy(bulletDamage);
            // Debug.Log("hit enemy for: " + bulletDamage);
            this.gameObject.SetActive(false);
            int explosionType = 0;
            GameObject explosion = SpriteFxManager.Instance.GetPooledExplosion(explosionType);
            if (explosion != null)
            {
                explosion.SetActive(true);
                explosion.transform.position = Col.transform.position;
                SpriteFxManager.Instance.deactivateSpriteAfterDelay(SpriteFxManager.Instance.explosionDurations[explosionType], explosion);
            }
            GameObject explosionAudio = AudioFxManager.Instance.GetAudioObject();
            if (explosionAudio != null)
            {
                explosionAudio.SetActive(true);
                explosionAudio.transform.position = Col.transform.position;
                int audioClip = Random.Range(0, AudioFxManager.Instance.explosionSounds.Length);
                explosionAudio.GetComponent<AudioSource>().PlayOneShot(AudioFxManager.Instance.explosionSounds[audioClip]);
                AudioFxManager.Instance.deactivateObjectAfterDelay(AudioFxManager.Instance.explosionDuration[explosionType], explosionAudio);
            }
        }
    }

    private IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(bulletDuration);
        this.gameObject.SetActive(false);
    }
}
