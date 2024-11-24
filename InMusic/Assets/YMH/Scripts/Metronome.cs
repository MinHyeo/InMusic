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

        // �� ����ð� * ���ļ��� ���� ���κ��� ũ�ų� ������ �۵�.
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
        // �� ������ ���� ���� ���
        frequency = SoundManager.Instance.frequency;
        samplesPerBeat = (stdBpm / songBpm) * frequency;
        //nextSample = samplesPerBeat;// - (frequency * defaultOffset);

        // �� ���� ���� (4/4���� ����)
        measureInterval = samplesPerBeat * 4.0f / frequency;

        // ���� ���� ���������� �����ϴ� �� �ɸ��� �ð� ���
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

            // ���� �� ����
            GameObject newLine = Instantiate(linePrefab, lineSpawnPoint.position, Quaternion.identity);
            newLine.GetComponent<Line>().Initialize(lineSpeed, judgementLine.position.y);
            Debug.Log("���� �� ����");
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
        // spawnTime���� ���
        yield return new WaitForSeconds(spawnTime - Time.time);

        // ���� �� ����
        GameObject newLine = Instantiate(linePrefab, lineSpawnPoint.position, Quaternion.identity);
        newLine.GetComponent<Line>().Initialize(lineSpeed, judgementLine.position.y);
        Debug.Log("���� �� ����");
    }
}