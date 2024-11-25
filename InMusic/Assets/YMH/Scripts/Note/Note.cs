using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Note : MonoBehaviour
{
    public int channel;
    private float speed;
    private float targetY;

    public float targetTime { get { return Time.time; } }

    public void Initialize(int channel, float noteSpeed, float judgementLineY)
    {
        this.channel = channel;
        speed = noteSpeed;
        targetY = judgementLineY;
    }

    private void Update()
    {
        // 노트가 내려오는 로직
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // 판정선에 도달하면 삭제
        if (transform.position.y <= -3.0f)
        {
            Destroy(gameObject);
        }
    }

    public void Hit()
    {
        // 노트가 맞았을 때의 처리 (예: 이펙트, 점수 추가, 노트 비활성화 등)
        NoteManager.Instance.RemoveNoteFromActiveList(this);
        Destroy(gameObject);
    }
}
