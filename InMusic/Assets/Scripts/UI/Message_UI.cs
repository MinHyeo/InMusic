using UnityEditor;
using UnityEngine;

public class Message_UI : MonoBehaviour
{
    public void LobbyButton(string buttonname) {
        if (buttonname == "Exit") {
            EditorApplication.isPlaying = false; //�����Ϳ�
            Application.Quit();//�ΰ��ӿ�
        }
        else if (buttonname == "Cancle") { 
            Destroy(gameObject);
        }
    }
    public void Multiutton() { 
    
    }
}
