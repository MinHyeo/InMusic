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
        Debug.Log("[CreateMultiRoom] OnCreateButton called");
        
        string roomName = roomNameInputField.text;
        string password = passWordInputField.text;
        bool isPassword = isPasswordToggle.isOn;

        // NetworkManager와 runnerInstance 존재 확인
        if (NetworkManager.Instance == null)
        {
            Debug.LogError("[CreateMultiRoom] NetworkManager.Instance is null!");
            return;
        }

        if (NetworkManager.runnerInstance == null)
        {
            Debug.LogError("[CreateMultiRoom] NetworkManager.runnerInstance is null!");
            return;
        }
        // 클라우드 서비스가 준비되지 않은 경우 최대 10초동안 대기 - 0.1초 간격으로 체크
        // 준비 전에 세션 생성을 시도하면 오류가 발생하여 NetworkRunner가 중단되는 것을 방지
        // Cloud 연결 상태 확인
        Debug.Log($"[CreateMultiRoom] IsCloudReady: {NetworkManager.runnerInstance.IsCloudReady}");
        Debug.Log($"[CreateMultiRoom] IsRunning: {NetworkManager.runnerInstance.IsRunning}");
        Debug.Log($"[CreateMultiRoom] IsConnectedToServer: {NetworkManager.runnerInstance.IsConnectedToServer}");
        Debug.Log($"[CreateMultiRoom] IsSessionLobbyReady: {NetworkManager._isSessionLobbyReady}");
        
        // Cloud가 준비되고 세션 로비에 연결될 때까지 대기
        if (!NetworkManager.runnerInstance.IsCloudReady || !NetworkManager._isSessionLobbyReady)
        {
            Debug.LogWarning("[CreateMultiRoom] Waiting for cloud connection and session lobby...");
            
            // 최대 10초 대기
            float timeout = 10f;
            float elapsed = 0f;
            
            while ((!NetworkManager.runnerInstance.IsCloudReady || !NetworkManager._isSessionLobbyReady) && elapsed < timeout)
            {
                await Task.Delay(100);
                elapsed += 0.1f;
                Debug.Log($"[CreateMultiRoom] Waiting... {elapsed:F1}s (CloudReady: {NetworkManager.runnerInstance.IsCloudReady}, SessionLobbyReady: {NetworkManager._isSessionLobbyReady})");
            }

            if (!NetworkManager.runnerInstance.IsCloudReady || !NetworkManager._isSessionLobbyReady)
            {
                Debug.LogError("[CreateMultiRoom] Connection timeout!");
                return;
            }
            
            Debug.Log("[CreateMultiRoom] Cloud and session lobby ready! Proceeding to create room.");
        }
        else
        {
            Debug.Log("[CreateMultiRoom] Cloud and session lobby already ready! Proceeding immediately.");
        }

        MultiRoomManager.Instance.SetRoomName(roomName);
        Debug.Log("[CreateMultiRoom] Calling NetworkManager.CreateRoom...");
        await NetworkManager.Instance.CreateRoom(roomName, password, isPassword);
        Debug.Log("[CreateMultiRoom] CreateRoom call completed");
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