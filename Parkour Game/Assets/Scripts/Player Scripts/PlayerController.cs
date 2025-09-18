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
    
    [SerializeField] private LineRenderer lineRenderer; // line renderer for the grapple

    private SpriteRenderer spriteRenderer; // the sprite renderer to control where the character is facing


    private float movement; // where the player is trying to move

    private float maxSpeed; // the max speed the player can go

    private bool increasingSpeed = false; // this will controll whether the player can increase their max speed or not
    private bool isGrappeling = false; // this will indecate if the player is grappleing or not

    void Start() {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>(); // get the sprite renderer
    }

    void Update()
    {
        movement = Input.GetAxisRaw("Horizontal"); // get the direction the player is moving
        FlipCharacter(); // flip the player based on what direction they are moving in

        if(Input.GetKeyDown(KeyCode.Space)) {// if the player hits space
            Jump(); // we jump   
        }

        if(Input.GetMouseButtonDown(0)) {
            // if the player presses right click
            SwingGrapple(); // we grapple
        }

        if(Input.GetKey(KeyCode.LeftShift)) {
            // if the player presses down left shift
            SwitchToPullGrapple(); // we switch to pull grapple
        }

        if(Input.GetKey(KeyCode.LeftShift) == false) {
            // if the player realeases the left shift
            SwitchToSwingGrapple(); // we switch the pull grapple to swing grapple
        }

        if(Input.GetMouseButtonUp(0)) {
            // if the player realeases the right click
            distanceJoint2D.enabled = false; // we disable the distance joint
            targetJoint2D.enabled = false; // and the target joint
            isGrappeling = false; // we are not grappleing anymore

            lineRenderer.enabled = false; // disable the line renderer
        }

        SetLineRendererPos();

        Debug.DrawLine(transform.position, transform.position + (Vector3.down * jumpBuffer), Color.red, 0.001f); // draw the jump ray

        //print(maxSpeed + " : " + minSpeed); // for debugging

        SetIncreasingSpeed(); // set the increasing speed bool

        //print(increasingSpeed); // for debugging

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


    private void SwingGrapple() {
        // this will make a grapple that the player can swing from

        Vector2 mouseDir = (Vector2) (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized; // get the direction of the mouse

        RaycastHit2D hit = Physics2D.Raycast(transform.position, mouseDir, maxGrappleDistance, player); // shot a ray from the player towards the mouse
        Debug.DrawLine(transform.position, transform.position + (Vector3) mouseDir * maxGrappleDistance, Color.green, 3f);

        if (hit.collider == null) return; // if we hit nothing we return

        // but if we do hit something

        distanceJoint2D.enabled = true; // we enable the distance joint

        distanceJoint2D.connectedAnchor = hit.point; // we set the anchor to the positions of the collision of the ray

        distanceJoint2D.distance = Vector2.Distance(transform.position, hit.point); // set the distance of the joint because we can't have it auto set

        lineRenderer.enabled = true; // enable the line renderer
        
        lineRenderer.SetPosition(1, hit.point); // set the line renderer pos
    }

    private void SwitchToPullGrapple() {
        // this will change the distance joint in for a target joint

        if(distanceJoint2D.enabled == false) return; // if the distance joint is not enabled we return

        targetJoint2D.enabled = true; // we enable the target joint
        distanceJoint2D.enabled = false; // and disable the distance joint

        targetJoint2D.target = distanceJoint2D.connectedAnchor; // we set the target of the target joint to the position of the distance joint anchor
    }

    private void SwitchToSwingGrapple() {
        // this will chagne the target joint to a distance joint

        if(targetJoint2D.enabled == false) return; // if the target joint is not enabled we return

        targetJoint2D.enabled = false; // disable the target joint
        distanceJoint2D.enabled = true; // we enable the distance joint

        // we wont change the anchor
    }

    private void SetLineRendererPos() {
        // this will set the line renderer pos to the current player position

        lineRenderer.SetPosition(0, transform.position); // set the position of the line renderer
    }

    private void FlipCharacter() {
        if(rb.linearVelocityX < -0.1f) {
            spriteRenderer.flipX = true;
        } else if(rb.linearVelocityX > 0.1f)  {
            spriteRenderer.flipX = false;
        }
    }
}