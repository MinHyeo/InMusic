using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

namespace Play 
{
    public class Metronome : SingleTon<Metronome>
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

        //��ȯ�Ǿ� �ִ� ���ڼ� ������ ����Ʈ
        private List<GameObject> linesObject = new List<GameObject>();

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
            //������Ʈ Ǯ�� ������Ʈ �̸� ����
            ObjectPoolManager.Instance.CreatePool(linePrefab);

            // �� ������ ���� ���� ���
            frequency = SoundManager.Instance.frequency;
            samplesPerBeat = (stdBpm / songBpm) * frequency;
            //nextSample = samplesPerBeat;// - (frequency * defaultOffset);

            // �� ���� ���� (4/4���� ����)
            measureInterval = samplesPerBeat * 4.0f / frequency;

            // ���� ���� ���������� �����ϴ� �� �ɸ��� �ð� ���
            float distanceToJudgementLine = lineSpawnPoint.position.y - judgementLine.position.y;
            travelTime = distanceToJudgementLine / lineSpeed;

            NoteManager.Instance.SetTimingInfo(measureInterval, travelTime);
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
            //hitSource.Play();
            nextSample += samplesPerBeat;// - (frequency * defaultOffset);
            text.text = $"BPM - 92 : {upperBeats} / {underBeats}";
            if (upperBeats == 1)
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
            //GameObject newLine = Instantiate(linePrefab, lineSpawnPoint.position, Quaternion.identity);
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