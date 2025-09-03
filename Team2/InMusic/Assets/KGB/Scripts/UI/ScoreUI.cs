using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // ���� ǥ�� �ؽ�Ʈ
    public TextMeshProUGUI accuracyText; // ��Ȯ�� ǥ�� �ؽ�Ʈ
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateScoreDisplay();
    }

    public void UpdateScoreDisplay()
    {
        
        scoreText.text = $"{GameManagerProvider.Instance.TotalScore:F0}";
        accuracyText.text = $"{GameManagerProvider.Instance.Accuracy:F2}%";
    }
}
