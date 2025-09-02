using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerStatusUI : MonoBehaviour, IPointerClickHandler
{
    [Header("플레이어 프리팹")]
    [SerializeField] public GameObject pNetworkObject;
    [SerializeField] private int uIID;
    [Header("플레이어 메뉴 UI")]
    [SerializeField] GameObject playerMenuUI;

    void Update()
    {
        if (!playerMenuUI.activeSelf) return;

        // 마우스 좌클릭이 발생했을 때
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

    //마우스 감지
    public void OnPointerClick(PointerEventData eventData)
    {
        //본인 선택 OR 방장 권한 없으면 무시
        if ((uIID == 1 && GameManager_PSH.PlayerRole) || ((uIID == 2 && !GameManager_PSH.PlayerRole)) 
            || pNetworkObject.GetComponent<PlayerInfo>().IsOwner) {
            return;
        }
        //우클릭 감지
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            playerMenuUI.SetActive(true); //활성화
            playerMenuUI.GetComponent<RectTransform>().position = eventData.position; //위치 이동
            playerMenuUI.GetComponent<PlayerMenuUI>().SetTarget(uIID);
        }
    }
}
