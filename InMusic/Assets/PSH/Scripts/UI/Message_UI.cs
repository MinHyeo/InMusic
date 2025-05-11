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
            //EditorApplication.isPlaying = false; //에디터용 (빌드전에 지우기)
            Application.Quit();//인게임용
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
