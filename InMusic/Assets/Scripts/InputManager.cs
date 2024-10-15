using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    //private Action<> keyPress;
    //키 입력 시 해당 키의 입력을 기다리는 함수들 모두 호출
    void Update()
    {
        //LeftArrow
        if (Input.GetKeyDown(KeyCode.LeftArrow)) 
        {
            //keyPress.Invoke();
        }
        //RightArrow
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //keyPress.Invoke();
        }
        //UpArrow
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
           //keyPress.Invoke();
        }
        //DownArrow
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //keyPress.Invoke();
        }
    }
}
