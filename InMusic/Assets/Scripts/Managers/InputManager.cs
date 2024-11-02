using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    [Tooltip("Observer ������ �ϴ� �Լ��� �� ���� �Ҵ�")]
    public Action<Define.UIControl> keyPress;
    //Ű �Է� �� �ش� Ű�� �Է��� ��ٸ��� �Լ��� ��� ȣ��
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
