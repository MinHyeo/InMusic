using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    public static Metronome Instance;

    [SerializeField]
    private AudioSource hitSource;
    [SerializeField] 
    private GameObject linePrefab;
    [SerializeField]
    private Transform lineSpawnPoint;
    [SerializeField] 
    private float lineSpeed;
    [SerializeField]
    private Transform judgementLine;
    [SerializeField]
    private TextMeshProUGUI text;

    private float nextSample;
    private float samplesPerBeat;
    private float songBpm = 92f;
    private float stdBpm = 60;

    [SerializeField]
    private float defaultOffset = 0.05f;

    private bool isStart = false;
    private int underBeats = 4;
    private int upperBeats = 1;
    private float frequency;
    private float measureInterval;
    private float travelTime;
    public readonly float preStartDelay = 2.0f;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!isStart) 
            return;

        // 곡 진행시간 * 주파수가 다음 라인보다 크거나 같으면 작동.
        if (SoundManager.Instance.positionInSamples >= nextSample)
        {
            StartCoroutine(PlayTicks());
        }
    }

    public void OnClickButton()
    {
        CalculateSync();
    }
    public void CalculateSync()
    {
        // 한 박자의 샘플 간격 계산
        frequency = SoundManager.Instance.frequency;
        samplesPerBeat = (stdBpm / songBpm) * frequency;
        //nextSample = samplesPerBeat;// - (frequency * defaultOffset);

        // 한 마디 간격 (4/4박자 기준)
        measureInterval = samplesPerBeat * 4.0f / frequency;

        // 마디 선이 판정선까지 도달하는 데 걸리는 시간 계산
        float distanceToJudgementLine = lineSpawnPoint.position.y - judgementLine.position.y;
        travelTime = distanceToJudgementLine / lineSpeed;

        NoteManager.Instance.SetTimingInfo(samplesPerBeat, measureInterval, travelTime);
    }
    public void StartMetronome()
    {
        isStart = true;
    }
    public void StartInitialMetronome()
    {
        StartCoroutine(SpawnInitialMeasureLines());
    }
    private IEnumerator SpawnInitialMeasureLines()
    {
        float initialSpawnTime = preStartDelay - travelTime;
        if (initialSpawnTime > 0)
        {
            yield return new WaitForSeconds(initialSpawnTime);

            // 마디 선 생성
            GameObject newLine = Instantiate(linePrefab, lineSpawnPoint.position, Quaternion.identity);
            newLine.GetComponent<Line>().Initialize(lineSpeed, judgementLine.position.y);
            Debug.Log("마디 선 생성");
        }
    }
    private IEnumerator PlayTicks()
    {
        hitSource.Play();
        nextSample += samplesPerBeat - (frequency * defaultOffset);
        text.text = $"BPM - 92 : {upperBeats} / {underBeats}";
        if(upperBeats == 1)
        {
            float measureStartTime = Time.time + measureInterval;
            StartCoroutine(SpawnMeasureLine(measureStartTime - travelTime));
        }
        upperBeats++;
        if (upperBeats > underBeats)
        {
            upperBeats = 1;
        }

        yield return null;
    }

    private IEnumerator SpawnMeasureLine(float spawnTime)
    {
        // spawnTime까지 대기
        yield return new WaitForSeconds(spawnTime - Time.time);

        // 마디 선 생성
        GameObject newLine = Instantiate(linePrefab, lineSpawnPoint.position, Quaternion.identity);
        newLine.GetComponent<Line>().Initialize(lineSpeed, judgementLine.position.y);
        Debug.Log("마디 선 생성");
    }
}