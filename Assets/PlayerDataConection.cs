using System;
using Cinemachine;
using Com.MyCompany.MyGame;
using UnityEngine;
using GGJ2021.Networking;
using Photon.Pun;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[Serializable]
public class PlayerNetworkData : NetworkData
{
    public byte playerState = 0; // 0 = sin estado / 1 = bruja / 2 = fantasma / 3 = espectador;
    public string propName = "TRUE FORM";
    public int actorNumber = 0;
    public string nickName = "";
    public int masterClient = 0;
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
    
    public UnityEvent onWitch = new UnityEvent();
    public UnityEvent onPhantom = new UnityEvent();

    private void Awake()
    {
        movement = GetComponent<Movement>();
        propTransformer = GetComponent<PropTransformer>();
        networkEntity = GetComponent<NetworkEntity>();
        playerNetworkData = new PlayerNetworkData();
        
        networkEntity.SubscribeToLocalDataRetrieval(SendPlayerNetworkData);
        networkEntity.SubscribeToLocalInstanceUpdate(ReceivePlayerNetworkData);
        
        if(PhotonNetwork.IsConnected && !networkEntity.ClientIsOwner()) return;

        playerNetworkData.nickName = PhotonNetwork.LocalPlayer.NickName;
        playerNetworkData.actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        playerNetworkData.masterClient = PhotonNetwork.CurrentRoom.MasterClientId;

        CheckIfIWitch();
        
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
        
        CheckState(playerNetworkData.playerState);
    }

    private void FixedUpdate()
    {
        if(PhotonNetwork.IsConnected && !networkEntity.ClientIsOwner()) return;

        movement.MoveAction(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
        
        if(propTransformer.CheckCanTransform())
        {
            if (Input.GetMouseButton(0))
            {
                propTransformer.TransformTo(propTransformer.Hit.transform.gameObject);
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            propTransformer.BackToOriginalForm();
        }
    }

    private void LateUpdate()
    {
        if (playerNetworkData.masterClient != PhotonNetwork.CurrentRoom.MasterClientId)
        {
            CheckIfIWitch();
            CheckState(playerNetworkData.playerState);
        }
    }

    private PlayerNetworkData SendPlayerNetworkData()
    {
        return playerNetworkData;
    }

    private void ReceivePlayerNetworkData(NetworkData data)
    {
        var receivedData = (PlayerNetworkData)data;

        switch (receivedData.playerState)
        {
            case 1:
                Debug.Log($"El jugador: <color=blue>{receivedData.nickName}</color> es la bruja");
                break;
            case 2:
                Debug.Log($"El jugador: <color=blue>{receivedData.nickName}</color> es la fantasma");
                break;
        }
        
        propTransformer.ChangeToProp(receivedData.propName);
    }

    void TransformedInProp(string value)
    {
        playerNetworkData.propName = value;
    }

    void CheckIfIWitch()
    {
        playerNetworkData.masterClient = PhotonNetwork.CurrentRoom.MasterClientId;
        
        if (playerNetworkData.actorNumber == PhotonNetwork.CurrentRoom.MasterClientId)
        {
            playerNetworkData.playerState = 1; //Bruja
            propTransformer.BackToOriginalForm();
            
            onWitch?.Invoke();
            Debug.Log($"El jugador: <color=blue>{playerNetworkData.nickName}</color> es la bruja");
        }
        else
        {
            playerNetworkData.playerState = 2; //Fantasma
            
            onPhantom?.Invoke();
            Debug.Log($"El jugador: <color=blue>{playerNetworkData.nickName}</color> es la fantasma");
        }
    }

    private void CheckState(byte state)
    {
        switch (state)
        {
            case 1:
                if(firstPersonCamera) firstPersonCamera.SetActive(true);
                if(thirdPersonCamera) thirdPersonCamera.SetActive(false);
                movement.LockToViewDirection = true;
                break;
            case 2:
                if(firstPersonCamera) firstPersonCamera.SetActive(false);
                if(thirdPersonCamera) thirdPersonCamera.SetActive(true);
                movement.LockToViewDirection = false;
                break;
            case 3:
                if(firstPersonCamera) firstPersonCamera.SetActive(false);
                if(thirdPersonCamera) thirdPersonCamera.SetActive(true);
                movement.LockToViewDirection = false;
                break;
        }
    }
}
