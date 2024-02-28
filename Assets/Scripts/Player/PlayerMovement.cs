//Worked on by: Aidan
//Added Player Rotation: Trevor
//Norman - 2/27/24 10:41PM - placeholder comment. Either use update or trigger to call PathingMap.Instance.generateFlowField(playerCell);

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
        //rotation (easy thanks to direction enum thanks)
        switch (currDir)
        {
            case direction.up:
                RotatePlayer(0);
                break;
            case direction.down:
                RotatePlayer(180);
                break;
            case direction.left:
                RotatePlayer(90);
                break;
            case direction.right:
                RotatePlayer(270);
                break;
        }
        
    }

    private void FixedUpdate()
    {
        //MOVE PLAYER
        rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);
    }
    //rotation
    void RotatePlayer(float angle)
    {
        rb.rotation = angle;
    }
}
