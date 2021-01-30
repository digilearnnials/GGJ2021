#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    [SerializeField] private bool lockToViewDirection = false;
    [SerializeField] float moveSpeed = 10;

    private Vector3 inputDirection = default;
    private Vector3 moveDirection = default;
    private Vector3 lookDirection = default;
    private Vector3 velocity = default;
    private Rigidbody body = default;

    private bool onground = false;
    
    public bool LockToViewDirection
    {
        get => lockToViewDirection;
        set => lockToViewDirection = value;
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    public void MoveAction(Vector3 value)
    {
        velocity = body.velocity;
        
        inputDirection = value;
        
        moveDirection = Camera.main.transform.TransformDirection(inputDirection);

        moveDirection.y = 0;

        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);
        
        if (lockToViewDirection)
        {
            var cameraDir = Camera.main.transform.forward;
            cameraDir.y = 0;

            lookDirection = cameraDir;
        }
        else
        {
            if (inputDirection.magnitude > .1f) lookDirection = moveDirection;
        }
        
        body.MoveRotation(Quaternion.LookRotation(lookDirection));

        if (onground)
        {
            velocity.x = moveDirection.x * moveSpeed;
            velocity.z = moveDirection.z * moveSpeed;
        }
        
        body.velocity = velocity;

        onground = false;
    }

    private void OnCollisionExit(Collision other)
    {
        onground = false;
    }

    private void OnCollisionStay(Collision other)
    {
        for (int i = 0; i < other.contactCount; i++) {
            Vector3 normal = other.GetContact(i).normal;
            onground |= normal.y >= 0.9f;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.yellow;
        Handles.ArrowHandleCap(0, transform.position, transform.rotation, 1f, EventType.Repaint);
    }
#endif
}
