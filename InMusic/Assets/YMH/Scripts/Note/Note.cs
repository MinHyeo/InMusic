using System.Collections;
using UnityEngine;

public class Note : MonoBehaviour
{
    private float speed;
    private Vector3 direction = Vector3.down;  // 노트가 내려오는 방향

    public void Initialize(NoteData noteData, float bpm)
    {
        // 속도 계산 (BPM에 따라 조절)
        float bps = bpm / 60.0f;
        speed = bps * 2;  // 스피드 설정 (조정 가능)
    }

    void Update()
    {
        // 속도에 맞춰 노트를 아래로 이동
        transform.Translate(direction * speed * Time.deltaTime);
    }
}