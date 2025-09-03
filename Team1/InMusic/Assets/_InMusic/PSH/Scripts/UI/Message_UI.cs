using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Windows;

public class Message_UI : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Input.SetUIKeyEvent(MessageKeyEvent);
    }

    public void MessageButton(string buttonname) {
        if (buttonname == "Exit") {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false; //�����Ϳ�
#endif
            Application.Quit();//�ΰ��ӿ�
        }
        else if (buttonname == "Cancle") {
            GameManager.Input.RemoveUIKeyEvent(MessageKeyEvent);
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
