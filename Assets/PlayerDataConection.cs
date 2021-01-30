using System;
using Cinemachine;
using Com.MyCompany.MyGame;
using UnityEngine;
using GGJ2021.Networking;
using Photon.Pun;

[Serializable]
public class PlayerNetworkData : NetworkData
{
    public byte playerState = 0; // 0 = sin estado / 1 = bruja / 2 = fantasma / 3 = perdedor / 4 = ganador;
    public string propName = "TRUE FORM";
    public int actorNumber = 0;
    public string nickName = "";
    public int masterClient = 0;
    public int capurePlayer = 0;
}

public class PlayerDataConection : MonoBehaviour
{
    private Movement movement = default;
    private PropTransformer propTransformer = default;
    private CaptureProp captureProp = default;
    
    private NetworkEntity networkEntity = default;
    [SerializeField] private PlayerNetworkData playerNetworkData = default;
    private GameRoomManager gameRoomManager = default;

    private GameObject firstPersonCamera = default;
    private GameObject thirdPersonCamera = default;
    private FollowTarget mapCamera = default;
    private GameObject miniMapUI = default;

    private void Awake()
    {
        movement = GetComponent<Movement>();
        propTransformer = GetComponent<PropTransformer>();
        captureProp = GetComponent<CaptureProp>();
        
        networkEntity = GetComponent<NetworkEntity>();
        playerNetworkData = new PlayerNetworkData();
        
        networkEntity.SubscribeToLocalDataRetrieval(SendPlayerNetworkData);
        networkEntity.SubscribeToLocalInstanceUpdate(ReceivePlayerNetworkData);

        if (PhotonNetwork.IsConnected)
        {
            if (!networkEntity.ClientIsOwner())
            {
                gameObject.tag = "NPC";
            }
            else
            {
                gameObject.tag = "Player";
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
                
                miniMapUI = GameObject.Find("MiniMap");
                miniMapUI.SetActive(false);
            }
        }
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

        switch (playerNetworkData.playerState)
        {
            case 1:
                if (!captureProp.CheckStuned())
                {
                    movement.MoveAction(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
                    
                    if (Input.GetMouseButton(0))
                    {
                        playerNetworkData.capurePlayer = captureProp.TryToCapture();
                    }
                    else
                    {
                        playerNetworkData.capurePlayer = 0;
                    }
                }
                break;
            case 2:
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
                break;
            case 3:
                movement.SpectatorMode();
                movement.SpectatorMoveAction(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
                break;
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
        playerNetworkData = (PlayerNetworkData)data;

        switch (playerNetworkData.playerState)
        {
            case 1:
                if (playerNetworkData.capurePlayer > 0)
                {
                    var players = FindObjectsOfType<PlayerDataConection>();

                    foreach (var player in players)
                    {
                        if (player.GetActorNumber() == playerNetworkData.capurePlayer)
                        {
                            player.SetState(3);
                            player.GetComponent<Movement>().SpectatorMode();
                        }
                    }
                }
                Debug.Log($"El jugador: <color=blue>{playerNetworkData.nickName}</color> es la bruja");
                break;
            case 2:
                Debug.Log($"El jugador: <color=blue>{playerNetworkData.nickName}</color> es la fantasma");
                break;
        }
        
        propTransformer.ChangeToProp(playerNetworkData.propName);
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
            
            Debug.Log($"El jugador: <color=blue>{playerNetworkData.nickName}</color> es la bruja");
        }
        else
        {
            playerNetworkData.playerState = 2; //Fantasma
            
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
                if(miniMapUI) miniMapUI.SetActive(true);
                movement.LockToViewDirection = true;
                break;
            case 2:
                if(firstPersonCamera) firstPersonCamera.SetActive(false);
                if(thirdPersonCamera) thirdPersonCamera.SetActive(true);
                if(miniMapUI) miniMapUI.SetActive(false);
                movement.LockToViewDirection = false;
                break;
            case 3:
                Debug.Log($"Jugador <color=blue>{playerNetworkData.nickName}</color> perdio");
                if(firstPersonCamera) firstPersonCamera.SetActive(false);
                if(thirdPersonCamera) thirdPersonCamera.SetActive(true);
                if(miniMapUI) miniMapUI.SetActive(false);
                
                movement.LockToViewDirection = false;
                propTransformer.BackToOriginalForm();
                
                break;
        }
    }

    public void SetState(byte value)
    {
        playerNetworkData.playerState = value;
        
        CheckState(playerNetworkData.playerState);
    }

    public byte GetState()
    {
        return playerNetworkData.playerState;
    }

    public int GetActorNumber()
    {
        return playerNetworkData.actorNumber;
    }

    public string GetNickName()
    {
        return playerNetworkData.nickName;
    }
}
