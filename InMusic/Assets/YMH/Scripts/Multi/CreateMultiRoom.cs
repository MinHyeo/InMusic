using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;
public class CreateMultiRoom : MonoBehaviour
{
    [SerializeField]
    private InputField roomNameInputField;
    [SerializeField]
    private InputField passWordInputField;
    [SerializeField]
    private Toggle isPasswordToggle;

    public void Initialized()
    {
    }

    public void Create(){
        string roomName = roomNameInputField.text;
        string password = passWordInputField.text;
        bool isPassword = isPasswordToggle.isOn;

        Debug.Log("Room Name: " + roomName);
        Debug.Log("Password: " + password);
        NetworkManager.Instance.CreateRoom(roomName, password, isPassword);
    }

    public void Cancel(){

    }

    public void IsCheckPassword(){
        bool isCheck = isPasswordToggle.isOn;
        passWordInputField.interactable = isCheck;
        passWordInputField.text = "";
    }
}