using UnityEditor;
using UnityEngine;

public class Message_UI : MonoBehaviour
{
    public void LobbyButton(string buttonname) {
        if (buttonname == "Exit") {
            EditorApplication.isPlaying = false; //에디터용
            Application.Quit();//인게임용
        }
        else if (buttonname == "Cancle") { 
            Destroy(gameObject);
        }
    }
    public void Multiutton() { 
    
    }
}
