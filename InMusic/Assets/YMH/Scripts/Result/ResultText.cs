using FMOD.Studio;
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Play.Result
{
    public enum Rank
    {
        S,
        A,
        B,
        C,
        D,
    }

    public class ResultText : MonoBehaviour
    {
        [Header("Song Info Texts")]
        [SerializeField]
        private Image songImage;
        [SerializeField]
        private TextMeshProUGUI songNameText;
        [SerializeField]
        private TextMeshProUGUI songArtistText;
        [SerializeField]
        private Image backgroundImage;

        [Header("Score Texts")]
        [SerializeField]
        private TextMeshProUGUI scoreText;
        [SerializeField]
        private TextMeshProUGUI[] judgmentText;
        [SerializeField]
        private TextMeshProUGUI accuracyText;
        [SerializeField]
        private TextMeshProUGUI maxComboText;

        [Header("Rank Texts")]
        [SerializeField]
        private TextMeshProUGUI rankText;
        [SerializeField]
        [ColorUsage(true)]
        private Color[] colors;

        /// <summary>
        /// 플레이한 노래 정보 표시
        /// </summary>
        /// <param name="songName"></param>
        /// <param name="songArtist"></param>
        public void SetSongInfoText(string songName, string songArtist)
        {
            string path = "Song/" + songName + "/" + songName;
            var image = Resources.Load<Sprite>(path);
            if(image == null)
            {
                Debug.LogError($"Image not found at path: {path}");
                return;
            }

            songImage.sprite = image;
            songNameText.text = songName;
            songArtistText.text = songArtist;
            backgroundImage.sprite = image;
        }

        /// <summary>
        /// 플레이 결과 UI 텍스트 적용
        /// </summary>
        /// <param name="score"></param>
        /// <param name="judgment"></param>
        /// <param name="accuracy"></param>
        /// <param name="maxCombo"></param>
        public void SetResultText(int score, int[] judgment, float accuracy, int maxCombo)
        {
            //결과 저장
            scoreText.text = score.ToString();
            for (int i = 0; i < judgmentText.Length; i++)
            {
                judgmentText[i].text = judgment[i].ToString();
            }
            accuracyText.text = accuracy.ToString("F2") + "%";
            maxComboText.text = maxCombo.ToString();
        }

        public void SetRank(string rank, bool isFullCombo)
        {
            int rankColor = 0;
            if (isFullCombo)
                rankColor = 1;

            rankText.text = rank;
            rankText.colorGradient = new VertexGradient(Color.white, Color.white, colors[rankColor], colors[rankColor]);
        }
    }
}