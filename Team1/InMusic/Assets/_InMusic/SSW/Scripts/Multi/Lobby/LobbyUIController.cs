using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fusion;

public class LobbyUIController : MonoBehaviour
{
    public void OnClickBackButton()
    {
        Debug.Log("[LobbyUIController] Back button clicked.");
        
        // NetworkManager의 static runnerInstance 사용
        if (NetworkManager.runnerInstance != null && NetworkManager.runnerInstance.IsRunning)
        {
            Debug.Log("[LobbyUIController] Shutting down network runner...");
            NetworkManager.runnerInstance.Shutdown();
            
            // 셧다운 완료까지 잠시 대기 후 씬 전환
            Invoke(nameof(LoadMainScene), 0.3f);
        }
        else
        {
            Debug.Log("[LobbyUIController] No active network runner found.");
            LoadMainScene();
        }
    }
    
    private void LoadMainScene()
    {
        Debug.Log("[LobbyUIController] Loading main lobby scene.");
        SceneManager.LoadScene("MainLobbyScene_InMusic");
    }
}