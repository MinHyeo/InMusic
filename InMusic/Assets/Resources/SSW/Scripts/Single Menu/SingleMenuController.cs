using UnityEngine;
using UnityEngine.SceneManagement;
using System;


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
        if(Input.GetKeyDown(KeyCode.Space))
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
        //SceneManager.LoadScene("Next_Scene");
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
