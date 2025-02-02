using UnityEngine;
using UnityEngine.Pool;

namespace Play 
{
    public class Note : MonoBehaviour
    {
        public IObjectPool<GameObject> Pool { get; set; }

        //노트 변수
        private int channel;
        private float speed;
        private float noteScore;

        public float targetTime;

        //외부 접근용 변수
        public int Channel { get { return channel; } }
        public float Speed { get { return speed; } }
        public float NoteScore { get { return noteScore; } }

        public void Initialize(int channel, float speed, float noteScore, float travelTime)
        {
            this.channel = channel;
            this.speed = speed;
            this.noteScore = noteScore;
            targetTime = travelTime + Time.time;
        }

        private void Update()
        {
            // 노트가 내려오는 로직
            transform.Translate(Vector3.down * speed * Time.deltaTime);

            // 특정 위치까지 도달하면 삭제
            if (transform.position.y <= -3.0f)
            {
                Destroy(gameObject);
            }
        }

        public float Hit()
        {
            // 노트가 맞았을 때의 처리 (예: 이펙트, 점수 추가, 노트 비활성화 등)
            NoteManager.Instance.RemoveNoteFromActiveList(this);

            return noteScore;
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.CompareTag("EndLine"))
            {
                Debug.Log("Miss");
                PlayManager.Instance.HandleNoteHit(this, AccuracyType.Miss, 0);
            }
        }
    }
}