using UnityEngine;

public class Guide_UI : MonoBehaviour
{
    public void GuideButton(string buttonname){
        if (buttonname == "Exit") {
            Destroy(gameObject);
        }
    }

    public void GuideKeyEvent(Define.UIControl keyEvent) {
        if (keyEvent == Define.UIControl.Esc) GuideButton("Exit");
    }
}
