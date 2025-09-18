using Play.Result;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Play 
{
    public class ScoreManager : MonoBehaviour
    {
        [Header("Score and Accuracy Scripts")]
        [SerializeField]
        private Accuracy accuracyScript;
        [SerializeField]
        private Combo comboScript;

        [Header("UI Elements")]
        [SerializeField]
        private TextMeshProUGUI scoreText;
        [SerializeField]
        private TextMeshProUGUI accuracyText;

        private float score = 0;
        private float accuracy = 0;
        private float totalAccuracy = 0;
        private string rank;
        private int noteCount = 0;

        //입력 횟수
        private int[] inputCount = new int[4] { 0, 0, 0, 0 };

        //외부 접근
        public float Score { get { return score; } private set { } }
        public float Accuracy { get { return accuracy; } private set { } }
        public string Rank { get { return rank; } private set { } }
        public int[] InputCount { get { return inputCount; } private set { } }

        public void Init()
        {
            score = 0;
            accuracy = 0;

            inputCount = new int[4] { 0, 0, 0, 0 };

            scoreText.text = "0";
            accuracyText.text = "0.00%";
        }

        public void AddScore(float noteScore, AccuracyType accuracyResult)
        {
            //점수 계산
            score += noteScore * ((float)accuracyResult / 100);
            scoreText.text = ((int)score).ToString();

            //정확도 계산
            totalAccuracy += (int)accuracyResult;
            noteCount += 1;
            accuracy = totalAccuracy / noteCount;
            accuracyText.text = accuracy.ToString("F2") + "%";

            //정확도 표시
            accuracyScript.ShowAccracy(accuracyResult);
            //콤보 표시
            comboScript.ChangeInCombo(accuracyResult);

            //정확도 별 입력 횟수 저장
            int index = System.Array.IndexOf(System.Enum.GetValues(typeof(AccuracyType)), accuracyResult);
            inputCount[index] += 1;
        }

        public void SetRank()
        {
            int num = (int)accuracy / 5;

            if (accuracy >= 95)
            {
                rank = Result.Rank.S.ToString();
            }
            else if (accuracy >= 90)
            {
                rank = Result.Rank.A.ToString();
            }
            else if (accuracy >= 80)
            {
                rank = Result.Rank.B.ToString();
            }
            else if (accuracy >= 70)
            {
                rank = Result.Rank.C.ToString();
            }
            else
            {
                rank = Result.Rank.D.ToString();
            }
        }
        
        public ScoreData SaveScore(string songName, string artist)
        {
            SetRank();

            ScoreData scoreData = new ScoreData
            {
                songName = songName,
                artist = artist,
                songKey = $"{songName}_{artist}",
                score = (int)score,
                accuracy = accuracy,
                rank = rank,
                great = inputCount[0],
                good = inputCount[1],
                bad = inputCount[2],
                miss = inputCount[3],
                maxCombo = comboScript.MaxCombo,
                isFullCombo = comboScript.IsFullCombo
            };

            return scoreData;
        }
    }
}