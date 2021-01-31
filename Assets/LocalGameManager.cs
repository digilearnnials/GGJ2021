using System.Collections;
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

    [SerializeField] private PlayerDataConection[] players = default;

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
                
                localPlayerID = PhotonNetwork.LocalPlayer.ActorNumber;
                
                ScreenMessage.instance.HideMessage();
                
                ScreenMessage.instance.ShowMessage($"MATCH READY!!!", 3);
                
                matchTimer.gameObject.SetActive(true);
                
                matchTimer.StartTimer(matchDurationInSeconds);
                
                ScreenMessage.instance.HideMessage();

                gameState = GameState.GAMEPLAY;
                
                players = FindObjectsOfType<PlayerDataConection>();
                foreach (var player in players)
                {
                    player.ResetToStartPos();
                }

                break;
        }
    }

    private void OnTimerEnd()
    {
        players = FindObjectsOfType<PlayerDataConection>();
        
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

    void OnAllScape()
    {
        players = FindObjectsOfType<PlayerDataConection>(true);
        
        int sprite = 0;
        string finalText = "THE GHOSTS ESCAPED!!";
        
        foreach (var player in players)
        {
            switch (player.GetState())
            {
                case 1:
                    if (player.GetActorNumber() == localPlayerID) sprite = 10;
                    finalText += $"\n <b>{player.GetNickName()}</b>: <color=red>He failed to catch any ghosts</color>";
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
    
    void OnAllCaptured()
    {
        players = FindObjectsOfType<PlayerDataConection>();
        
        int sprite = 0;
        string finalText = "ALL THE GHOSTS WERE CAPTURED!!";
        
        foreach (var player in players)
        {
            switch (player.GetState())
            {
                case 1:
                    if (player.GetActorNumber() == localPlayerID) sprite = 3;
                    finalText += $"\n <b>{player.GetNickName()}</b>: <color=green>Has captured all the ghosts</color>";
                    break;
                case 3:
                    if (player.GetActorNumber() == localPlayerID) sprite = 10;
                    finalText += $"\n <b>{player.GetNickName()}</b>: <color=red>He was captured</color>";
                    break;
            }
        }
        
        ScreenMessage.instance.ShowMessage(finalText, sprite);
        
        gameState = GameState.GAMEEND;
    }
    
    private void OnUnespectedEndMatch(string text, bool witchWin)
    {
        players = FindObjectsOfType<PlayerDataConection>(true);
        
        int sprite = 0;
        string finalText = text;

        foreach (var player in players)
        {
            switch (player.GetState())
            {
                case 1:
                    if (witchWin)
                    {
                        if (player.GetActorNumber() == localPlayerID) sprite = 9;
                        finalText += $"\n <b>{player.GetNickName()}</b>: <color=green>Won by most catches</color>";
                    }
                    else
                    {
                        if (player.GetActorNumber() == localPlayerID) sprite = 15;
                        finalText += $"\n <b>{player.GetNickName()}</b>: <color=red>They got away a lot and lost</color>";
                    }
                    break;
                case 3:
                    if (player.GetActorNumber() == localPlayerID) sprite = 10;
                    finalText += $"\n <b>{player.GetActorNumber()}</b>: <color=red>He was captured</color>";
                    break;
                case 4:
                    if (player.GetActorNumber() == localPlayerID) sprite = 3;
                    finalText += $"\n <b>{player.GetNickName()}</b>: <color=green>I escape but it wasn't enough</color>";
                    break;
            }
        }
        
        ScreenMessage.instance.ShowMessage(finalText, sprite);
        
        gameState = GameState.GAMEEND;
    }
    

    public void CheckAllPlayerState()
    {
        players = FindObjectsOfType<PlayerDataConection>();
        
        int captureAmount = 0;
        int scapedAmount = 0;
        
        foreach (var player in players)
        {
            switch (player.GetState())
            {
                case 3:
                    captureAmount++;
                    break;
                case 4:
                    scapedAmount++;
                    break;
            }
        }

        if (scapedAmount >= maxPlayersInRoom - 1)
        {
            // All scape
            matchTimer.StopTimer();
            OnAllScape();
            return;
        }
        
        if (captureAmount >= maxPlayersInRoom - 1)
        {
            // All capture
            matchTimer.StopTimer();
            OnAllCaptured();
            return;
        }

        if (scapedAmount + captureAmount >= maxPlayersInRoom - 1)
        {
            matchTimer.StopTimer();
            if (scapedAmount > captureAmount)
            {
                OnUnespectedEndMatch("Ghosts win by majority", false);
            }
            else
            {
                OnUnespectedEndMatch("The witch captured almost everyone", true);
            }
        }
    }
}
