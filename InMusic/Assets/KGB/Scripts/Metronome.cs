using UnityEngine;

public class Metronome : MonoBehaviour
{
    public AudioClip tickSound;   // 박자 소리
    public float bpm = 120f;      // BPM (분당 박자 수)
    private float interval;       // 한 박자 간격 (초)
    private float nextTickTime;   // 다음 틱 소리가 울릴 시간

    private AudioSource audioSource;

    void Start()
    {
        // AudioSource를 설정하거나 자동 추가
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = tickSound;

        // BPM에 따라 한 박자 간격 계산
        interval = 60f / bpm;
        nextTickTime = Time.time + interval;  // 첫 박자 소리 시간
    }

    void FixedUpdate()
    {
        // 현재 시간이 다음 박자 소리 시간 이상이면 틱 발생
        if (Time.time >= nextTickTime)
        {
            PlayTick();
            nextTickTime += interval; // 다음 박자 시간 계산
        }
    }

    void PlayTick()
    {
        if (tickSound != null)
            audioSource.PlayOneShot(tickSound); // 박자 소리 재생
        Debug.Log("Tick!"); // 콘솔에 표시 (디버깅용)
    }
}
