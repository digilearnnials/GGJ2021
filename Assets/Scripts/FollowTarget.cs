using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target = default;
    [SerializeField] private Vector3 offsetPos = new Vector3(0, 10, 0);

    private void LateUpdate()
    {
        if(target) transform.position = target.position + offsetPos;
    }
}
