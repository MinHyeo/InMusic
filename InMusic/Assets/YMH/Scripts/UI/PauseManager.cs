using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Play
{
    public class PauseManager : MonoBehaviour
    {
        public static PauseManager Instance;
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Scene�� �������� PauseManager ����");
            }
            Instance = this;
        }

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
        }

        private void Update()
        {
            if (isPause && isContinue)
            {
                currentTime += Time.unscaledDeltaTime;

                if(currentTime >= maxTime)
                {
                    isPause = false;
                    isContinue = false;

                    //���� ����
                    PlayManager.Instance.Continue();
                }
            }
        }

        public void Pause()
        {
            isPause = true;

            //�뷡 ����
            SoundManager.Instance.Pause(isPause);

            //�Ͻ�����
            pauseObject.gameObject.SetActive(isPause);
            Time.timeScale = 0;

            //ù��° ��ư ���� ȿ��
            buttonsAnim[buttonIndex].SetTrigger("Select");
        }

        public void Continue()
        {
            isContinue = true;
            currentTime = 0;

            pauseObject.gameObject.SetActive(false);
        }

        public void OnKeyPress(KeyCode key)
        {
            switch (key)
            {
                case KeyCode.Return:
                    InputKeyEnter();
                    break;
                case KeyCode.UpArrow:
                    InputKeyArrow(-1);
                    break;
                case KeyCode.DownArrow:
                    InputKeyArrow(1);
                    break;
            }

            //���߿� enterŰ�� ���� ó���� �߰��ؾ���
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
            buttons[buttonIndex].onClick.Invoke();
        }
    }

}