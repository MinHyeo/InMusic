using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using SongList;
using UI_BASE_PSH;
using Unity.XR.Oculus.Input;


public class SingleMenuController : MonoBehaviour
{
    private void OnEnable() {
        GameManager.SingleMenuInput.keyAction += OnKeyPress;
        Debug.Log("SingleMenu Input Enabled");
    }

    private void OnDisable() {
        GameManager.SingleMenuInput.keyAction -= OnKeyPress;
        Debug.Log("SingleMenu Input Enabled");
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
        //TODO: Option UI 표현
        

        Debug.Log("Option Button Clicked.");
    }
}
