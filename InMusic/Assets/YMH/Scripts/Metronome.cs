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

        // �� ����ð� * ���ļ��� ���� ���κ��� ũ�ų� ������ �۵�.
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
        // 2�� ��Ʈ�� �ð� ���
        yield return new WaitForSeconds(preStartDelay);

        // ���� ��� �� ��Ʈ�γ� ����
        audioSource.Play();
        NoteManager.instance.InitializeNotes(BmsLoader.instance.songInfo);
        isStart = true;
    }
    private void CalculateSync()
    {
        // �� ������ ���� ���� ���
        samplesPerBeat = (stdBpm / songBpm) * audioSource.clip.frequency;
        nextSample = samplesPerBeat - (audioSource.clip.frequency * defaultOffset);

        // �� ���� ���� (4/4���� ����)
        measureInterval = samplesPerBeat * 4.0f / audioSource.clip.frequency;

        // ���� ���� ���������� �����ϴ� �� �ɸ��� �ð� ���
        float distanceToJudgementLine = lineSpawnPoint.position.y - judgementLine.position.y;
        travelTime = distanceToJudgementLine / lineSpeed;

    }
    private IEnumerator SpawnInitialMeasureLines()
    {
        // �ʱ� ������ ���� ���� ���� �̸� ����
        float currentTime = -travelTime;

        // ��Ʈ�� ������ ���� �ʿ��� ���� ���� ����
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
        // spawnTime���� ���
        yield return new WaitForSeconds(spawnTime - Time.time);

        // ���� �� ����
        GameObject newLine = Instantiate(linePrefab, lineSpawnPoint.position, Quaternion.identity);
        newLine.GetComponent<Line>().Initialize(lineSpeed, judgementLine.position.y);
        Debug.Log("���� �� ����");
    }
}
