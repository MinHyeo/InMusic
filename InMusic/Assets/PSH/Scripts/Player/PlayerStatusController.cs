using TMPro;
using UnityEngine;

public class PlayerStatusController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] playerNames;
    [SerializeField] GameObject[] isRoomCap;
    [SerializeField] GameObject[] player1Status;
    [SerializeField] GameObject[] player2Status;

    void SetP1Name(string name)
    {
        playerNames[0].text = name;
    }

    void SetP2Name(string name) 
    {
        playerNames[1].text = name;
    }

    void SetP1ToCap() {
        isRoomCap[0].SetActive(true);
        isRoomCap[1].SetActive(false);
    }

    void Setp2ToCap() {
        isRoomCap[0].SetActive(false);
        isRoomCap[1].SetActive(true);
    }

    void Setp1Status(bool isReady) {
        if (isReady) {
            player1Status[0].SetActive(!isReady);
            player1Status[1].SetActive(isReady);
        }
        else
        {
            player1Status[0].SetActive(isReady);
            player1Status[1].SetActive(!isReady);
        }
    
    }

    void Setp2Status(bool isReady) {
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
}
