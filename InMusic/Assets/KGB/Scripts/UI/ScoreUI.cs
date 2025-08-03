using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // 점수 표시 텍스트
    public TextMeshProUGUI accuracyText; // 정확도 표시 텍스트
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
