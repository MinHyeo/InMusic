using System.Collections;
using UnityEngine;


namespace Play.Result 
{
    public class ResultManager : Singleton<ResultManager>
    {
        [Header("Scripts")]
        [SerializeField]
        private ResultText resultText;

        [Header("Result Canvas")]
        [SerializeField]
        private GameObject resultCanvas;
        private ScoreData scoreData;

        private WaitForSeconds delayTime = new WaitForSeconds(3f); //3s

        private void Start()
        {
            resultCanvas.SetActive(false);
        }

        // �÷��� ��� �޾ƿ���
        public void ReceiveResult(ScoreData scoreData)
        {
            this.scoreData = scoreData;
            StartCoroutine(ShowResult());
        }

        private IEnumerator ShowResult()
        {
            //������
            yield return delayTime;

            //Canvas Ȱ��ȭ
            resultCanvas.SetActive(true);

            //�뷡 ���� ���
            resultText.SetSongInfoText(scoreData.songName, scoreData.artist);
            //��� ���
            resultText.SetResultText(scoreData.score, new int[] { scoreData.great, scoreData.good, scoreData.bad, scoreData.miss }, scoreData.accuracy, scoreData.maxCombo);

            //����� ���� ��ũ ����
            resultText.SetRank(scoreData.rank, scoreData.isFullCombo);
        }

        public void OnClickNextButton()
        {
            GameManager.Instance.ReturnMusicSelectScene(scoreData);
        }
    }
}