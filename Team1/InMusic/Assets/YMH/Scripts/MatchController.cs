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

        public void ShowOpponentHitResult(AccuracyType accuracyType, int channel, int noteId = 1)
        {
            // 판정 결과에 따라 상대방의 HP를 조절하고 시각적 이펙트를 보여줍니다.
            switch (accuracyType)
            {
                case AccuracyType.Miss:
                    if (matchHpBar.SetHp(-10))
                    {
                        // 상대방 HP가 0이 되면 사망 처리
                        MatchPlayerDeath();
                    }
                    break;

                // Great, Good, Bad 판정일 경우
                default:
                    matchHpBar.SetHp(5);
                    // 상대방의 키 입력 이펙트를 활성화합니다.
                    if (channel >= 11 && channel <= 14)
                    {
                        keyEffectObjects[channel - 11].SetActive(true);
                    }
                    break;
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