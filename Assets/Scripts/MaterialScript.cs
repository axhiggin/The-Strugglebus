using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Debug.Log("material picked up");
            GameManager.Instance.materialCount += 1;
            Destroy(this.gameObject);
        }
    }
}
