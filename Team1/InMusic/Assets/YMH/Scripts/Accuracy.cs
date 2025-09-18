using TMPro;
using UnityEngine;

namespace Play
{
    public enum AccuracyType
    {
        Great = 100,
        Good = 75,
        Bad = 50,
        Miss = 0
    }

    public class Accuracy : MonoBehaviour
    {
        //판정 텍스트 저장
        private TextMeshProUGUI accuarcy;
        //판정 색 저장
        [SerializeField]
        [ColorUsage(false)]
        private Color[] accuaryColors;

        //화면 표시 타이머 관련
        private float showTime = 0.5f;
        private float currentTime = 0.0f;

        //현재 화면에 정확도가 표시되어 있는지 확인
        private bool isShow = false;

        //초기 설정
        private void Start()
        {
            //변수 초기화
            accuarcy = GetComponent<TextMeshProUGUI>();
            //초기 글씨 안보이게 표시
            gameObject.SetActive(false);
        }

        public void ShowAccracy(AccuracyType type)
        {
            //텍스트 글자 변경
            accuarcy.text = type.ToString().ToLower();
            //텍스트 색 변경
            if (type == AccuracyType.Great) accuarcy.color = accuaryColors[0];
            else if (type == AccuracyType.Good) accuarcy.color = accuaryColors[1];
            else if (type == AccuracyType.Bad) accuarcy.color = accuaryColors[2];
            else if (type == AccuracyType.Miss) accuarcy.color = accuaryColors[3];;

            //정확도 표시
            if (!isShow)
            {
                isShow = !isShow;
                accuarcy.gameObject.SetActive(isShow);
            }

            //화면 표시 시간 초기화
            currentTime = 0.0f;
        }

        private void Update()
        {
            //화면 표시 시간 측정
            if (isShow)
            {
                currentTime += Time.deltaTime;

                if (currentTime >= showTime)
                {
                    isShow = !isShow;
                    accuarcy.gameObject.SetActive(isShow);
                }
            }
        }
    }
}