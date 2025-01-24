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

    public void MessageKeyEvent(Define.UIControl keyEvent)
    {
        switch (keyEvent)
        {
            case Define.UIControl.Esc:
            case Define.UIControl.Enter:
                MessageButton("Exit");
                break;
        }
    }
}
