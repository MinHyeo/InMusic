using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    public static Metronome instance;

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

    private AudioSource audioSource;
    private float nextSample;
    private float samplesPerBeat;
    private float songBpm = 92f;
    private float stdBpm = 60;

    private const float defaultOffset = 0.05f;

    private bool isStart = false;
    private int underBeats = 4;
    private int upperBeats = 1;
    private float measureInterval;
    private float travelTime;
    public float preStartDelay = 2.0f;

    private void Awake()
    {
        instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!isStart) 
            return;

        // 곡 진행시간 * 주파수가 다음 라인보다 크거나 같으면 작동.
        if (audioSource.timeSamples >= nextSample)
        {
            StartCoroutine(PlayTicks());
        }
    }

    public void OnClickButton()
    {
        CalculateSync();
        StartCoroutine(SpawnInitialMeasureLines());
        StartCoroutine(StartMusicWithIntroDelay());
    }
    private IEnumerator StartMusicWithIntroDelay()
    {
        // 2초 인트로 시간 대기
        yield return new WaitForSeconds(preStartDelay);

        // 음악 재생 및 메트로놈 시작
        audioSource.Play();
        NoteManager.instance.InitializeNotes(BmsLoader.instance.songInfo);
        isStart = true;
    }
    private void CalculateSync()
    {
        // 한 박자의 샘플 간격 계산
        samplesPerBeat = (stdBpm / songBpm) * audioSource.clip.frequency;
        nextSample = samplesPerBeat - (audioSource.clip.frequency * defaultOffset);

        // 한 마디 간격 (4/4박자 기준)
        measureInterval = samplesPerBeat * 4.0f / audioSource.clip.frequency;

        // 마디 선이 판정선까지 도달하는 데 걸리는 시간 계산
        float distanceToJudgementLine = lineSpawnPoint.position.y - judgementLine.position.y;
        travelTime = distanceToJudgementLine / lineSpeed;

    }
    private IEnumerator SpawnInitialMeasureLines()
    {
        // 초기 딜레이 동안 마디 선을 미리 생성
        float currentTime = -travelTime;

        // 인트로 딜레이 동안 필요한 마디 선을 생성
        while (currentTime < 0)
        {
            float spawnTime = Time.time + currentTime;
            StartCoroutine(SpawnMeasureLine(spawnTime));
            currentTime += measureInterval;
        }

        yield return null;
    }
    private IEnumerator PlayTicks()
    {
        hitSource.Play();
        nextSample += (stdBpm / songBpm) * audioSource.clip.frequency;
        text.text = $"BPM - 92 : {upperBeats++} / {underBeats}";
        if (upperBeats > underBeats)
        {
            upperBeats = 1;
            float measureStartTime = Time.time + measureInterval;
            StartCoroutine(SpawnMeasureLine(measureStartTime - travelTime));
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
