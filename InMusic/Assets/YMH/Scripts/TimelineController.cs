using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Play
{

    [System.Serializable]
    public class NoteData
    {
        public int bar;
        public int channel;
        public List<int> noteData;
    }

    public class TimelineController : SingleTon<TimelineController>
    {
        // 오브젝트 프리팹
        [SerializeField] private GameObject linePrefab;
        [SerializeField] private GameObject note1Prefab;
        [SerializeField] private GameObject note2Prefab;
        // 노트 및 박자선 생성 위치
        [SerializeField] private Transform lineSpawnPoint;
        [SerializeField] private Transform[] noteSpawnPoints;
        // 판정선
        [SerializeField] private Transform judgementLine;

        // 시작 체크
        bool isStart = false;

        // 노트 및 박자선 생성에 필요한 변수
        private float bpm = 92f;
        private float beatIntervalMs;
        private float measureInterval;
        private float travelTime;
        private int beatCount = 0;
        private float speed = 5f;
        private float startDelayBeats = 2f;
        private float songStartTime = 0f;
        public float SongStartTime { get { return songStartTime; } private set { } }
        private float nextSample;
        private int underBeats = 4;
        private int upperBeats = 1;

        // 노트 소환 리스트 코루틴
        private List<Coroutine> coroutines = new List<Coroutine>();

        // 노트 데이터 리스트 및 생성된 리스트
        private List<NoteData> noteDataList;
        private int noteCount;
        private List<GameObject> linesObject = new List<GameObject>();
        private List<Note> activeNotes = new();

        public void Initialize(SongInfo songInfo)
        {
            // 오브젝트 풀 생성
            ObjectPoolManager.Instance.CreatePool(linePrefab);
            ObjectPoolManager.Instance.CreatePool(note1Prefab);
            ObjectPoolManager.Instance.CreatePool(note2Prefab);

            // 곡 정보 받아오기
            noteDataList = songInfo.NoteList;
            noteCount = songInfo.NoteCount;

            // 비트 및 마디 계산
            beatIntervalMs = 60000f / bpm;
            measureInterval = beatIntervalMs * 4f;
            // 내려오는데 걸리는 시간
            float distanceToJudgementLine = lineSpawnPoint.position.y - judgementLine.position.y;
            travelTime = distanceToJudgementLine / speed;

            // 시작 시간 계산(2마디 후 노래 시작)
            float beatDelayTime = measureInterval * startDelayBeats / 1000f;
            songStartTime = Time.time + beatDelayTime;
            Debug.Log(songStartTime);

            nextSample = SoundManager.Instance.GetTimelinePosition();

            isStart = true;

            StartCoroutine(SpawnRoutine());
        }

        private void Update()
        {
            if (!isStart)
                return;

            // 곡 진행시간 * 주파수가 다음 라인보다 크거나 같으면 작동.
            if (SoundManager.Instance.GetTimelinePosition() >= nextSample)
            {
                StartCoroutine(PlayTicks());
            }
        }

        private IEnumerator PlayTicks()
        {
            nextSample += beatIntervalMs;

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
            // spawnTime까지 대기
            yield return new WaitForSeconds(delaySeconds);

            GameObject newLine = ObjectPoolManager.Instance.GetFromPool("Line");
            newLine.transform.position = lineSpawnPoint.position;
            newLine.GetComponent<Line>().Initialize(speed, judgementLine.position.y);
            linesObject.Add(newLine);
        }

        private IEnumerator SpawnRoutine()
        {
            foreach (NoteData noteData in noteDataList)
            {
                float barTime = noteData.bar * (measureInterval / 1000);
                int divisions = noteData.noteData.Count;

                for (int i = 0; i < divisions; i++)
                {
                    int noteValue = noteData.noteData[i];
                    if (noteValue == 0) continue;

                    int channel = noteData.channel;
                    float noteAppearTime = barTime + ((measureInterval / divisions) / 1000) * i;
                    float spawnTime = songStartTime + noteAppearTime - travelTime;

                    coroutines.Add(StartCoroutine(SpawnNote(channel, spawnTime)));
                }

                yield return null;
            }
        }

        private IEnumerator SpawnNote(int channel, float spawnTime)
        {
            yield return new WaitForSeconds(spawnTime - Time.time);

            GameObject prefab = (channel == 11 || channel == 14) ? note1Prefab : note2Prefab;
            GameObject note = ObjectPoolManager.Instance.GetFromPool(prefab.name);
            note.transform.position = noteSpawnPoints[channel - 11].position;

            Note noteScript = note.GetComponent<Note>();
            noteScript.Initialize(channel, speed, 1000000 / noteCount, travelTime);
            activeNotes.Add(noteScript);
        }

        public Note GetClosestNote(int channel, float pressTime)
        {
            Note closestNote = null;
            float minTimeDifference = float.MaxValue;

            foreach (Note note in activeNotes)
            {
                if (note.Channel == channel)
                {
                    float timeDifference = Mathf.Abs(note.targetTime - pressTime);
                    if (timeDifference < minTimeDifference)
                    {
                        minTimeDifference = timeDifference;
                        closestNote = note;
                    }
                }
            }

            return closestNote;
        }

        public void RemoveNote(Note note)
        {
            if (activeNotes.Contains(note))
            {
                activeNotes.Remove(note);
                ObjectPoolManager.Instance.ReleaseToPool(note.gameObject.name, note.gameObject);
            }
        }

        public void RemoveLine(GameObject line)
        {
            if (linesObject.Contains(line))
            {
                linesObject.Remove(line);
                ObjectPoolManager.Instance.ReleaseToPool("Line", line);
            }
        }

        public void ClearAll()
        {
            foreach (var line in linesObject)
                ObjectPoolManager.Instance.ReleaseToPool("Line", line);
            linesObject.Clear();

            foreach (var note in activeNotes)
                ObjectPoolManager.Instance.ReleaseToPool(note.gameObject.name, note.gameObject);
            activeNotes.Clear();
        }

        public void RemoveJudgementLine()
        {
            judgementLine.gameObject.SetActive(false);
        }
    }
}