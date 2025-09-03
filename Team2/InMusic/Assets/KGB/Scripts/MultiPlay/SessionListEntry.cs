using Fusion;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SessionListEntry : MonoBehaviour
{
    public TextMeshProUGUI roomName, playerCount;
    public Button joinButton;

    public bool isSelected;
    public Image curImage;
    public Sprite defaultImage;
    public Sprite selectedImage;

    private string lobbyScene = "KGB_Multi_Lobby";
    private string waitingScene = "Waiting_Room_PSH";
    private GameObject passwordPanel; // ��й�ȣ �Է� �г�
    private TMP_InputField passwordInputField;
    private Button passwordConfirmButton;

    private string sessionName;
    private string sessionPassword;
    

    public void Initialize(SessionInfo session)
    {
        sessionName = session.Name;
        roomName.text = session.Name;
        playerCount.text = $"{session.PlayerCount}/{session.MaxPlayers}";
        joinButton.interactable = session.IsOpen;
        // ��й�ȣ �������� Ȯ��
        if (session.Properties != null && session.Properties.TryGetValue("pw", out SessionProperty pw))
        {
            sessionPassword = (string)pw.PropertyValue;
            Debug.Log("���Ǻ��"+sessionPassword);
        }
    }

    public void JoinRoom()
    {
        if (!string.IsNullOrEmpty(sessionPassword))
        {
            passwordPanel.SetActive(true); // ��й�ȣ �Է� �г� �����ֱ�
            
           
        }
        else
        {
            StartSession();
        }
    }

    public void OnPasswordConfirm()
    {
        if (passwordInputField.text == sessionPassword)
        {
            StartSession();
        }
        else
        {
            Debug.Log("��й�ȣ�� Ʋ�Ƚ��ϴ�.");
        }
    }

    private void StartSession()
    {
        NetworkManager.runnerInstance.StartGame(new StartGameArgs()
        {
            SessionName = sessionName,
            GameMode = GameMode.Shared,
            Scene = SceneRef.FromIndex(GetSceneIndex(waitingScene)),
        });
    }

    public void SetPasswordPanel(GameObject panel)
    {
        passwordPanel = panel;
        passwordInputField = passwordPanel.transform.GetComponentInChildren<TMP_InputField>();
        passwordConfirmButton = passwordPanel.transform.Find("roomCreatePanel/ConfirmButton").GetComponent<Button>();

        passwordConfirmButton.onClick.AddListener(OnPasswordConfirm);
    }

    public int GetSceneIndex(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (name == sceneName)
            {
                return i;
            }
        }
        return -1;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (curImage != null)
        {
            curImage.sprite = selected ? selectedImage : defaultImage;
        }
    }

}
