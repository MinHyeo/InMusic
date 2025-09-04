using UnityEngine;
using SSW;

public class Guide_UI : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Input.SetUIKeyEvent(GuideKeyEvent);
    }

    public void GuideButton(string buttonname){
        if (buttonname == "Exit") {
            GameManager.Input.RemoveUIKeyEvent(GuideKeyEvent);
            GlobalInputControl.CurrentInputMode = InputMode.GamePlay;
            Destroy(gameObject);
        }
    }

    public void GuideKeyEvent(Define.UIControl keyEvent) {
        if (keyEvent == Define.UIControl.Esc || keyEvent == Define.UIControl.Enter) GuideButton("Exit");
    }
}
