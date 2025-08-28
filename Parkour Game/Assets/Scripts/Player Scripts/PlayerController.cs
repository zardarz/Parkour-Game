using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float movementSpeed; // the speed at which the player reaches their max speed
    [SerializeField] private float jumpForce; // how high the player can jump
    [SerializeField] private float jumpBuffer; // the distance from the ground the player has to be in able to jump
    [SerializeField] private float minSpeed; // the slowest speed the player 
    [SerializeField] private float maxGrappleDistance; //  the max grapple distance

    [Header("Refrences")]
    [SerializeField] private Rigidbody2D rb; // ther player's rigidbody
    [SerializeField] private LayerMask player; // everything but the player 

    [SerializeField] private TargetJoint2D targetJoint2D; // pull grapple
    [SerializeField] private DistanceJoint2D distanceJoint2D; // swing grapple

    private float movement; // where the player is trying to move

    private float maxSpeed; // the max speed the player can go

    private bool increasingSpeed = false; // this will controll whether the player can increase their max speed or not
    private bool isGrappeling = false; // this will indecate if the player is grappleing or not

    void Update()
    {
        movement = Input.GetAxisRaw("Horizontal"); // get the direction the player is moving

        if(Input.GetKeyDown(KeyCode.Space)) {// if the player hits space
            Jump(); // we jump   
        }

        if(Input.GetMouseButtonDown(0)) {
            // if the player presses right click
            Grapple(); // we grapple
        }
        if(Input.GetMouseButtonUp(0)) {
            // if the player realeases the right click
            distanceJoint2D.enabled = false; // we disable the distance joint
            targetJoint2D.enabled = false; // and the target joint
        }

        Debug.DrawLine(transform.position, transform.position + (Vector3.down * jumpBuffer), Color.red, 0.001f); // draw the jump ray

        print(maxSpeed + " : " + minSpeed); // for debugging

        SetIncreasingSpeed(); // set the increasing speed bool

        if(distanceJoint2D.enabled || targetJoint2D.enabled) isGrappeling = true; // if the distance or target joints are enabled, we are grappleing

        if(maxSpeed < minSpeed) {maxSpeed = minSpeed;} // if the max speed is less than the min speed, we set the max speed to the min speed

        // increasing speed logic
        if(increasingSpeed) {
            // the player should be increasing speed
            maxSpeed = rb.linearVelocity.magnitude; // the player's max speed is the player's current speed
        } else {
            // if the player is not increasing speed
            rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -maxSpeed, maxSpeed); // max the speed the player can go
            
            // and if they go below their max speed
            if(rb.linearVelocity.magnitude < maxSpeed) {
                maxSpeed = rb.linearVelocity.magnitude; // then their max speed is their current speed
            }
        }
        
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

    private void SetIncreasingSpeed() {
        // this will set the increasing speed bool based on some logic
        increasingSpeed = false;

        if(isGrappeling) {
            // if the player is grappleing
            increasingSpeed = true; // we can increase the speed
        } 
    }


    private void Grapple() {
        // this will make a grapple that the player can swing from

        Vector2 mouseDir = (Vector2) (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized; // get the direction of the mouse

        RaycastHit2D hit = Physics2D.Raycast(transform.position, mouseDir, maxGrappleDistance, player); // shot a ray from the player towards the mouse
        Debug.DrawLine(transform.position, transform.position + (Vector3) mouseDir * maxGrappleDistance, Color.green, 3f);

        if (hit.collider == null) return; // if we hit nothing we return

        // but if we do hit something

        distanceJoint2D.enabled = true; // we enable the distance joint


        distanceJoint2D.connectedAnchor = hit.point; // we set the anchor to the positions of the collision of the ray
    }
}