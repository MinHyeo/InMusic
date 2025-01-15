using UnityEngine;


namespace Play.Result 
{
    public class ResultManager : SingleTon<ResultManager>
    {
        [Header("Scripts")]
        [SerializeField]
        private ResultText resultText;

        [Header("Result Canvas")]
        [SerializeField]
        private GameObject resultCanvas;
        private ScoreData scoreData;

        private void Start()
        {
            resultCanvas.SetActive(false);
        }

        // 플레이 결과 받아오기
        public void ReceiveResult(ScoreData scoreData)
        {
            this.scoreData = scoreData;
            ShowResult();
        }

        private void ShowResult()
        {
            //Canvas 활성화
            resultCanvas.SetActive(true);

            //노래 정보 출력
            resultText.SetSongInfoText(scoreData.songName, scoreData.artist);
            //결과 출력
            resultText.SetResultText(scoreData.score, new int[] { scoreData.great, scoreData.good, scoreData.bad, scoreData.miss }, scoreData.accuracy, scoreData.maxCombo);
            //결과에 따른 랭크 판정
            resultText.SetRank(scoreData.accuracy, scoreData.isFullCombo);
            //결과 저장
        }
    }
}