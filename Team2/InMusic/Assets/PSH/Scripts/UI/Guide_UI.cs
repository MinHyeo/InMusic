using UnityEngine;

public class Guide_UI : MonoBehaviour
{
    private void Awake()
    {
        GameManager_PSH.Input.SetUIKeyEvent(GuideKeyEvent);
    }

    public void GuideButton(string buttonname){
        if (buttonname == "Exit") {
            GameManager_PSH.Input.RemoveUIKeyEvent(GuideKeyEvent);
            Destroy(gameObject);
        }
    }

    public void GuideKeyEvent(Define_PSH.UIControl keyEvent) {
        if (keyEvent == Define_PSH.UIControl.Esc) GuideButton("Exit");
    }
}
