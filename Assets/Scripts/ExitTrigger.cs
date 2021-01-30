using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    private Collider col = default;
    
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var playerDataConection = other.GetComponent<PlayerDataConection>();
        if(!playerDataConection) return;
        
        switch (playerDataConection.GetState())
        {
            case 2:
                playerDataConection.SetState(4);
                break;
        }
    }
}
