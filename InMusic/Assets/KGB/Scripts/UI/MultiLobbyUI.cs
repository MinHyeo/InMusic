using UnityEngine;
using UnityEngine.UI;

public class MultiLobbyUI : MonoBehaviour
{
    public Button createButton;
    public GameObject roomCreateUI;
    public GameObject roomJoinUI;
    public void OnClickCreateButton()
    {
        roomCreateUI.SetActive(true);
    }
}
