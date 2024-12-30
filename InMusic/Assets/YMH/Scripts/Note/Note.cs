using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Note : MonoBehaviour
{
    public int channel;
    private float speed;
    private float travelTime;
    public float noteScore;

    public float targetTime;

    public void Initialize(int channel, float noteSpeed, float travelTime, float noteScore)
    {
        this.channel = channel;
        this.speed = noteSpeed;
        this.targetTime = travelTime + Time.time;
        this.noteScore = noteScore;
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
        Destroy(gameObject);

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
