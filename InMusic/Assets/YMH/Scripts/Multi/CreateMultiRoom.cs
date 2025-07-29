using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
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

    public async void OnCreateButton(){
        string roomName = roomNameInputField.text;
        string password = passWordInputField.text;
        bool isPassword = isPasswordToggle.isOn;

        MultiRoomManager.Instance.SetRoomName(roomName);
        await NetworkManager.Instance.CreateRoom(roomName, password, isPassword);
    }

    public void OnCancelButton(){
        roomNameInputField.text = "";
        passWordInputField.text = "";
        isPasswordToggle.isOn = false;

        gameObject.SetActive(false);
    }

    public void IsCheckPassword(){
        bool isCheck = isPasswordToggle.isOn;
        passWordInputField.interactable = isCheck;
        passWordInputField.text = "";
    }
}