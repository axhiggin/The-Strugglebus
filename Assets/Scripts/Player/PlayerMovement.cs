
//Norman - 3/7/24 9:36PM - bugfix. player collision now destroys the gameobject, not the transform.
//Worked on by: Aidan
//Added Player Rotation: Trevor
//Norman - 2/27/24 10:41PM - placeholder comment. Either use update or trigger to call PathingMap.Instance.generateFlowField(playerCell);

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private bool isMoving;
    public enum direction
    {
        left, right, up, down
    }

    [SerializeField] float moveSpeed = 5f;
    private Rigidbody2D rb;
    private SpriteRenderer rbSprite;
    private Vector2 movement;
    public direction currDir;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rbSprite = rb.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //GET INPUT
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (movement.magnitude > 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        GetComponent<Animator>().SetBool("IsWalking", isMoving);

        //update direction of player (used for animations and building)
        if (movement.x > 0)
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

        if(currDir == direction.right)
        {
            rbSprite.flipX = false;
        }
        if(currDir == direction.left)
        {
            rbSprite.flipX = true;
        }


        /*//rotation (easy thanks to direction enum thanks)
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
        }*/
        
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
    



    //picking up materials
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Material"))
        {
            Debug.Log("material picked up");
            //Destroy(collision.transform);
            Destroy(collision.gameObject);
            gameObject.GetComponent<PlayerBuild>().materialCount += 1;
            //update UI here
            
        }
    }
}
