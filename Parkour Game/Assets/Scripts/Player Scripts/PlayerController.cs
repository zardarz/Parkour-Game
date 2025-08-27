using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float movementSpeed; // the speed at which the player reaches their max speed
    [SerializeField] private float jumpForce; // how high the player can jump
    [SerializeField] private float jumpBuffer; // the distance from the ground the player has to be in able to jump
    [SerializeField] private float minSpeed; // the slowest speed the player 

    [Header("Refrences")]
    [SerializeField] private Rigidbody2D rb; // ther player's rigidbody
    [SerializeField] private LayerMask player; // everything but the player 

    private float movement; // where the player is trying to move

    private float maxSpeed; // the max speed the player can go

    void Update()
    {
        movement = Input.GetAxisRaw("Horizontal"); // get the direction the player is moving

        if(Input.GetKeyDown(KeyCode.Space)) {// if the player hits space
            Jump(); // we jump   
        }

        Debug.DrawLine(transform.position, transform.position + (Vector3.down * jumpBuffer), Color.red, 0.001f); // draw the jump ray

        //print(maxSpeed + " : " + minSpeed);
        if(maxSpeed < minSpeed) {maxSpeed = minSpeed;} // if the max speed is less than the min speed, we set the max speed to the min speed
        rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -maxSpeed, maxSpeed); // max the speed the player can go
    }

    void FixedUpdate()
    {
        rb.AddForce(new(movement * movementSpeed * Time.deltaTime, 0), ForceMode2D.Impulse); // add the force of the movement
    }

    private void Jump() {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, jumpBuffer, player); // send a raycast down
        

        if(hit.collider != null) { // if the cast hits anything
            rb.AddForce(new(0,jumpForce), ForceMode2D.Impulse); // add the jump force
        }
    }
}