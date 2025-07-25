using UnityEngine;
using TMPro;

namespace Play
{
    public class MatchController : MonoBehaviour
    {
        [SerializeField]
        private Accuracy accuracyScript;
        //콤보 관련
        [SerializeField]
        private Combo comboScript;

        [Header("점수 관련")]
        [SerializeField]
        private TextMeshProUGUI scoreText;
        [SerializeField]
        private TextMeshProUGUI accuracyText;

        private ScoreManager scoreManager;

        private void Start()
        {
            scoreManager = new ScoreManager(scoreText, accuracyText, accuracyScript, comboScript);
            scoreManager.Init();
        }

        public void ShowKeyEffect(int keyIndex, AccuracyType accuracyType, float percent, int noteId)
        {
            Note targetNote = TimelineController.Instance.GetClosestNoteById(noteId);
            if (targetNote != null)
            {
                // 노트에 대한 판정 처리
                float score = targetNote.Hit();

                scoreManager.AddScore(score, percent, accuracyType);
            }
        }
    }    
}