using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace Play
{
    public class PauseManager : SingleTon<PauseManager>
    {
        [SerializeField]
        private GameObject pauseObject;
        private List<Button> buttons = new List<Button>();
        private List<Animator> buttonsAnim = new List<Animator>();
        private List<Slider> buttonsSlider = new List<Slider>();

        //현재 선택된 버튼 인덱스
        private int buttonIndex = 0;

        //일시정지 여부
        private bool isPause = false;
        //계속하기 여부
        private bool isContinue = false;

        //일시정지에서 게임 시작까지 시간 계산 변수
        [SerializeField]
        private TextMeshProUGUI pauseCount;
        private float currentTime = 0;
        private float maxTime = 3;

        private void Start()
        {
            //초기화
            buttons.AddRange(pauseObject.gameObject.GetComponentsInChildren<Button>());
            buttonsAnim.AddRange(pauseObject.gameObject.GetComponentsInChildren<Animator>());
            buttonsSlider.AddRange(pauseObject.gameObject.GetComponentsInChildren<Slider>());

            //타임스케일 무시
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
                //시간 측정
                currentTime += Time.unscaledDeltaTime;

                //남은 시간 텍스트 표시
                pauseCount.text = ((int)(maxTime - currentTime + 1)).ToString();

                //일시정지 해체
                if (currentTime >= maxTime)
                {
                    isPause = false;
                    isContinue = false;
                    pauseCount.gameObject.SetActive(false);

                    //게임 진행
                    PlayManager.Instance.Continue();
                }
            }
        }

        public void Pause()
        {
            isPause = true;

            //노래 정지
            SoundManager.Instance.Pause(isPause);

            //일시정지
            pauseObject.gameObject.SetActive(isPause);
            Time.timeScale = 0;

            //첫번째 버튼 선택 효과
            buttonsAnim[buttonIndex].SetTrigger("Select");

            //키 입력 설정
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

            //나중에 enter키에 대한 처리도 추가해야함
        }

        public void DestroyKeyEvent()
        {
            GameManager.Input.RemoveUIKeyEvent(OnKeyPress);
        }

        private void InputKeyArrow(int keyNum)
        {
            //전 버튼 선택 효과 제거
            buttonsAnim[buttonIndex].SetTrigger("Deselect");
            buttonsSlider[buttonIndex].value = 0;
            Canvas.ForceUpdateCanvases();

            //버튼 인덱스 변경
            int buttonLength = buttonsSlider.Count;
            buttonIndex = buttonIndex + keyNum < 0 ? buttonIndex = buttonLength - 1 : (buttonIndex + keyNum) % buttonLength;

            //버튼 선택 효과
            buttonsAnim[buttonIndex].SetTrigger("Select");
        }

        private void InputKeyEnter()
        {
            buttons[buttonIndex].onClick.Invoke();
        }
    }

}