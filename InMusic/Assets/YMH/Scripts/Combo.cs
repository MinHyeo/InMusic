using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Play
{
    public class Combo : MonoBehaviour
    {
        private TextMeshProUGUI comboText;
        private Animator anim;

        //�޺� ����
        private int combo = 0;
        private int maxCombo = 0;

        //�ð� ����
        private float currentTime = 0.0f;
        private float showTime = 1f;

        private bool isShow = false;

        //�ʱ�ȭ
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

        //�ܺο��� �޺� ����
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
            //�޺� �ؽ�Ʈ ���� �� Ȱ��ȭ
            comboText.text = string.Format("{0}", combo);
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                comboText.color = Color.white;
            }
                

            //�ð� �ʱ�ȭ
            if (!isShow)
                isShow = !isShow;
            currentTime = 0.0f;
        }

        public void HideCombo()
        {
            //��Ȱ��ȭ
            gameObject.SetActive(false);
        }
    }
}