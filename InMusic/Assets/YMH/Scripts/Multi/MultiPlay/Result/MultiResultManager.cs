using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Play.Result
{
    public class MultiResultManager : SingleTon<MultiResultManager>
    {
        [Header("결과창 UI")]
        [SerializeField]
        private ResultText resultText;

        [SerializeField]
        private GameObject resultCanvas;
        [SerializeField]
        private UserResult userResult;
        private ScoreData[] scoreData;

        private WaitForSeconds delayTime = new WaitForSeconds(3f); // 3 seconds

        private int currentIndex = 0;
        private int winnerIndex = 0;

        private void Start()
        {
            resultCanvas.SetActive(false);
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
            MultiPlayUserSetting.Instance.SetResultUserSetting(winnerIndex, 0);
            MultiPlayUserSetting.Instance.SetResultUserSetting(1 - winnerIndex, 1);
            StartCoroutine(ShowResults());
        }

        private IEnumerator ShowResults()
        {
            yield return delayTime;

            resultCanvas.SetActive(true);
            userResult.SetUserResult("Player");
            

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

            resultText.SetSongInfoText(scoreData[currentIndex].songName, scoreData[currentIndex].artist);
            resultText.SetResultText(scoreData[currentIndex].score, new int[] { scoreData[currentIndex].great, scoreData[currentIndex].good, scoreData[currentIndex].bad, scoreData[currentIndex].miss }, scoreData[currentIndex].accuracy, scoreData[currentIndex].maxCombo);
            resultText.SetRank(scoreData[currentIndex].rank, scoreData[currentIndex].isFullCombo);
        }

        public void OnClickNextButton()
        {
            NetworkManager.runnerInstance.LoadScene("MultiRoom");
        }
    }
}