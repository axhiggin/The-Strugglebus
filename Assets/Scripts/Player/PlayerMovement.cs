//Worked on by: Aidan
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum direction
    {
        left, right, up, down
    }

    [SerializeField] float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    public direction currDir;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //GET INPUT
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        //update direction of player (used for animations and building)
        if(movement.x > 0)
        {
            currDir = direction.right;
        }
        else if(movement.x < 0)
        {
            currDir = direction.left;
        }
        else if(movement.y > 0)
        {
            currDir = direction.up;
        }
        else if(movement.y < 0)
        {
            currDir = direction.down;
        }
        
    }

    private void FixedUpdate()
    {
        //MOVE PLAYER
        rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);
    }
}
