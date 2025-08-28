using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    private TargetJoint2D joint;

    public void Start() {
        joint = gameObject.AddComponent<TargetJoint2D>();
    }

    public void FixedUpdate() {
        joint.target = new Vector2(target.position.x, target.position.y);
    }
}