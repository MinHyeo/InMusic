using UnityEngine;
using TMPro;

namespace Play
{
    public class MatchController : MonoBehaviour
    {
        [SerializeField]
        private HpBar matchHpBar;
        [SerializeField]
        private GameObject DeathPanel;
        private bool isDead = false;

        private void Start()
        {
            DeathPanel.SetActive(false);
        }

        public void ShowKeyEffect(AccuracyType accuracyType, float percent, int noteId)
        {
            Note targetNote = TimelineController.Instance.GetClosestNoteById(noteId);
            if (targetNote != null)
            {
                // 노트에 대한 판정 처리
                float score = targetNote.Hit(1);

                switch (accuracyType)
                {
                    case AccuracyType.Miss:
                        isDead = matchHpBar.SetHp(-10);
                        MatchPlayerDeath();
                        break;
                    default:
                        matchHpBar.SetHp(5);
                        break;
                }
                MultiScoreComparison.Instance.UpdateMatchScore(score, percent, accuracyType);
            }
        }

        private void MatchPlayerDeath()
        {
            if (isDead)
            {
                Debug.Log("Match player is dead");

                DeathPanel.SetActive(true);
            }
        }
    }    
}