using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class SessionListEntry : MonoBehaviour
{
    public Text roomName, playerCount;
    public GameObject lockIcon;
    public Button joinButton;

    public void JoinRoom()
    {
        NetworkManager.runnerInstance.StartGame(new StartGameArgs()
        {
            SessionName = roomName.text,
        });
    }
}
