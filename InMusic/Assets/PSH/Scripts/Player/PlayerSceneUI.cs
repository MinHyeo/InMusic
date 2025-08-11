using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerSceneUI : MonoBehaviour, IPointerClickHandler
{
    [Header("플레이어 프리팹")]
    [SerializeField] public GameObject pNetworkObject;
    [SerializeField] private int uIID;
    [Header("플레이어 메뉴 UI")]
    [SerializeField] GameObject playerMenuUI;

    void Update()
    {
        // 팝업 메뉴가 활성화된 상태에서만 작동
        if (playerMenuUI.activeSelf)
        {
            // 마우스 좌클릭이 발생했을 때
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    playerMenuUI.SetActive(false);
                }
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
        if (pNetworkObject.GetComponent<PlayerInfo>().IsOwner) {
            return;
        }
        //우클릭 감지
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            playerMenuUI.SetActive(true); //활성화
            playerMenuUI.GetComponent<RectTransform>().position = eventData.position; //위치 이동
            playerMenuUI.GetComponent<PlayerMenuUI>().SetTarget(pNetworkObject.GetComponent<PlayerInfo>());
        }
    }
}
