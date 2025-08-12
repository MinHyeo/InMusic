using TMPro;
using UnityEngine;

public class Player_Result : MonoBehaviour
{
    [Header("플레이어 정보 UI")]
    [SerializeField] TextMeshProUGUI[] playerNames;
    [SerializeField] GameObject[] winnerMark;

    public void SetP1Name(string nickName) {
        playerNames[0].text = nickName;
    }

    public void SetP2Name(string nickName) {
        playerNames[1].text = nickName;
    }
    public void SetP1Win() {
        winnerMark[0].SetActive(true);
    }

    public void SetP2Win() {
        winnerMark[1].SetActive(true);
    }
}
