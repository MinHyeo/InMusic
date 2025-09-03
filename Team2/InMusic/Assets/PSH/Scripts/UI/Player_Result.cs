using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Player_Result : MonoBehaviour 
{
    [Header("플레이어 정보 UI")]
    [SerializeField] TextMeshProUGUI[] playerNames;
    [SerializeField] Image[] playerUI;
    [SerializeField] GameObject[] winnerMark;
    [SerializeField] Sprite P1Selected;
    [SerializeField] Sprite P1UnSelected;
    [SerializeField] Sprite P2Selected;
    [SerializeField] Sprite P2UnSelected;

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

    public void SelectP1() {
        playerUI[0].sprite = P1Selected;
        playerUI[1].sprite = P2UnSelected;
    }

    public void SelectP2() {
        playerUI[0].sprite = P1UnSelected;
        playerUI[1].sprite = P2Selected;
    }
}
