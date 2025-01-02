using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Play
{
    public class Combo : MonoBehaviour
    {
        private TextMeshProUGUI comboText;
        private Animator anim;

        //콤보 변수
        private int combo = 0;
        private int maxCombo = 0;

        //시간 변수
        private float currentTime = 0.0f;
        private float showTime = 1f;

        private bool isShow = false;

        //초기화
        private void Start()
        {
            comboText = GetComponent<TextMeshProUGUI>();
            anim = GetComponent<Animator>();
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (isShow)
            {
                currentTime += Time.deltaTime;

                if(currentTime >= showTime)
                {
                    isShow = !isShow;
                    anim.SetTrigger("Hide");
                }
            }
        }

        //외부에서 콤보 조작
        public void ChangeInCombo(AccuracyType accuracy)
        {
            if (accuracy != AccuracyType.Miss)
            {
                combo += 1;
                if (combo > maxCombo)
                {
                    maxCombo = combo;
                }
            }
            else
            {
                combo = 0;
            }

            ShowCombo();
        }

        private void ShowCombo()
        {
            //콤보 텍스트 변경 및 활성화
            comboText.text = string.Format("{0}", combo);
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                comboText.color = Color.white;
            }
                

            //시간 초기화
            if (!isShow)
                isShow = !isShow;
            currentTime = 0.0f;
        }

        public void HideCombo()
        {
            //비활성화
            gameObject.SetActive(false);
        }
    }
}