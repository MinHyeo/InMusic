using UnityEditor;
using UnityEngine;

public class Message_UI : MonoBehaviour
{
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
                MessageButton("Exit");
                break;
            case Define.UIControl.Enter:
                MessageButton("Cancle");
                break;
        }
    }
}
