using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace Com.MyCompany.MyGame
{
    public class GameRoomManager : MonoBehaviourPunCallbacks
    {
        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

        void Start()
        {
            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Room Manager'", this);
            }
            else
            {
                Debug.LogFormat("We are Instantiating LocalPlayer in {0}", SceneManager.GetActiveScene().name);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0f, 1f, 0f), Quaternion.identity, 0);
            }
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("MultiplayerLobby");
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("<color=yellow>Player connected: {0}</color>", other.NickName); // not seen if you're the player connecting
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("<color=yellow>Player dropped: {0}</color>", other.NickName); // seen when other disconnects
        }
    }
}