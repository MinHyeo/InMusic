using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] GameObject continueButton;
    [SerializeField] GameObject restartButton;
    [SerializeField] GameObject musicSelectButton;
    [SerializeField] GameObject exitButton;

    [SerializeField] Sprite idleButtonSprite;
    [SerializeField] Sprite selectedButtonSprite;

    [SerializeField] int isSelected = 1;
    void Start()
    {
        continueButton.GetComponent<Image>().sprite = selectedButtonSprite;
        continueButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black; // 텍스트 색상 변경
    }

    // Update is called once per frame
    void Update()
    {
        // 키보드 입력 처리 (예시: 위/아래 방향키로 선택 이동)
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            isSelected = Mathf.Clamp(isSelected - 1, 1, 4);
            UpdateButtonSelection();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            isSelected = Mathf.Clamp(isSelected + 1, 1, 4);
            UpdateButtonSelection();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            switch (isSelected)
            {
                case 1:
                    Continue();
                    break;
                case 2:
                    Restart();
                    break;
                case 3:
                    MusicSelect();
                    break;
                case 4:
                    Exit();
                    break;

            }
        }
    }

    public void Continue()
    {
        GameManager.Instance.ResumeGame();
    }
    public void Restart()
    {
        Debug.Log("re");
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);

    }
    public void MusicSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    public void Exit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); //메인화면으로 가야함 임시로 선택화면
    }
 

    private void UpdateButtonSelection()
    {
        // 모든 버튼을 idle 상태로 설정
        continueButton.GetComponent<Image>().sprite = idleButtonSprite;
        restartButton.GetComponent<Image>().sprite = idleButtonSprite;
        musicSelectButton.GetComponent<Image>().sprite = idleButtonSprite;
        exitButton.GetComponent<Image>().sprite = idleButtonSprite;

        // 모든 버튼 텍스트의 기본 색상(흰색)으로 설정
        continueButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        restartButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        musicSelectButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        exitButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        // 선택된 버튼만 selected 상태로 설정
        switch (isSelected)
        {
            case 1:
                continueButton.GetComponent<Image>().sprite = selectedButtonSprite;
                continueButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black; // 텍스트 색상 변경
                break;
            case 2:
                restartButton.GetComponent<Image>().sprite = selectedButtonSprite;
                restartButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black; // 텍스트 색상 변경
                break;
            case 3:
                musicSelectButton.GetComponent<Image>().sprite = selectedButtonSprite;
                musicSelectButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black; // 텍스트 색상 변경
                break;
            case 4:
                exitButton.GetComponent<Image>().sprite = selectedButtonSprite;
                exitButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black; // 텍스트 색상 변경
                break;
        }
    }

    private void SetButtonSprite(Button button, Sprite sprite)
    {
        if (button != null && button.image != null)
        {
            button.image.sprite = sprite;
        }
    }
}
