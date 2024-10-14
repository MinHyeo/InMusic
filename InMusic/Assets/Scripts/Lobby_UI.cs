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


    //��ư ���
    public void Button(string Type) {
        switch (Type) {
            //����
            case "Gear":
                //Todo
                Debug.Log("Gear function is not implemented");
                break;
            //������
            case "Exit":
                Debug.Log("Exit function is not implemented");
                break;
            //����
            case "Left":
                SetMode();
                //Animation Control

                break;
            //������
            case "Right":
                SetMode();
                //Animation Control

                break;
            //�ַ�
            case "Solo":
                //Todo
                Debug.Log("Solo function is not implemented");
                break;
            //��Ƽ
            case "Multi":
                //Todo
                Debug.Log("Multi function is not implemented");
                break;
            case "KeyGuide":
                //Todo
                Debug.Log("KeyGuide function is not implemented");
                break;
            default:
                Debug.Log("���� ����� ���ų� �߸� �Է�");
                break;
        }
    }
    public void SetMode() {
        Solo.gameObject.SetActive(!Solo.gameObject.activeSelf);
        Multi.gameObject.SetActive(!Solo.gameObject.activeSelf);
    }
}
