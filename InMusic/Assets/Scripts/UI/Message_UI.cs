using UnityEditor;
using UnityEngine;

public class Message_UI : MonoBehaviour
{
    public void MessageButton(string buttonname) {
        if (buttonname == "Exit") {
            EditorApplication.isPlaying = false; //에디터용
            Application.Quit();//인게임용
        }
        else if (buttonname == "Cancle") {
            //Remove KeyEvent
            Destroy(gameObject);
        }
    }
    public void MessageKeyEvent(Define.UIControl keyEvent) {
        switch (keyEvent) {
            case Define.UIControl.Esc:
                MessageButton("Exit");
                break;
            case Define.UIControl.Enter:
                MessageButton("Cancle");
                break;
        }
    }
}
