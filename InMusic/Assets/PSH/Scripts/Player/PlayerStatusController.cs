using TMPro;
using UnityEngine;

public class PlayerStatusController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] playerNames;
    [SerializeField] GameObject[] isRoomOwner;
    [SerializeField] GameObject[] player1Status;
    [SerializeField] GameObject[] player2Status;

    public void InitP1Status()
    {
        isRoomOwner[0].SetActive(true);
        foreach(GameObject info in player1Status)
        {
            info.SetActive(false);
        }
    }

    public void InitP2Status()
    {
        isRoomOwner[1].SetActive(false);
        foreach (GameObject info in player2Status)
        {
            info.SetActive(false);
        }
        playerNames[1].text = "";
    }

    public bool GetP1Status() {
        return player1Status[1].activeSelf;
    }

    public bool GetP2Status() {
        return player2Status[1].activeSelf;
    }

    public void Setp1Status(bool isReady)
    {
        if (isReady)
        {
            player1Status[0].SetActive(!isReady);
            player1Status[1].SetActive(isReady);
        }
        else
        {
            player1Status[0].SetActive(isReady);
            player1Status[1].SetActive(!isReady);
        }

    }

    public void Setp2Status(bool isReady)
    {
        if (isReady)
        {
            player2Status[0].SetActive(!isReady);
            player2Status[1].SetActive(isReady);
        }
        else
        {
            player2Status[0].SetActive(isReady);
            player2Status[1].SetActive(!isReady);
        }
    }

    public void SetP1Name(string name)
    {
        playerNames[0].text = name;
    }

    public void SetP2Name(string name) 
    {
        playerNames[1].text = name;
    }

    public void SetRoomOwner(bool isP1) {
        isRoomOwner[0].SetActive(isP1);
        isRoomOwner[1].SetActive(!isP1);
    }

}
