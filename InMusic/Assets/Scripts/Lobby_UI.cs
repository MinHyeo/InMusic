using UnityEngine;
using UnityEngine.UI;

public class Lobby_UI : MonoBehaviour
{
    [Header("UI Button")]
    public Button Gear;
    public Button Exit;
    public Button Solo;
    public Button Multi;
    public Button Left_Arrow;
    public Button Right_Arrow;
    public Button Key_Guide;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Solo.gameObject.SetActive(true);
        Multi.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //버튼 기능
    public void Button(string Type) {
        switch (Type) {
            //설정
            case "Gear":
                //Todo
                Debug.Log("Gear function is not implemented");
                break;
            //나가기
            case "Exit":
                Debug.Log("Exit function is not implemented");
                break;
            //왼쪽
            case "Left":
                SetMode();
                //Animation Control

                break;
            //오른쪽
            case "Right":
                SetMode();
                //Animation Control

                break;
            //솔로
            case "Solo":
                //Todo
                Debug.Log("Solo function is not implemented");
                break;
            //멀티
            case "Multi":
                //Todo
                Debug.Log("Multi function is not implemented");
                break;
            case "KeyGuide":
                //Todo
                Debug.Log("KeyGuide function is not implemented");
                break;
            default:
                Debug.Log("아직 기능이 없거나 잘못 입력");
                break;
        }
    }
    public void SetMode() {
        Solo.gameObject.SetActive(!Solo.gameObject.activeSelf);
        Multi.gameObject.SetActive(!Solo.gameObject.activeSelf);
    }
}
