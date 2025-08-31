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
        private int totalNotes;

        public int MaxCombo { get { return maxCombo; } private set { } }
        public bool IsFullCombo
        {
            get
            {
                int totalNotes = TimelineController.Instance.NoteCount;
                return maxCombo == totalNotes;
            }
        }

        //�ð� ����
        private float currentTime = 0.0f;
        private const float showTime = 2f;

        private bool isShow = false;

        public void Init()
        {
            currentTime = 0;
            combo = 0;
            maxCombo = 0;
            comboText.text = $"{combo}";
        }

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
                    anim.enabled = true;
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
                Debug.Log("Miss");
                combo = 0;
            }

            ShowCombo();
        }

        private void ShowCombo()
        {
            //�޺� �ؽ�Ʈ ���� �� Ȱ��ȭ
            comboText.text = string.Format($"{combo}");
            if (!gameObject.activeSelf)
            {
                anim.enabled = false;

                gameObject.SetActive(true);
                comboText.alpha = 1;
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