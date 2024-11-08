using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TestCode : MonoBehaviour
{
    [SerializeField]
    private AudioSource hitSource;
    private AudioSource audioSource;
    [SerializeField]
    private TextMeshProUGUI text;

    private float offsetSample;
    private float nextSample;
    private float samplesPerBeat;
    private float songBpm = 92f;
    private float stdBpm = 60;

    private const float defaultOffset = 0.05f;

    private bool isStart = false;
    private int underBeats = 4;
    private int upperBeats = 2;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!isStart) return;

        // �� ����ð� * ���ļ��� ���� ���κ��� ũ�ų� ������ �۵�.
        if (audioSource.timeSamples >= nextSample)
        {
            StartCoroutine(PlayTicks());
        }
    }

    public void OnClickButton()
    {
        CalculateSync();

        audioSource.Play();
        isStart = true;
    }

    private void CalculateSync()
    {
        // �� ������ �ð� * ���ļ� (���� bpm�� ����, ���ڿ� ���� �ٲ��.
        // ����� (����BPM(60) / ���� BPM) * ����(4/4=1) * ���� ���ļ��� ����Ѵ�)
        samplesPerBeat = (stdBpm / 92) * 1 * audioSource.clip.frequency;
        // ���� ���������� ���ļ��� ����. (�ϵ����/����Ʈ���� �������� ���� 0.05�������� ���ؾ� �´�)
        //offsetSample = audioSource.clip.frequency * (1 + defaultOffset);
        // ���� ���� �ð� * ���ļ�
        nextSample = samplesPerBeat - offsetSample;
    }

    private IEnumerator PlayTicks()
    {
        hitSource.Play();
        nextSample += (stdBpm / 92) * audioSource.clip.frequency;
        text.text = $"BPM - 92 : {upperBeats++} / {underBeats}";
        if (upperBeats > underBeats) upperBeats = 1;

        yield return null;
    }
}
