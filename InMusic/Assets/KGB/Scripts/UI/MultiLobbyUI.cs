using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiLobbyUI : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;

    public List<SessionListEntry> sessionListEntries = new List<SessionListEntry>();
    public int curSelectIndex = 0;
    [SerializeField] GameObject roomCreateUI;
    private void OnEnable()
    {
        if (networkManager != null)
        {
            networkManager.OnSessionListChanged += RefreshSessionList;
            RefreshSessionList(); // 초기 1회
        }
    }

    private void OnDisable()
    {
        if (networkManager != null)
        {
            networkManager.OnSessionListChanged -= RefreshSessionList;
        }
    }

    private void RefreshSessionList()
    {
        sessionListEntries = networkManager.GetSessionEntries();
        curSelectIndex = 0;
        UpdateSelection();
    }

    private void Update()
    {
        int count = sessionListEntries.Count;
        if (count == 0) return;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            curSelectIndex = (curSelectIndex + 1) % count;
            UpdateSelection();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            curSelectIndex = (curSelectIndex - 1 + count) % count;
            UpdateSelection();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            if (curSelectIndex >= 0 && curSelectIndex < sessionListEntries.Count)
            {
                sessionListEntries[curSelectIndex].JoinRoom();
            }
        }
    }

    private void UpdateSelection()
    {
        for (int i = 0; i < sessionListEntries.Count; i++)
        {
            sessionListEntries[i].isSelected = (i == curSelectIndex);

            if (sessionListEntries[i].curImage != null)
            {
                sessionListEntries[i].curImage.sprite = sessionListEntries[i].isSelected
                    ? sessionListEntries[i].selectedImage
                    : sessionListEntries[i].defaultImage;
            }
        }

        Debug.Log("현재 선택 인덱스: " + curSelectIndex);
    }

    public void OnClickCreateRoomButton()
    {
        roomCreateUI.SetActive(true);
    }

    public void OnClickBackButton()
    {
        SceneManager.LoadScene("MainScene 1");
    }
}
