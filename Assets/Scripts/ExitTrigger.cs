using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    private Collider col = default;

    private LocalGameManager localGameManager = default;
    
    // Start is called before the first frame update
    void Start()
    {
        localGameManager = FindObjectOfType<LocalGameManager>();
        
        col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerDataConection playerDataConection = other.GetComponent<PlayerDataConection>();
        
        if(!playerDataConection) return;
        
        switch (playerDataConection.GetState())
        {
            case 2:
                playerDataConection.SetState(4);
                break;
        }
    }
}
