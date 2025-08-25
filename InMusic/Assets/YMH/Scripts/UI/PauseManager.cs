using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace Play
{
    public class PauseManager : Singleton<PauseManager>
    {
        [SerializeField]
        private GameObject pauseObject;
        private List<Button> buttons = new List<Button>();
        private List<Animator> buttonsAnim = new List<Animator>();
        private List<Slider> buttonsSlider = new List<Slider>();

        //���� ���õ� ��ư �ε���
        private int buttonIndex = 0;

        //�Ͻ����� ����
        private bool isPause = false;
        //����ϱ� ����
        private bool isContinue = false;

        //�Ͻ��������� ���� ���۱��� �ð� ��� ����
        [SerializeField]
        private TextMeshProUGUI pauseCount;
        private float currentTime = 0;
        private float maxTime = 3;

        private void Start()
        {
            //�ʱ�ȭ
            buttons.AddRange(pauseObject.gameObject.GetComponentsInChildren<Button>());
            buttonsAnim.AddRange(pauseObject.gameObject.GetComponentsInChildren<Animator>());
            buttonsSlider.AddRange(pauseObject.gameObject.GetComponentsInChildren<Slider>());

            //Ÿ�ӽ����� ����
            foreach (Animator anim in buttonsAnim)
            {
                anim.updateMode = AnimatorUpdateMode.UnscaledTime;
            }

            pauseObject.gameObject.SetActive(false);
            pauseCount.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (isPause && isContinue)
            {
                //�ð� ����
                currentTime += Time.unscaledDeltaTime;

                //���� �ð� �ؽ�Ʈ ǥ��
                pauseCount.text = ((int)(maxTime - currentTime + 1)).ToString();

                //�Ͻ����� ��ü
                if (currentTime >= maxTime)
                {
                    isPause = false;
                    isContinue = false;
                    pauseCount.gameObject.SetActive(false);

                    //���� ����
                    PlayManager.Instance.Continue();
                }
            }
        }

        public void Pause()
        {
            isPause = true;
            buttonIndex = 0;

            //�뷡 ����
            SoundManager.Instance.SetPause(isPause);

            //�Ͻ�����
            pauseObject.gameObject.SetActive(isPause);
            Time.timeScale = 0;

            //ù��° ��ư ���� ȿ��
            buttonsAnim[buttonIndex].SetTrigger("Select");

            //Ű �Է� ����
            GameManager.Input.SetUIKeyEvent(OnKeyPress);
        }

        public void Continue()
        {
            isContinue = true;
            currentTime = 0;

            pauseObject.gameObject.SetActive(false);
            pauseCount.gameObject.SetActive(true);
        }

        public void Restart()
        {
            pauseObject.gameObject.SetActive(false);
        }

        public void OnKeyPress(Define.UIControl keyEvent)
        {
            switch (keyEvent)
            {
                case Define.UIControl.Enter:
                    InputKeyEnter();
                    break;
                case Define.UIControl.Up:
                    InputKeyArrow(-1);
                    break;
                case Define.UIControl.Down:
                    InputKeyArrow(1);
                    break;
            }

            //���߿� enterŰ�� ���� ó���� �߰��ؾ���
        }

        public void DestroyKeyEvent()
        {
            GameManager.Input.RemoveUIKeyEvent(OnKeyPress);
        }

        private void InputKeyArrow(int keyNum)
        {
            //�� ��ư ���� ȿ�� ����
            buttonsAnim[buttonIndex].SetTrigger("Deselect");
            buttonsSlider[buttonIndex].value = 0;
            Canvas.ForceUpdateCanvases();

            //��ư �ε��� ����
            int buttonLength = buttonsSlider.Count;
            buttonIndex = buttonIndex + keyNum < 0 ? buttonIndex = buttonLength - 1 : (buttonIndex + keyNum) % buttonLength;

            //��ư ���� ȿ��
            buttonsAnim[buttonIndex].SetTrigger("Select");
        }

        private void InputKeyEnter()
        {
            buttonsSlider[buttonIndex].value = 0;
            buttons[buttonIndex].onClick.Invoke();
        }
    }

}