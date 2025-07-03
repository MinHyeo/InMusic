using TMPro;
using UnityEngine;

public class PlayerStatusController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] playerNames;
    [SerializeField] GameObject[] isRoomOwner;
    [Tooltip("방장")] [SerializeField] GameObject[] player1Status;
    [Tooltip("참가자")] [SerializeField] GameObject[] player2Status;


    private void Awake()
    {
        InitP2Status();
    }
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

    /// <summary>
    /// 플레이어 준비 상태 업데이트
    /// </summary>
    /// <param name="playerIndex">0: P1 1: P2</param>
    /// <param name="isReady"></param>
    /// <param name="hasAuthority"></param>
    public void SetPlayerStatus(int playerIndex, bool isReady, bool hasAuthority)
    {
        GameObject[] targetStatusUI;
        if (playerIndex == 0)
        {
            targetStatusUI = player1Status;
        }
        else if (playerIndex == 1)
        {
            targetStatusUI = player2Status;
        }
        else
        {
            Debug.Log("플레이어 번호 잘못 입력함");
            return;
        }

        //모든 상태 끔
        targetStatusUI[0].SetActive(false);
        targetStatusUI[1].SetActive(false);

        if (isReady)
        {
            targetStatusUI[1].SetActive(true);
        }
        else
        {
            if (hasAuthority) {
                targetStatusUI[0].SetActive(true);
            }
        }
    }

    /// <summary>
    /// 플레이어 이름 설정
    /// </summary>
    /// <param name="name"></param>
    public void SetPlayerName(int playerIndex, string name)
    {
        playerNames[playerIndex].text = name;
    }

    /// <summary>
    /// 플레이어 표시
    /// </summary>
    public void SetPlayerMark(bool isP1) {
           player1Status[2].SetActive(isP1);
           player2Status[2].SetActive(!isP1);
    }

    /// <summary>
    /// 방장 권한 병경
    /// </summary>
    /// <param name="isP1"></param>
    public void SetRoomOwner(bool isP1) {
        isRoomOwner[0].SetActive(isP1);
        isRoomOwner[1].SetActive(!isP1);
    }

}
