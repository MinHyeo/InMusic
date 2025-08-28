using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Play 
{
    public class Metronome : Singleton<Metronome>
    {
        [SerializeField]
        private AudioSource hitSource;
        [Space(10)]
        [SerializeField]
        private GameObject linePrefab;
        [SerializeField]
        private Transform lineSpawnPoint;
        [SerializeField]
        private float lineSpeed;
        [SerializeField]
        private Transform judgementLine;

        private float nextSample;
        private float samplesPerBeat;
        private float beatIntervalMs;
        private float songBpm = 92f;
        private float stdBpm = 60;

        [SerializeField]
        private float defaultOffset = 0.05f;

        private bool isStart = false;
        private int underBeats = 4;
        private int upperBeats = 1;
        private float measureInterval;
        private float travelTime;
        public readonly float preStartDelay = 2.0f;

        //��ȯ�Ǿ� �ִ� ���ڼ� ������ ����Ʈ
        private List<GameObject> linesObject = new List<GameObject>();

        private void Update()
        {
            if (!isStart)
                return;

            // �� ����ð� * ���ļ��� ���� ���κ��� ũ�ų� ������ �۵�.
            if (SoundManager.Instance.GetTimelinePosition() >= nextSample)
            {
                StartCoroutine(PlayTicks());
            }
        }
        
        public void CalculateSync()
        {
            //������Ʈ Ǯ�� ������Ʈ �̸� ����
            ObjectPoolManager.Instance.CreatePool(linePrefab);

            // �� ������ ���� ���� ���
            //frequency = SoundManager.Instance.frequency;
            //samplesPerBeat = (stdBpm / songBpm) * frequency;
            //beatIntervalMs = (samplesPerBeat / frequency) * 1000.0f;

            // �� ���� ���� (4/4���� ����)
            //measureInterval = samplesPerBeat * 4.0f / frequency;

            // �ð� ��� ���
            beatIntervalMs = 60000f / songBpm;
            measureInterval = beatIntervalMs * 4;

            // ���� ���� ���������� �����ϴ� �� �ɸ��� �ð� ���
            float distanceToJudgementLine = lineSpawnPoint.position.y - judgementLine.position.y;
            travelTime = distanceToJudgementLine / lineSpeed;

            NoteManager.Instance.SetTimingInfo(measureInterval, travelTime);

            nextSample = SoundManager.Instance.GetTimelinePosition();
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
                //GameObject newLine = Instantiate(linePrefab, lineSpawnPoint.position, Quaternion.identity);
                GameObject newLine = ObjectPoolManager.Instance.GetFromPool("Line");
                newLine.transform.position = lineSpawnPoint.position;
                newLine.GetComponent<Line>().Initialize(lineSpeed, judgementLine.position.y);
                linesObject.Add(newLine);
            }
        }
        private IEnumerator PlayTicks()
        {
            Debug.Log($"samplesPerBeat : {samplesPerBeat}");

            nextSample += beatIntervalMs;// - (frequency * defaultOffset);

            if (upperBeats == 1)
            {
                float measureStartTime = SoundManager.Instance.GetTimelinePosition() + measureInterval;

                float unitySpawnDelay = (measureStartTime - travelTime) / 1000f - Time.time;

                StartCoroutine(SpawnMeasureLine(unitySpawnDelay));
            }

            upperBeats++;
            if (upperBeats > underBeats)
                upperBeats = 1;

            yield return null;
        }

        private IEnumerator SpawnMeasureLine(float delaySeconds)
        {
            // spawnTime���� ���
            yield return new WaitForSeconds(delaySeconds);

            // ���� �� ����
            Debug.Log("��Ʈ ����");
            GameObject newLine = ObjectPoolManager.Instance.GetFromPool("Line");
            newLine.transform.position = lineSpawnPoint.position;
            newLine.GetComponent<Line>().Initialize(lineSpeed, judgementLine.position.y);
        }

        public void RemoveLine(GameObject line)
        {
            linesObject.Remove(line);
            ObjectPoolManager.Instance.ReleaseToPool("Line", line);
        }

        public void Restart()
        {
            isStart = false;

            //ȭ�鿡 �ִ� ���� �� ����
            foreach (GameObject line in linesObject.ToList())
            {
                RemoveLine(line);
            }
            StopAllCoroutines();
        }
    }
}