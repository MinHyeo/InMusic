using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

namespace Play 
{
    public class Note : MonoBehaviour
    {
        //노트 변수
        private int noteId;
        private int channel;
        private float speed;
        private float noteScore;
        private bool isHited = false;
        private float targetTime;

        //외부 접근용 변수
        public int NoteId => noteId;
        public int Channel => channel;
        public float Speed => speed;
        public float NoteScore => noteScore;
        public float TargetTime => targetTime;

        public void Initialize(int noteId, int channel, float speed, float noteScore, float travelTime, bool isMatch = false)
        {
            this.noteId = noteId;
            this.channel = channel;
            this.speed = speed;
            this.noteScore = noteScore;
            targetTime = travelTime + Time.time;
            isHited = false;
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

        public float Hit(int isMatch = 0)
        {
            // 노트가 맞았을 때의 처리 (예: 이펙트, 점수 추가, 노트 비활성화 등)
            isHited = true;
            TimelineController.Instance.RemoveNote(this, isMatch);
            Debug.Log("---Hit---");

            return noteScore;
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.CompareTag("EndLine"))
            {
                if (isHited)
                    return;
                    
                switch (GameManager.Instance.CurrentGameState)
                {
                    case GameState.GamePlay:
                        PlayManager.Instance.HandleNoteHit(this, AccuracyType.Miss, 0);
                        break;
                }
            }
        }
    }
}