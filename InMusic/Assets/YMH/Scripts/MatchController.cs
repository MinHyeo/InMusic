using UnityEngine;
using TMPro;

namespace Play
{
    public class MatchController : MonoBehaviour
    {
        public void ShowKeyEffect(int keyIndex, AccuracyType accuracyType, float percent, int noteId)
        {
            Note targetNote = TimelineController.Instance.GetClosestNoteById(noteId);
            if (targetNote != null)
            {
                // 노트에 대한 판정 처리
                float score = targetNote.Hit();

                MultiScoreComparison.Instance.UpdateMatchScore(score, percent, accuracyType);
            }
        }
    }    
}