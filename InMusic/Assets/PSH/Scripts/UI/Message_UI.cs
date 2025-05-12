using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

public class Message_UI : MonoBehaviour
{
    private void Awake()
    {
        GameManager_PSH.Input.SetUIKeyEvent(MessageKeyEvent);
    }

    public void MessageButton(string buttonname) {
        if (buttonname == "Exit") {
            //EditorApplication.isPlaying = false; //�����Ϳ� (�������� �����)
            Application.Quit();//�ΰ��ӿ�
        }
        else if (buttonname == "Cancle") {
            GameManager_PSH.Input.RemoveUIKeyEvent(MessageKeyEvent);
            Destroy(gameObject);
        }
    }

    public void MessageKeyEvent(Define_PSH.UIControl keyEvent) {
        switch (keyEvent) {
            case Define_PSH.UIControl.Esc:
                MessageButton("Cancle");
                break;
            case Define_PSH.UIControl.Enter:
                MessageButton("Exit");
                break;
        }
    }
}
