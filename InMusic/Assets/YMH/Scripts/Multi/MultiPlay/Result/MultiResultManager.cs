using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace Play.Result
{
    public class MultiResultManager : Singleton<MultiResultManager>
    {
        [Header("결과창 UI")]
        [SerializeField]
        private ResultText resultText;

        [SerializeField]
        private GameObject resultCanvas;
        [SerializeField]
        private UserResult userResult;
        private ScoreData[] scoreData;

        private int currentIndex = 0;
        private int winnerIndex = 0;

        private void Start()
        {
            if (MultiRoomManager.Instance.scoreDatas != null)
            {
                ReceiveResult(MultiRoomManager.Instance.scoreDatas);
            }
            else
            {
                resultCanvas.SetActive(false);
            }
        }

        private void SetWinner()
        {
            if (scoreData[0].score < scoreData[1].score)
            {
                ScoreData temp = scoreData[0];
                scoreData[0] = scoreData[1];
                scoreData[1] = temp;
                winnerIndex = 1 - winnerIndex;
            }
        }

        public void ReceiveResult(ScoreData[] scoreData)
        {
            this.scoreData = scoreData;
            winnerIndex = 0;
            SetWinner();
            Debug.Log($"ScoreData[0].userName: {scoreData[0].userName}, ScoreData[0].Score: {scoreData[0].score}");
            Debug.Log($"ScoreData[1].userName: {scoreData[1].userName}, ScoreData[1].Score: {scoreData[1].score}");

            userResult.SetUserResult(scoreData[0].userName, scoreData[0].isRed, 0);
            userResult.SetUserResult(scoreData[1].userName, scoreData[1].isRed, 1);

            StartCoroutine(ShowResults());
        }

        private IEnumerator ShowResults()
        {
            yield return null;

            resultCanvas.SetActive(true);
            userResult.Select(0);

            resultText.SetSongInfoText(scoreData[currentIndex].songName, scoreData[currentIndex].artist);
            resultText.SetResultText(scoreData[currentIndex].score, new int[] { scoreData[currentIndex].great, scoreData[currentIndex].good, scoreData[currentIndex].bad, scoreData[currentIndex].miss }, scoreData[currentIndex].accuracy, scoreData[currentIndex].maxCombo);
            resultText.SetRank(scoreData[currentIndex].rank, scoreData[currentIndex].isFullCombo);
        }

        public void OnClickUserResult(int index)
        {
            if (currentIndex == index)
                return;
            currentIndex = index;
            userResult.Select(currentIndex);

            //resultText.SetSongInfoText(scoreData[currentIndex].songName, scoreData[currentIndex].artist);
            resultText.SetResultText(scoreData[currentIndex].score, new int[] { scoreData[currentIndex].great, scoreData[currentIndex].good, scoreData[currentIndex].bad, scoreData[currentIndex].miss }, scoreData[currentIndex].accuracy, scoreData[currentIndex].maxCombo);
            resultText.SetRank(scoreData[currentIndex].rank, scoreData[currentIndex].isFullCombo);
        }

        public void OnClickNextButton()
        {
            NetworkManager.runnerInstance.LoadScene("MultiRoomScene_InMusic");
            //await SceneManager.LoadSceneAsync("MultiRoom");
        }
    }
}