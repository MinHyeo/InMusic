using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerStatusUI : MonoBehaviour, IPointerClickHandler
{
    [Header("�÷��̾� ������")]
    [SerializeField] public GameObject pNetworkObject;
    [SerializeField] private int uIID;
    [Header("�÷��̾� �޴� UI")]
    [SerializeField] GameObject playerMenuUI;

    void Update()
    {
        if (!playerMenuUI.activeSelf) return;

        // ���콺 ��Ŭ���� �߻����� ��
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                playerMenuUI.SetActive(false);
            }
        }
    }

    public void SetPlayerGameObject(GameObject playerPrefab)
    {
        pNetworkObject = playerPrefab;
    }

    public void SetPlayerRole() {
        PlayerInfo info = pNetworkObject.GetComponent<PlayerInfo>();
        info.InitReady();
        info.IsOwner = true;
    }

    //���콺 ����
    public void OnPointerClick(PointerEventData eventData)
    {
        if (pNetworkObject.GetComponent<PlayerInfo>().IsOwner) {
            return;
        }
        //��Ŭ�� ����
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            playerMenuUI.SetActive(true); //Ȱ��ȭ
            playerMenuUI.GetComponent<RectTransform>().position = eventData.position; //��ġ �̵�
            playerMenuUI.GetComponent<PlayerMenuUI>().SetTarget(uIID);
        }
    }
}
