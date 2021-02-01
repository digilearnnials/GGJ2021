using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;

namespace GGJ2021.Networking
{
    public class PhotonLauncher : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 8;
        [Tooltip("The Ui Panel to let the user know that the game is loading")]
        [SerializeField]
        private GameObject initScreen;
        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject mainMenu;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject loadingScreen;

        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        private string gameVersion = "1";
        /// <summary>
        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
        /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        /// Typically this is used for the OnConnectedToMaster() callback.
        /// </summary>
        private bool isConnecting = false;

        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        void Start()
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            if (!PhotonNetwork.IsConnected)
            {
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            loadingScreen.SetActive(true);
            mainMenu.SetActive(false);

            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public void Quit()
        {
            PhotonNetwork.Disconnect();
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
                EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("<color=yellow>OnConnectedToMaster() was called by PUN</color>");
            isConnecting = false;
            initScreen.SetActive(false);
            mainMenu.SetActive(true);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            loadingScreen.SetActive(false);
            initScreen.SetActive(false);
            mainMenu.SetActive(true);
            Debug.LogWarningFormat("<color=yellow>OnDisconnected() was called by PUN with reason {0}</color>", cause);
            isConnecting = false;
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("<color=yellow>OnJoinRandomFailed() was called by PUN. No random room available\nCalling: PhotonNetwork.CreateRoom</color>");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("<color=yellow>OnJoinedRoom() called by PUN. Now this client is in a room.</color>");
            PhotonNetwork.LoadLevel("GameRoom");
        }
    }
}
