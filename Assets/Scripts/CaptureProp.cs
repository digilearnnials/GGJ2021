using UnityEngine;

public class CaptureProp : MonoBehaviour
{
    [SerializeField] private string targetTag = "NPC";
    [SerializeField] private LayerMask raycastMask = default;
    [SerializeField] private float rayDistance = 5;
    [SerializeField] private float rayRadius = 1f;
    [SerializeField, Range(0, 10)] private float stunTimer = 5;

    [SerializeField] private bool stun = false;
    
    private float lastStun = 0;
    
    private RaycastHit hit = default;

    private void LateUpdate()
    {
        if (stun && Time.time - lastStun > stunTimer)
        {
            stun = false;
        }
    }

    public int TryToCapture()
    {
        if (Physics.SphereCast(Camera.main.transform.position, rayRadius, Camera.main.transform.forward, out hit, rayDistance, raycastMask))
        {
            if (hit.transform.CompareTag(targetTag))
            {
               return hit.transform.GetComponent<PlayerDataConection>().GetActorNumber();
            }
            else
            {
                stun = true;
                lastStun = Time.time;
            }
        }

        return 0;
    }

    public bool CheckStuned()
    {
        return stun;
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (hit.transform)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hit.point, rayRadius);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Camera.main.transform.position + Camera.main.transform.forward * rayDistance, rayRadius);
        }
        
        Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * rayDistance);
    }
#endif
}
