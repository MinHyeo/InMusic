using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Windows;

public class Message_UI : MonoBehaviour
{
    private void Awake()
    {
        
    }
    public void MessageButton(string buttonname) {
        if (buttonname == "Exit") {
            EditorApplication.isPlaying = false; //�����Ϳ�
            Application.Quit();//�ΰ��ӿ�
        }
        else if (buttonname == "Cancle") {
            //Remove KeyEvent
            Destroy(gameObject);
        }
    }

    public void MessageKeyEvent(Define.UIControl keyEvent) {
        switch (keyEvent) {
            case Define.UIControl.Esc:
                MessageButton("Cancle");
                break;
            case Define.UIControl.Enter:
                MessageButton("Exit");
                break;
        }
    }
}
