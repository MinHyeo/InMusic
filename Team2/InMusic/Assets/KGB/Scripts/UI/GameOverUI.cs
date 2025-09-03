using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] GameObject restartButton;
    [SerializeField] GameObject musicSelectButton;
    [SerializeField] GameObject exitButton;


    [SerializeField] Sprite idleButtonSprite;
    [SerializeField] Sprite selectedButtonSprite;
    [SerializeField] int isSelected = 1;
    void Start()
    {
        restartButton.GetComponent<Image>().sprite = selectedButtonSprite;
        restartButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black; // �ؽ�Ʈ ���� ����
    }

    // Update is called once per frame
    void Update()
    {
        // Ű���� �Է� ó�� (����: ��/�Ʒ� ����Ű�� ���� �̵�)
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            isSelected = Mathf.Clamp(isSelected - 1, 1, 3);
            UpdateButtonSelection();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            isSelected = Mathf.Clamp(isSelected + 1, 1, 3);
            UpdateButtonSelection();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            switch (isSelected)
            {
                case 1:
                    Restart();
                    break;
                case 2:
                    MusicSelect();
                    break;
                case 3:
                    Exit();
                    break;
            }
        }
    }

    public void Restart()
    {
        Debug.Log("re");
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);

    }
    public void MusicSelect()
    {
        SceneManager.LoadScene(0);
    }
    public void Exit()
    {
        SceneManager.LoadScene(2); //����ȭ������ ������ �ӽ÷� ����ȭ��
    }

    private void UpdateButtonSelection()
    {
        // ��� ��ư�� idle ���·� ����
        restartButton.GetComponent<Image>().sprite = idleButtonSprite;
        musicSelectButton.GetComponent<Image>().sprite = idleButtonSprite;
        exitButton.GetComponent<Image>().sprite = idleButtonSprite;

        // ��� ��ư �ؽ�Ʈ�� �⺻ ����(���)���� ����
        restartButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        musicSelectButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        exitButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        // ���õ� ��ư�� selected ���·� ����
        switch (isSelected)
        {
            case 1:
                restartButton.GetComponent<Image>().sprite = selectedButtonSprite;
                restartButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black; // �ؽ�Ʈ ���� ����
                break;
            case 2:
                musicSelectButton.GetComponent<Image>().sprite = selectedButtonSprite;
                musicSelectButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black; // �ؽ�Ʈ ���� ����
                break;
            case 3:
                exitButton.GetComponent<Image>().sprite = selectedButtonSprite;
                exitButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black; // �ؽ�Ʈ ���� ����
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
