using UnityEngine;
using Photon.Pun;
using TMPro;

namespace GGJ2021.View.UI
{
    /// <summary>
    /// Player name input field. Lets the user input his name.
    /// </summary>
    [RequireComponent(typeof(TMP_InputField))]
    public class PlayerNameInputFieldView : MonoBehaviour
    {
        // Store the PlayerPref Key to avoid typos
        const string playerNamePrefKey = "PlayerName";

        private TMP_InputField inputField;

        void Start()
        {
            string defaultName = string.Empty;
            inputField = GetComponent<TMP_InputField>();
            if (inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    inputField.text = defaultName;
                    Debug.Log("<color=yellow>Player Name retrieved!</color>");
                }
            }
            PhotonNetwork.NickName = defaultName;
        }

        /// <summary>
        /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
        /// </summary>
        /// <param name="value">The name of the Player</param>
        public void SetPlayerName()
        {
            string value = inputField.text;
            // #Important
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }
            PhotonNetwork.NickName = value;
            PlayerPrefs.SetString(playerNamePrefKey, value);
            Debug.Log("<color=yellow>Player Name set!</color>");
        }
    }
}
