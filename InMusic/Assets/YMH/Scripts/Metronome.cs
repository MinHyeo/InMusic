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

        //소환되어 있는 박자선 저장할 리스트
        private List<GameObject> linesObject = new List<GameObject>();

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
            //오브젝트 풀로 오브젝트 미리 생성
            ObjectPoolManager.Instance.CreatePool(linePrefab);

            // 한 박자의 샘플 간격 계산
            frequency = SoundManager.Instance.frequency;
            samplesPerBeat = (stdBpm / songBpm) * frequency;
            //nextSample = samplesPerBeat;// - (frequency * defaultOffset);

            // 한 마디 간격 (4/4박자 기준)
            measureInterval = samplesPerBeat * 4.0f / frequency;

            // 마디 선이 판정선까지 도달하는 데 걸리는 시간 계산
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

                // 마디 선 생성
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
            // spawnTime까지 대기
            yield return new WaitForSeconds(spawnTime - Time.time);

            // 마디 선 생성
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

            //화면에 있는 마디 선 삭제
            foreach (GameObject line in linesObject.ToList())
            {
                RemoveLine(line);
            }
            StopAllCoroutines();
        }
    }
}