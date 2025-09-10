using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using SongList;
using UI_BASE_PSH;
using SSW;
using Play;


public class SingleMenuController : MonoBehaviour
{
    [SerializeField] public GameObject curSetUI = null;
    [SerializeField] public GameObject guideUI = null;
    Key_Setting_UI _key_Setting_UI;
    private void OnEnable() {
        GameManager.SingleMenuInput.keyAction += OnKeyPress;
        Debug.Log("SingleMenu Input Enabled");
    }

    private void OnDisable() {
        GameManager.SingleMenuInput.keyAction -= OnKeyPress;
        Debug.Log("SingleMenu Input Disabled");
    }
    private void OnKeyPress()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            OnClickStart();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnClickBack();
        }
        if(Input.GetKeyDown(KeyCode.O))
        {
            GlobalInputControl.CurrentInputMode = InputMode.UI;
            OnClickOption();
        }
        if(Input.GetKeyDown(KeyCode.F1))
        {
            GlobalInputControl.CurrentInputMode = InputMode.UI;
            OnClickGuide();
        }
    }
    // 전체 기능 구현 완료 후 Manager를 추가하여 수정 예정
    public void OnClickStart()
    {
        Play.SoundManager.Instance.PlayUISound("Click");
        HighlightSong highlightSong = FindFirstObjectByType<HighlightSong>();
        highlightSong.StartButtonAction();
        Debug.Log("Start Button Action");
    }

    public void OnClickBack()
    {
        if (NetworkManager.runnerInstance != null && NetworkManager.runnerInstance.IsRunning)
        {
            Debug.Log("[SingleMenuController] Shutting down network runner...");
            NetworkManager.runnerInstance.Shutdown();

            // 셧다운 완료까지 잠시 대기 후 씬 전환
            Invoke(nameof(LoadMainScene), 0.3f);
        }
        else
        {
            Debug.Log("[SingleMenuController] No active network runner found.");
            LoadMainScene();
        }
        Debug.Log("Back Button Clicked");
    }
    private void LoadMainScene()
    {
        Debug.Log("[SingleMenuController] Loading main lobby scene.");

        if (MultiRoomManager.Instance != null)
        {
            Debug.Log("[SingleMenuController] Cleaning up MultiRoomManager before exiting.");
            MultiRoomManager.Instance.DestroyRoomManager();
        }
        SoundManager.Instance.End();
        SceneManager.LoadScene("MainLobbyScene_InMusic");
    }

    public void OnClickOption()
    {
        //TODO: Option UI 입출력 이벤트 처리
        if (curSetUI == null)
        {
            curSetUI = GameManager.Resource.Instantiate("SoundSetting_UI");
        }
        Debug.Log("Option Button Clicked.");
    }

    public void OnClickGuide()
    {
        if(guideUI == null)
        {
            guideUI = GameManager.Resource.Instantiate("KeyGuide_UI");
        }
        Debug.Log("Key Setting Button Clicked.");
    }
}
