using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Play
{
    public class MultiScoreComparison : Singleton<MultiScoreComparison>
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

        public void UpdateMyScore(float score, AccuracyType accuracyType)
        {
            MyScoreManager.AddScore(score, accuracyType);
            UpdateComparisonUI();
        }

        public void UpdateMatchScore(float score, AccuracyType accuracyType)
        {
            MatchScoreManager.AddScore(score, accuracyType);
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

        public ScoreData[] SetScore(string songName, string artist)
        {
            MyScoreManager.SetRank();
            MatchScoreManager.SetRank();

            ScoreData[] scores = new ScoreData[2];
            scores[0] = MyScoreManager.SaveScore(songName, artist);
            scores[0].userName = MultiPlayUserSetting.Instance.GetUserName(0);
            scores[0].isRed = MultiPlayUserSetting.Instance.GetIsRed(0);
            Debug.Log($"MyScore - userName: {scores[0].userName}, Score: {scores[0].score}");

            scores[1] = MatchScoreManager.SaveScore(songName, artist);
            scores[1].userName = MultiPlayUserSetting.Instance.GetUserName(1);
            scores[1].isRed = MultiPlayUserSetting.Instance.GetIsRed(1);
            Debug.Log($"MatchScore - userName: {scores[1].userName}, Score: {scores[1].score}");

            return scores;
        }
    }
}