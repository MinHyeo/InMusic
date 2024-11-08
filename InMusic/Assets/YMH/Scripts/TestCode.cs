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

        // 곡 진행시간 * 주파수가 다음 라인보다 크거나 같으면 작동.
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
        // 한 마디의 시간 * 주파수 (값은 bpm에 따라, 박자에 따라 바뀐다.
        // 현재는 (기준BPM(60) / 곡의 BPM) * 박자(4/4=1) * 곡의 주파수로 계산한다)
        samplesPerBeat = (stdBpm / 92) * 1 * audioSource.clip.frequency;
        // 곡의 시작지점을 주파수와 곱함. (하드웨어/소프트웨어 지연으로 인해 0.05초정도를 더해야 맞다)
        //offsetSample = audioSource.clip.frequency * (1 + defaultOffset);
        // 다음 정박 시간 * 주파수
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
