using UnityEngine;

public class Metronome : MonoBehaviour
{
    public AudioClip tickSound;   // ���� �Ҹ�
    public float bpm = 120f;      // BPM (�д� ���� ��)
    private float interval;       // �� ���� ���� (��)
    private float nextTickTime;   // ���� ƽ �Ҹ��� �︱ �ð�

    private AudioSource audioSource;

    void Start()
    {
        // AudioSource�� �����ϰų� �ڵ� �߰�
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = tickSound;

        // BPM�� ���� �� ���� ���� ���
        interval = 60f / bpm;
        nextTickTime = Time.time + interval;  // ù ���� �Ҹ� �ð�
    }

    void FixedUpdate()
    {
        // ���� �ð��� ���� ���� �Ҹ� �ð� �̻��̸� ƽ �߻�
        if (Time.time >= nextTickTime)
        {
            PlayTick();
            nextTickTime += interval; // ���� ���� �ð� ���
        }
    }

    void PlayTick()
    {
        if (tickSound != null)
            audioSource.PlayOneShot(tickSound); // ���� �Ҹ� ���
        Debug.Log("Tick!"); // �ֿܼ� ǥ�� (������)
    }
}
