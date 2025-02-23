using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using SongList;
using UI_BASE_PSH;


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
            OnClickOption();
        }
        if(Input.GetKeyDown(KeyCode.F1))
        {
            if(guideUI == null)
            {
                
            }
        }
    }
    // 전체 기능 구현 완료 후 Manager를 추가하여 수정 예정
    public void OnClickStart()
    {
        HighlightSong highlightSong = FindFirstObjectByType<HighlightSong>();
        highlightSong.StartButtonAction();
        Debug.Log("Start Button Action");
    }

    public void OnClickBack()
    {
        Debug.Log("Back Button Clicked");
        SceneManager.LoadScene("Main_Lobby_PSH");
    }

    public void OnClickOption()
    {
        //TODO: Option UI 입출력 이벤트 처리
        if(curSetUI == null)
        {

        }
        Debug.Log("Option Button Clicked.");
    }
}
