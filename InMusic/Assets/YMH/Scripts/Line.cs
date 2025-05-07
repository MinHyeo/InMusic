using UnityEngine;

namespace Play
{
    public class Line : MonoBehaviour
    {
        private float speed;
        private float targetY;
        private Vector3 direction = Vector3.down;  // 아래로 이동하는 방향

        public void Initialize(float lineSpeed, float judgementLineY)
        {
            speed = lineSpeed;
            targetY = judgementLineY;
        }

        void Update()
        {
            // 마디 선을 아래로 이동
            transform.Translate(direction * speed * Time.deltaTime);

            // 화면 아래로 사라지면 삭제
            if (transform.position.y < -10.0f)
            {
                //Metronome.Instance.RemoveLine(this.gameObject);
                TimelineController.Instance.RemoveLine(this.gameObject)
            }
        }
    }
}