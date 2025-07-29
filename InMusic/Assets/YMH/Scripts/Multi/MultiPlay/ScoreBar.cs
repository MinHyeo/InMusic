using UnityEngine;
using UnityEngine.UI;

public class ScoreBar : MonoBehaviour
{
    [SerializeField]
    private Scrollbar scrollbar;
    [SerializeField]
    private Text scoreText;

    [SerializeField]
    [ColorUsage(false)]
    private Color[] scoreColors = new Color[2];

    public void Init()
    {
        if (scrollbar == null || scoreText == null)
        {
            Debug.LogError("Scrollbar or ScoreText is not assigned in ScoreBar.");
            return;
        }

        scrollbar.value = 0.5f;
        scoreText.text = "0";
        scoreText.color = Color.black;
    }

    public void UpdateScoreBar(float myScore, float matchScore)
    {
        if (scrollbar == null || scoreText == null)
        {
            Debug.LogError("Scrollbar or ScoreText is not assigned in ScoreBar.");
            return;
        }

        // Update the scrollbar value based on the scores
        scrollbar.value = Mathf.Clamp01(myScore / (myScore + matchScore));

        // Update the score text
        int scoreDifference = (int)(myScore - matchScore);
        if (scoreDifference > 0)
        {
            scoreText.text = $"+{scoreDifference}";
        }
        else if (scoreDifference < 0)
        {
            scoreText.text = $"{scoreDifference}";
        }
        else
        {
            scoreText.text = "0";
        }

        // Change color based on score comparison
        if (myScore > matchScore)
        {
            scoreText.color = scoreColors[0]; // My score is higher
        }
        else if (myScore < matchScore)
        {
            scoreText.color = scoreColors[1]; // Match score is higher
        }
        else
        {
            scoreText.color = Color.black; // Scores are equal
        }
    }
}
