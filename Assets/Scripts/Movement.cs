#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    [SerializeField] private bool lockToViewDirection = false;
    [SerializeField] float moveSpeed = 10;

    [SerializeField] private Vector3 inputDirection = default;
    [SerializeField] private Vector3 moveDirection = default;
    [SerializeField] private Vector3 lookDirection = default;
    private Rigidbody body = default;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

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
        
        body.velocity = moveDirection * moveSpeed;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.yellow;
        Handles.ArrowHandleCap(0, transform.position, transform.rotation, 1f, EventType.Repaint);
    }
#endif
}
