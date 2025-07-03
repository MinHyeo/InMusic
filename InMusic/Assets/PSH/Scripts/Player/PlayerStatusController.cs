using TMPro;
using UnityEngine;

public class PlayerStatusController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] playerNames;
    [SerializeField] GameObject[] isRoomOwner;
    [Tooltip("����")] [SerializeField] GameObject[] player1Status;
    [Tooltip("������")] [SerializeField] GameObject[] player2Status;


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
    /// �÷��̾� �غ� ���� ������Ʈ
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
            Debug.Log("�÷��̾� ��ȣ �߸� �Է���");
            return;
        }

        //��� ���� ��
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
    /// �÷��̾� �̸� ����
    /// </summary>
    /// <param name="name"></param>
    public void SetPlayerName(int playerIndex, string name)
    {
        playerNames[playerIndex].text = name;
    }

    /// <summary>
    /// �÷��̾� ǥ��
    /// </summary>
    public void SetPlayerMark(bool isP1) {
           player1Status[2].SetActive(isP1);
           player2Status[2].SetActive(!isP1);
    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    /// <param name="isP1"></param>
    public void SetRoomOwner(bool isP1) {
        isRoomOwner[0].SetActive(isP1);
        isRoomOwner[1].SetActive(!isP1);
    }

}
