using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // this script will put the camera infront of the player based on where they are moving and how fast 

    [Header("Settings")]
    [Range(0,1)]
    [SerializeField] private float velocityFollowStrength; // this is how strong the camera will follow the player's velocity

    [Header("Refrences")]
    [SerializeField] private GameObject player; // this is the player

    [SerializeField] private Transform target; // this is what the camera will follow
    private Rigidbody2D rb; // the rigidbody of the player
    private TargetJoint2D joint;

    public void Start() {
        joint = gameObject.AddComponent<TargetJoint2D>(); // add the traget joint
        rb = player.GetComponent<Rigidbody2D>(); // get the player's rigidbody
    }

    public void FixedUpdate() {
        joint.target = new Vector2(target.position.x, target.position.y); // set the joints target
    }

    void Update() {
        target.position = new(player.transform.position.x + rb.linearVelocity.x * velocityFollowStrength, player.transform.position.y + rb.linearVelocity.y * velocityFollowStrength, -10f);// set the target's position to be the player's position plus the player's velocity
    }
}