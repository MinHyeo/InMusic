using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    //private Action<> keyPress;
    //Ű �Է� �� �ش� Ű�� �Է��� ��ٸ��� �Լ��� ��� ȣ��
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
