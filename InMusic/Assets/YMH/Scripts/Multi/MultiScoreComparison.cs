using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Play
{
    public class MultiScoreComparison : SingleTon<MultiScoreComparison>
    {
        public ScoreManager MyScoreManager;
        public ScoreManager MatchScoreManager;

        public ScoreBar scoreBar;
        public MultiAccuracy multiAccuracy;
        public MultiMiss multiMiss;

        private void Start()
        {
            scoreBar = GetComponentInChildren<ScoreBar>();
            multiAccuracy = GetComponentInChildren<MultiAccuracy>();
            multiMiss = GetComponentInChildren<MultiMiss>();

            if (scoreBar == null || multiAccuracy == null || multiMiss == null)
            {
                Debug.LogError("One or more components are missing in MultiScoreComparison.");
            }

            MyScoreManager.Init();
            MatchScoreManager.Init();

            scoreBar.Init();
            multiAccuracy.Init();
            multiMiss.Init();
        }

        public void UpdateMyScore(float score, float percent, AccuracyType accuracyType)
        {
            MyScoreManager.AddScore(score, percent, accuracyType);
            UpdateComparisonUI();
        }

        public void UpdateMatchScore(float score, float percent, AccuracyType accuracyType)
        {
            MatchScoreManager.AddScore(score, percent, accuracyType);
            UpdateComparisonUI();
        }

        private void UpdateComparisonUI()
        {
            if (MyScoreManager == null || MatchScoreManager == null)
            {
                Debug.LogError("Score managers are not initialized.");
                return;
            }

            // Update the UI elements with the current scores and accuracies
            scoreBar.UpdateScoreBar(MyScoreManager.Score, MatchScoreManager.Score);
            multiAccuracy.UpdateAccuracy(MyScoreManager.Accuracy, MatchScoreManager.Accuracy);
            multiMiss.UpdateMissCount(MyScoreManager.InputCount[3], MatchScoreManager.InputCount[3]);
        }
    }
}

