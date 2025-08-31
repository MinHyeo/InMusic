using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Play
{
    public class GameOverManager : Singleton<GameOverManager>
    {
        private List<Button> buttons = new List<Button>();
        private List<Animator> buttonsAnim = new List<Animator>();
        private List<Slider> buttonsSlider = new List<Slider>();

        private int buttonIndex = 0;

        private void Start()
        {
            buttons.AddRange(gameObject.GetComponentsInChildren<Button>());
            buttonsAnim.AddRange(gameObject.GetComponentsInChildren<Animator>());
            buttonsSlider.AddRange(gameObject.GetComponentsInChildren<Slider>());

            gameObject.SetActive(false);
        }

        public void GameOver()
        {
            gameObject.SetActive(true);
            SoundManager.Instance.SetPause(true);

            buttonIndex = 0;
            buttonsAnim[buttonIndex].SetTrigger("Select");

            GameManager.Input.SetUIKeyEvent(OnKeyPress);
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
        }

        private void OnEnable()
        {
            GameManager.Input.RemoveUIKeyEvent(OnKeyPress);
        }

        private void InputKeyArrow(int keyNum)
        {
            //�� ��ư ���� ȿ�� ����
            buttonsAnim[buttonIndex].SetTrigger("Deselect");
            buttonsSlider[buttonIndex].value = 0;

            //��ư �ε��� ����
            int buttonLength = buttonsSlider.Count;
            buttonIndex = buttonIndex + keyNum < 0 ? buttonLength - 1 : (buttonIndex + keyNum) % buttonLength;

            //��ư ���� ȿ��
            buttonsAnim[buttonIndex].SetTrigger("Select");
        }

        private void InputKeyEnter()
        {
            buttonsSlider[buttonIndex].value = 0;
            buttons[buttonIndex].onClick.Invoke();
        }

        public void Restart()
        {
            PlayManager.Instance.ReStart();
            gameObject.SetActive(false);
        }

        public void MusicSelect()
        {
            SoundManager.Instance.End();
            GameManager.Instance.SetGameState(GameState.MusicSelect);
            GameManager.Instance.SelectSong(PlayManager.Instance.SongTitle);
        }

        public void Exit()
        {
            GameManager.Instance.SetGameState(GameState.MainMenu);
        }
    }
}
