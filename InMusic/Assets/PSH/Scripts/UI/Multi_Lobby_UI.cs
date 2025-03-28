using UnityEngine;
using UI_BASE_PSH;

public class Multi_Lobby_UI : UI_Base_PSH
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Button(string buttontype)
    {
        switch (buttontype) {
            case "Gear":
                Gear();
                break;
            case "Guide":
                Guide();
                break;
            case "Set":
                break;
            case "Create":
                break;
            case "Enter":
                break;
            case "Back":
                break;
        }
    }
}
