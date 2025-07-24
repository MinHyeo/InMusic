using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SessionListEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private SessionInfo sessionInfo;

    [SerializeField]
    private Text roomName, playerCount;
    [SerializeField]
    private GameObject lockIcon, selectImage;
    [SerializeField]
    private bool isLocked = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Highlight the entry when hovered over
        selectImage.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Reset the highlight when not hovered over
        selectImage.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Handle the click event
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (isLocked)
            {
                LobbyManager.Instance.JoinLockedRoom(sessionInfo);
            }
            else
            {
                // Join the room when clicked
                LobbyManager.Instance.TryJoinRoom(roomName.text);
            }

        }
    }

    public void CreateRoom(SessionInfo session)
    {
        this.sessionInfo = session;
        roomName.text = sessionInfo.Name;
        playerCount.text = sessionInfo.PlayerCount.ToString() + "/" + sessionInfo.MaxPlayers.ToString();

        isLocked = false;
        if (sessionInfo.Properties.TryGetValue("isLocked", out var lockProp))
        {
            isLocked = (bool)lockProp;
        }
        lockIcon.SetActive(isLocked);
    }

    public void UpdateRoom(SessionInfo session)
    {
        sessionInfo = session;
        roomName.text = sessionInfo.Name;
        playerCount.text = sessionInfo.PlayerCount.ToString() + "/" + sessionInfo.MaxPlayers.ToString();
    }
}
