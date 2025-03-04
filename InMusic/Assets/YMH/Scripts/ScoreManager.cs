using System.Collections;
using TMPro;
using UnityEngine;

namespace Play 
{
    public class ScoreManager : MonoBehaviour
    {
        private Accuracy accuracyScript;
        private Combo comboScript;

        private TextMeshProUGUI scoreText;
        private TextMeshProUGUI accuracyText;

        private float score = 0;
        private float accuracy = 0;
        private float totalAccuracy = 0;
        private int noteCount = 0;

        //입력 횟수
        private int[] inputCount = new int[4] { 0, 0, 0, 0 };

        //외부 접근
        public float Score { get { return score; } private set { } }
        public float Accuracy { get { return accuracy; } private set { } }
        public int[] InputCount { get { return inputCount; } private set { } }

        public ScoreManager(TextMeshProUGUI scoreText, TextMeshProUGUI accuracyText, Accuracy accuracy, Combo combo)
        {
            this.scoreText = scoreText;
            this.accuracyText = accuracyText;

            this.accuracyScript = accuracy;
            this.comboScript = combo;
        }

        public void Init()
        {
            score = 0;
            accuracy = 0;

            inputCount = new int[4] { 0, 0, 0, 0 };

            scoreText.text = "0";
            accuracyText.text = "0.00%";
        }
        
        public void AddScore(float noteScore, float percent, AccuracyType accuracyResult)
        {
            //점수 계산
            score += noteScore * (percent / 100);
            scoreText.text = ((int)score).ToString();

            //정확도 계산
            totalAccuracy += percent;
            noteCount += 1;
            accuracy = totalAccuracy / noteCount;
            accuracyText.text = accuracy.ToString("F2") + "%";

            //정확도 표시
            accuracyScript.ShowAccracy(accuracyResult);
            //콤보 표시
            comboScript.ChangeInCombo(accuracyResult);

            //정확도 별 입력 횟수 저장
            inputCount[(int)accuracyResult] += 1;
        }
    }
}