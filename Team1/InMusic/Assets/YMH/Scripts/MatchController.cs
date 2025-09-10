using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Play
{
    public class MatchController : Singleton<MatchController>
    {
        [SerializeField]
        private HpBar matchHpBar;
        [SerializeField]
        private GameObject DeathPanel;
        [SerializeField]
        private GameObject leftText;

        private bool isDead = false;

        [Header("Key Effects")]
        [SerializeField]
        private GameObject[] keyEffectObjects;

        private void Start()
        {
            DeathPanel.SetActive(false);
            leftText.SetActive(false);
        }

        public void ShowKeyEffect(int channel, AccuracyType accuracyType, float percent, int noteId)
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
                        keyEffectObjects[channel - 11].SetActive(true);
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

        public void OnPlayerLeft()
        {
            leftText.SetActive(true);
        }
    }    
}