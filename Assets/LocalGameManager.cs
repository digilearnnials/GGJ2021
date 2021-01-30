using Photon.Pun;
using UnityEngine;

public class LocalGameManager : MonoBehaviour
{
    enum GameState
    {
        GAMESTART,
        GAMEPLAY,
        GAMEEND
    }

    [SerializeField] private GameState gameState = GameState.GAMESTART;
    [SerializeField] private int maxPlayersInRoom = 0;
    [SerializeField] private int curPlayersInRoom = 0;
    [SerializeField, Range(60, 300)] private int matchDurationInSeconds = 180;

    private PlayerDataConection[] players = default;

    private int localPlayerID = 0;

    private MatchTimer matchTimer = default;
    
    public void Start()
    {
        matchTimer = FindObjectOfType<MatchTimer>();
        
        matchTimer.onTimerEnd.AddListener(OnTimerEnd);
        
        matchTimer.gameObject.SetActive(false);
        
        maxPlayersInRoom = PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    private void LateUpdate()
    {
        curPlayersInRoom = PhotonNetwork.CurrentRoom.PlayerCount;
        switch (gameState)
        {
            case GameState.GAMESTART:
                ScreenMessage.instance.ShowMessage($"Waiting Players... {curPlayersInRoom}/{maxPlayersInRoom}");
                if(curPlayersInRoom < maxPlayersInRoom) return;
                
                players = FindObjectsOfType<PlayerDataConection>();
                localPlayerID = PhotonNetwork.LocalPlayer.ActorNumber;
                
                ScreenMessage.instance.HideMessage();
                
                ScreenMessage.instance.ShowMessage($"MATCH READY!!!", 3);
                
                matchTimer.gameObject.SetActive(true);
                
                matchTimer.StartTimer(matchDurationInSeconds);
                
                ScreenMessage.instance.HideMessage();
                
                gameState = GameState.GAMEPLAY;
                break;
        }
    }

    private void OnTimerEnd()
    {
        int sprite = 0;
        string finalText = "TIME IS UP!!!";

        foreach (var player in players)
        {
            switch (player.GetState())
            {
                case 1:
                    if (player.GetActorNumber() == localPlayerID) sprite = 10;
                    finalText += $"\n <b>{player.GetNickName()}</b>: <color=red>He failed to catch any ghosts</color>";
                    break;
                case 2:
                    if (player.GetActorNumber() == localPlayerID) sprite = 15;
                    finalText += $"\n <b>{player.GetNickName()}</b>: <color=red>Could not escape</color>";
                    break;
                case 3:
                    if (player.GetActorNumber() == localPlayerID) sprite = 10;
                    finalText += $"\n <b>{player.GetActorNumber()}</b>: <color=red>He was captured</color>";
                    break;
                case 4:
                    if (player.GetActorNumber() == localPlayerID) sprite = 3;
                    finalText += $"\n <b>{player.GetNickName()}</b>: <color=green>Escaped!!!</color>";
                    break;
            }
        }
        
        ScreenMessage.instance.ShowMessage(finalText, sprite);
        
        gameState = GameState.GAMEEND;
    }

    void CheckAllPlayerState()
    {
        
    }
}
