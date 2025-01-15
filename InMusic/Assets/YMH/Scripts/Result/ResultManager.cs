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

        // �÷��� ��� �޾ƿ���
        public void ReceiveResult(ScoreData scoreData)
        {
            this.scoreData = scoreData;
            ShowResult();
        }

        private void ShowResult()
        {
            //Canvas Ȱ��ȭ
            resultCanvas.SetActive(true);

            //�뷡 ���� ���
            resultText.SetSongInfoText(scoreData.songName, scoreData.artist);
            //��� ���
            resultText.SetResultText(scoreData.score, new int[] { scoreData.great, scoreData.good, scoreData.bad, scoreData.miss }, scoreData.accuracy, scoreData.maxCombo);
            //����� ���� ��ũ ����
            resultText.SetRank(scoreData.accuracy, scoreData.isFullCombo);
            //��� ����
        }
    }
}