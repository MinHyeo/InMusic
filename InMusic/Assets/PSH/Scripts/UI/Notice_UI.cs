using UnityEditor;
using UnityEngine;

public class Notice_UI : MonoBehaviour
{
    private void Awake()
    {
        GameManager_PSH.Input.SetUIKeyEvent(MessageKeyEvent);
    }

    public void MessageButton(string buttonname)
    {
        if (buttonname == "Exit")
            Destroy(gameObject);
    }

    public void MessageKeyEvent(Define_PSH.UIControl keyEvent)
    {
        switch (keyEvent)
        {
            case Define_PSH.UIControl.Esc:
            case Define_PSH.UIControl.Enter:
                MessageButton("Exit");
                break;
        }
    }
}
