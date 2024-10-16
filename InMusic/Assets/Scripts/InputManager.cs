using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    [Tooltip("Observer 역할을 하는 함수에 이 변수 할당")]
    public Action<Define.UIControl> keyPress;
    //키 입력 시 해당 키의 입력을 기다리는 함수들 모두 호출
    public void OnUpdate()
    {
        //LeftArrow
        if (Input.GetKeyDown(KeyCode.LeftArrow)) 
        {
            keyPress.Invoke(Define.UIControl.Left);
        }
        //RightArrow
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
           keyPress.Invoke(Define.UIControl.Right);
        }
        //UpArrow
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
           keyPress.Invoke(Define.UIControl.Up);
        }
        //DownArrow
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            keyPress.Invoke(Define.UIControl.Down);
        }
        //Enter
        if (Input.GetKeyDown(KeyCode.Return))
        {
            keyPress.Invoke(Define.UIControl.Enter);
        }
        //ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            keyPress.Invoke(Define.UIControl.Esc);
        }
        //F1: KeyGuide
        if (Input.GetKeyDown(KeyCode.F1))
        {
            keyPress.Invoke(Define.UIControl.Guide);
        }
        //F10: Setting
        if (Input.GetKeyDown(KeyCode.F10))
        {
            keyPress.Invoke(Define.UIControl.Setting);
        }
    }
}
