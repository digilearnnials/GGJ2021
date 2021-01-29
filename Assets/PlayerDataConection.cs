using System;
using Cinemachine;
using Com.MyCompany.MyGame;
using UnityEngine;
using GGJ2021.Networking;
using Photon.Pun;

[Serializable]
public class PlayerNetworkData : NetworkData
{
    public byte playerState = 0; // 0 = bruja 1 = fantasma 3 = espectador;
    public string propName;
}

public class PlayerDataConection : MonoBehaviour
{
    private Movement movement = default;
    private PropTransformer propTransformer = default;
    private NetworkEntity networkEntity = default;
    private PlayerNetworkData playerNetworkData = default;
    private GameRoomManager gameRoomManager = default;

    private GameObject firstPersonCamera = default;
    private GameObject thirdPersonCamera = default;
    private FollowTarget mapCamera = default;

    private void Awake()
    {
        movement = GetComponent<Movement>();
        propTransformer = GetComponent<PropTransformer>();
        networkEntity = GetComponent<NetworkEntity>();
        playerNetworkData = new PlayerNetworkData();
        
        networkEntity.SubscribeToLocalDataRetrieval(MandarDato);
        networkEntity.SubscribeToLocalInstanceUpdate(RecibirDato);
        
        if(PhotonNetwork.IsConnected && !networkEntity.ClientIsOwner()) return;
        
        firstPersonCamera = GameObject.Find("First Person Cam");
        firstPersonCamera.SetActive(false);
        thirdPersonCamera = GameObject.Find("Third Person Cam");
        thirdPersonCamera.SetActive(false);
        
        mapCamera = FindObjectOfType<FollowTarget>();
        mapCamera.target = transform;
    }

    private void Start()
    {
        if(PhotonNetwork.IsConnected && !networkEntity.ClientIsOwner()) return;
        
        firstPersonCamera.GetComponent<CinemachineVirtualCamera>().Follow = transform;
        firstPersonCamera.SetActive(false);
        
        thirdPersonCamera.GetComponent<CinemachineFreeLook>().Follow = transform;
        thirdPersonCamera.GetComponent<CinemachineFreeLook>().LookAt = transform;
        thirdPersonCamera.SetActive(true);
        
        propTransformer.OnPropTransform.AddListener(TransformedInProp);
    }

    private void FixedUpdate()
    {
        if(PhotonNetwork.IsConnected && !networkEntity.ClientIsOwner()) return;
        
        movement.MoveAction(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
    }
    
    private PlayerNetworkData MandarDato()
    {
        return playerNetworkData;
    }

    private void RecibirDato(NetworkData data)
    {
        var receivedData = (PlayerNetworkData)data;
        propTransformer.ChangeToProp(receivedData.propName);
    }

    void TransformedInProp(string value)
    {
        playerNetworkData.propName = value;
    }
}
